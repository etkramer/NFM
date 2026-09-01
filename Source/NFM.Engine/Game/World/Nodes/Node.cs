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

	public IEnumerable<Node> Children => children;
	private readonly ObservableCollection<Node> children = [];

	/// <summary>
	/// Every node spawned by this one, depth-first.
	/// </summary>
	public IEnumerable<Node> OwnedNodes
	{
		get
		{
			Stack<Node> pending = new(children.Where(child => child.Owner == this));

			while (pending.TryPop(out Node? node))
			{
				yield return node;

				foreach (Node child in node.Children.Where(child => child.Owner == this))
				{
					pending.Push(child);
				}
			}
		}
	}

	/// <summary>
	/// This node's key path relative to its owner, e.g. "Spine/Neck/Head".
	/// </summary>
	public string? OwnerPath
	{
		get
		{
			if (!IsOwned)
			{
				return null;
			}

			List<string> keys = [];
			for (Node? node = this; node is not null && node.Owner == Owner; node = node.Parent)
			{
				keys.Add(Guard.NotNull(node.OwnerKey));
			}

			keys.Reverse();
			return string.Join('/', keys);
		}
	}

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
	/// Resolves a key path produced by <see cref="OwnerPath"/> against this node's owned subtree.
	/// </summary>
	public Node? FindOwned(string path)
	{
		Node? node = this;

		foreach (string key in path.Split('/'))
		{
			node = node?.children.FirstOrDefault(child => child.Owner == this && child.OwnerKey == key);
		}

		return node;
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