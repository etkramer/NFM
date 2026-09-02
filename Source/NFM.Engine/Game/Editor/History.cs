using System.Reflection;
using NFM.World;

namespace NFM;

/// <summary>
/// The editor's undo stack. A change is recorded by snapshotting whatever it's about to touch and
/// diffing that once it's done, so a call site only has to declare what it works on:
/// <code>
/// using (History.Begin("Move"))
/// {
///     History.Track(node);
///     node.Position = destination;
/// }
/// </code>
/// </summary>
public static class History
{
	private const int MaxEntries = 64;

	/// <summary>
	/// Bumped whenever the stack moves, for panels that re-render off editor state.
	/// </summary>
	public static int Version { get; private set; }

	public static string? UndoName => undoStack.Count > 0 ? undoStack[^1].Name : null;
	public static string? RedoName => redoStack.Count > 0 ? redoStack[^1].Name : null;

	private static readonly List<Entry> undoStack = [];
	private static readonly List<Entry> redoStack = [];

	private static Entry? open;
	private static int depth;

	/// <summary>
	/// Opens a transaction, which becomes one entry on the stack once disposed. Transactions opened
	/// inside another join it, so a change built out of smaller ones still undoes in a single step.
	/// </summary>
	public static Transaction Begin(string name)
	{
		if (depth++ == 0)
		{
			open = new Entry(name, [.. Selection.Selected]);
		}

		return default;
	}

	/// <summary>
	/// Snapshots a subject's inspectable properties, along with its place in the scene tree. Anything
	/// that differs by the end of the transaction becomes part of the entry.
	/// </summary>
	public static void Track(object subject)
	{
		if (open is not Entry entry || entry.Tracked.Any(snapshot => ReferenceEquals(snapshot.Subject, subject)))
		{
			return;
		}

		PropertyInfo[] properties = NodeSerializer.GetSavedProperties(subject.GetType());
		object?[] values = new object?[properties.Length];

		for (int i = 0; i < properties.Length; i++)
		{
			values[i] = properties[i].GetValue(subject);
		}

		Node? node = subject as Node;
		entry.Tracked.Add(new Snapshot(subject, properties, values, node?.Parent, node?.SiblingIndex ?? 0));
	}

	/// <summary>
	/// Records a node that was just added to the scene.
	/// </summary>
	public static void RecordSpawn(Node node)
	{
		using (Begin($"Add {node.Name}"))
		{
			Guard.NotNull(open).Edits.Add(new LifetimeEdit(node, node.Parent, node.SiblingIndex, true));
		}
	}

	/// <summary>
	/// Removes a node from the scene, holding it here so undo can put it back. Takes the place of
	/// disposing it - the node is only destroyed for real once this entry falls off the stack.
	/// </summary>
	public static void RecordDespawn(Node node)
	{
		using (Begin($"Delete {node.Name}"))
		{
			Guard.NotNull(open).Edits.Add(new LifetimeEdit(node, node.Parent, node.SiblingIndex, false));
			node.Detach();
		}
	}

	public static void Undo()
	{
		if (open is not null || undoStack.Count == 0)
		{
			return;
		}

		Entry entry = Take(undoStack);

		for (int i = entry.Edits.Count - 1; i >= 0; i--)
		{
			entry.Edits[i].Undo();
		}

		Restore(entry.SelectionBefore);
		redoStack.Add(entry);
		Version++;
	}

	public static void Redo()
	{
		if (open is not null || redoStack.Count == 0)
		{
			return;
		}

		Entry entry = Take(redoStack);

		foreach (IEdit edit in entry.Edits)
		{
			edit.Redo();
		}

		Restore(entry.SelectionAfter);
		undoStack.Add(entry);
		Version++;
	}

	/// <summary>
	/// Drops the whole stack, destroying any node still being held out of the scene.
	/// </summary>
	public static void Clear()
	{
		open = null;

		Discard(undoStack);
		Discard(redoStack);

		Version++;
	}

