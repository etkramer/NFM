namespace NFM.World;

[Icon("crop_free")]
public class Node : ISelectable, IDisposable
{
	[Inspect] public string Name { get; set; }

	[Inspect] public Vector3 Position { get; set; } = Vector3.Zero;
	[Inspect] public Vector3 Rotation { get; set; } = Vector3.Zero;
	[Inspect] public Vector3 Scale { get; set; } = Vector3.One;

	[Notify] public Matrix4 WorldTransform { get; private set; } = Matrix4.Identity;

	public Scene Scene { get; }

	/// <summary>
	/// The node that spawned this one, if any. Owned nodes are regenerated from their owner's source
	/// data, so they can't be reparented or deleted on their own.
	/// </summary>
	public Node? Owner { get; private set; }

	/// <summary>
	/// Identifies this node among its siblings in the owner's subtree, and stays stable across respawns.
	/// </summary>
	public string? OwnerKey { get; private set; }

	public bool IsOwned => Owner is not null;

	/// <summary>
	/// Set for editor-owned nodes that live in the scene without being part of its content, and so
	/// aren't saved alongside it.
	/// </summary>
	public bool IsTransient { get; init; }

	/// <summary>
	/// Set while this node is held out of the scene by the undo stack, waiting to be put back or
	/// destroyed. Detached nodes keep their state but render nothing.
	/// </summary>
	public bool IsDetached { get; private set; }

	public IEnumerable<Node> Children => children;
	private readonly ObservableCollection<Node> children = [];

	/// <summary>
	/// Property values captured when this node was spawned. Anything still equal to them is left out
	/// of saves.
	/// </summary>
	internal object?[]? SavedDefaults { get; set; }

	private bool isDespawning;

	private Node? parent;
	public Node? Parent
	{
		get => parent;
		set
		{
			// An owned node with no parent yet is mid-spawn; past that it's pinned where its owner put it.
			Guard.Require(!IsOwned || parent is null, "Owned nodes cannot be reparented.");
			Guard.Require(value is null || !value.IsOwned || value.Owner == Owner,
				"Nodes cannot be parented to a node owned by something else.");
			Guard.Require(value is null || value.Scene == Scene,
				"Nodes can only be parented to other nodes from the same scene.");

			for (Node? ancestor = value; ancestor is not null; ancestor = ancestor.parent)
			{
				Guard.Require(ancestor != this, "Nodes cannot be parented to themselves or their own descendants.");
			}

			if (parent == value)
			{
				return;
			}

			if (parent is null)
			{
				Scene.RemoveRootNode(this);
			}
			if (value is null)
			{
				Scene.AddRootNode(this);
			}

			parent?.children.Remove(this);
			parent = value;
			parent?.children.Add(this);

			UpdateTransform();
		}
	}

	/// <summary>
	/// Where this node sits among its siblings, which is what the outliner shows.
	/// </summary>
	internal int SiblingIndex => parent is null ? Scene.IndexOfRootNode(this) : parent.children.IndexOf(this);

	/// <summary>
	/// Takes this node's subtree out of the scene without destroying it, so the undo stack can hold
	/// onto it. The node keeps everything but its place in the tree.
	/// </summary>
	internal void Detach()
	{
		Guard.Require(!IsDetached, "Node is already detached.");
		Guard.Require(!IsOwned, "Owned nodes are detached along with their owner.");

		if (parent is null)
		{
			Scene.RemoveRootNode(this);
		}
		else
		{
			parent.children.Remove(this);
		}

		parent = null;
		SetDetached(true);
	}

	/// <summary>
	/// Puts a detached subtree back where it came from.
	/// </summary>
	internal void Attach(Node? newParent, int index)
	{
		Guard.Require(IsDetached, "Node is already attached.");

		SetDetached(false);
		Place(newParent, index);
	}

	/// <summary>
	/// Reparents this node to a given spot among its new siblings, staying in the scene throughout.
	/// </summary>
	internal void MoveTo(Node? newParent, int index)
	{
		Guard.Require(!IsOwned, "Owned nodes cannot be reparented.");

		if (parent is null)
		{
			Scene.RemoveRootNode(this);
		}
		else
		{
			parent.children.Remove(this);
		}

		Place(newParent, index);
	}

	private void Place(Node? newParent, int index)
	{
		parent = newParent;

		if (parent is null)
		{
			Scene.InsertRootNode(this, index);
		}
		else
		{
			parent.children.Insert(Math.Clamp(index, 0, parent.children.Count), this);
		}

		UpdateTransform();
	}

	private void SetDetached(bool value)
	{
		IsDetached = value;

		if (value)
		{
			Selection.Deselect(this);
		}

		OnDetachedChanged();

		foreach (Node child in children)
		{
			child.SetDetached(value);
		}
	}

	/// <summary>
	/// Called as this node enters or leaves the scene, for subclasses holding render state.
	/// </summary>
	protected virtual void OnDetachedChanged() {}

	public Node(Scene? scene)
	{
		Name = "Node";
		Scene = scene ?? Scene.Main;

		// Don't use setter for performance reasons
		parent = null;
		Scene.AddRootNode(this);

		this.SubscribeFast(nameof(Position), nameof(Rotation), nameof(Scale), UpdateTransform);
	}

	void UpdateTransform()
	{
		// Grab local transform.
		Matrix4 localTransform = Matrix4.CreateTransform(Position, Rotation, Scale);

		// Apply parent transforms.
		if (parent is null)
		{
			WorldTransform = localTransform;
		}
		else
		{
			WorldTransform = localTransform * parent.WorldTransform;
		}

		// Recursively update children.
		foreach (var child in Children)
		{
			child.UpdateTransform();
		}
	}

	/// <summary>
	/// Spawns a node owned by this one, under <paramref name="parent"/> - either this node or another
	/// node it already owns. The key addresses the spawned node across respawns.
	/// </summary>
	protected T SpawnOwned<T>(T node, string key, Node parent) where T : Node
	{
		Guard.Require(parent == this || parent.Owner == this, "Owned nodes can only be spawned into their owner's subtree.");

		node.Owner = this;
		node.OwnerKey = key;
		node.Parent = parent;

		NodeSerializer.CaptureDefaults(node);

		return node;
	}

	/// <summary>
	/// Destroys this node's owned subtree.
	/// </summary>
	protected void DespawnOwned()
	{
		isDespawning = true;

		foreach (Node owned in children.Where(child => child.Owner == this).ToArray())
		{
			owned.Dispose();
		}

		isDespawning = false;
	}

	/// <summary>
	/// Finds the child under the given key that shares this node's owned subtree.
	/// </summary>
	public Node? FindOwned(string key)
	{
		Node? owner = Owner ?? this;
		return children.FirstOrDefault(child => child.Owner == owner && child.OwnerKey == key);
	}

	public virtual void Dispose()
	{
		Guard.Require(Owner is null || Owner.isDespawning, "Owned nodes are disposed by their owner.");

		// Make sure we're not still selected.
		Selection.Deselect(this);

		// Remove self from scene tree, without re-rooting the way the Parent setter would.
		if (parent is null)
		{
			Scene.RemoveRootNode(this);
		}
		else
		{
			parent.children.Remove(this);
			parent = null;
		}

		// A node on its way out takes its owned subtree with it.
		isDespawning = true;

		foreach (var child in children.Reverse())
		{
			child.Dispose();
		}
	}

	public virtual void OnSelect() {}
	public virtual void OnDeselect() {}
}