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

	public IEnumerable<Node> Children => children;
	private ObservableCollection<Node> children = new();

	private Node parent;
	public Node Parent
	{
		get => parent;
		set
		{
			Debug.Assert(value != this, "Nodes cannot be parented to themselves.");
			Debug.Assert(value == null || value.Scene == Scene,
				"Nodes can only be parented to other nodes from the same scene.");

			if (parent != value || value == null /*Could be initial setup...*/)
			{
				if (parent == null)
				{
					Scene.RemoveRootNode(this);
				}
				if (value == null)
				{
					Scene.AddRootNode(this);
				}

				parent?.children.Remove(this);
				parent = value;
				parent?.children.Add(this);
			}
		}
	}

	public Node(Scene scene)
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
		if (parent == null)
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

	public virtual void Dispose()
	{
		// Make sure we're not still selected.
		Selection.Deselect(this);

		// Remove self from scene tree.
		Parent = null;

		foreach (var child in children.Reverse())
		{
			child.Dispose();
		}
	}

	public virtual void OnSelect() {}
	public virtual void OnDeselect() {}
}