	internal static void End()
	{
		Guard.Require(depth > 0, "Transaction was already closed.");

		if (--depth > 0)
		{
			return;
		}

		// Nothing to commit if the transaction was abandoned partway through.
		if (open is not Entry entry)
		{
			return;
		}

		open = null;

		foreach (Snapshot snapshot in entry.Tracked)
		{
			Diff(snapshot, entry.Edits);
		}

		// A transaction that changed nothing leaves the stack alone, redo included.
		if (entry.Edits.Count == 0)
		{
			return;
		}

		entry.SelectionAfter = [.. Selection.Selected];
		Push(entry);
	}

	private static void Diff(Snapshot snapshot, List<IEdit> into)
	{
		for (int i = 0; i < snapshot.Properties.Length; i++)
		{
			PropertyInfo property = snapshot.Properties[i];
			object? value = property.GetValue(snapshot.Subject);

			if (property.CanWrite && !Equals(value, snapshot.Values[i]))
			{
				into.Add(new PropertyEdit(snapshot.Subject, property, snapshot.Values[i], value));
			}
		}

		// A node that left the scene entirely is covered by the edit that removed it.
		if (snapshot.Subject is Node node && !node.IsDetached &&
			(node.Parent != snapshot.Parent || node.SiblingIndex != snapshot.Index))
		{
			into.Add(new TreeEdit(node, snapshot.Parent, snapshot.Index, node.Parent, node.SiblingIndex));
		}
	}

	private static void Push(Entry entry)
	{
		Discard(redoStack);

		undoStack.Add(entry);

		while (undoStack.Count > MaxEntries)
		{
			undoStack[0].Discard();
			undoStack.RemoveAt(0);
		}

		Version++;
	}

	private static Entry Take(List<Entry> stack)
	{
		Entry entry = stack[^1];
		stack.RemoveAt(stack.Count - 1);

		return entry;
	}

	private static void Discard(List<Entry> stack)
	{
		foreach (Entry entry in stack)
		{
			entry.Discard();
		}

		stack.Clear();
	}

	private static void Restore(ISelectable[] items)
	{
		Selection.DeselectAll();
		Selection.Select(items.Where(item => item is not Node node || !node.IsDetached));
	}

	private sealed record Snapshot(object Subject, PropertyInfo[] Properties, object?[] Values, Node? Parent, int Index);

	private sealed class Entry(string name, ISelectable[] selectionBefore)
	{
		public string Name { get; } = name;

		public List<Snapshot> Tracked { get; } = [];
		public List<IEdit> Edits { get; } = [];

		public ISelectable[] SelectionBefore { get; } = selectionBefore;
		public ISelectable[] SelectionAfter { get; set; } = [];

		public void Discard()
		{
			foreach (IEdit edit in Edits)
			{
				edit.Discard();
			}
		}
	}

	private interface IEdit
	{
		void Undo();
		void Redo();

		/// <summary>
		/// Releases anything this edit was keeping alive on the stack's behalf.
		/// </summary>
		void Discard() {}
	}

	private sealed class PropertyEdit(object subject, PropertyInfo property, object? before, object? after) : IEdit
	{
		public void Undo() => property.SetValue(subject, before);
		public void Redo() => property.SetValue(subject, after);
	}

	private sealed class TreeEdit(Node node, Node? beforeParent, int beforeIndex, Node? afterParent, int afterIndex) : IEdit
	{
		public void Undo() => node.MoveTo(beforeParent, beforeIndex);
		public void Redo() => node.MoveTo(afterParent, afterIndex);
	}

	/// <summary>
	/// A node entering or leaving the scene. Whichever side of it is out of the scene is held by this
	/// edit, and destroyed along with it.
	/// </summary>
	private sealed class LifetimeEdit(Node node, Node? parent, int index, bool spawned) : IEdit
	{
		public void Undo()
		{
			if (spawned)
			{
				node.Detach();
			}
			else
			{
				node.Attach(parent, index);
			}
		}

		public void Redo()
		{
			if (spawned)
			{
				node.Attach(parent, index);
			}
			else
			{
				node.Detach();
			}
		}

		public void Discard()
		{
			if (node.IsDetached)
			{
				node.Dispose();
			}
		}
	}
}

/// <summary>
/// Scope handle for <see cref="History.Begin"/>.
/// </summary>
public readonly struct Transaction : IDisposable
{
	public void Dispose() => History.End();
}
