using System;
using System.Reactive.Linq;
using Engine.Editor;
using Engine.Rendering;
using ReactiveUI;

namespace Engine.World
{
	[Icon('\uE3C2')]
	public class Node : ISelectable, IDisposable
	{
		[Inspect] public string Name { get; set; }

		[Inspect] public Vector3 Position { get; set; } = Vector3.Zero;
		[Inspect] public Vector3 Rotation { get; set; } = Vector3.Zero;
		[Inspect] public Vector3 Scale { get; set; } = Vector3.One;

		[Notify]
		public Matrix4 Transform { get; private set; } = Matrix4.Identity;

		public Scene Scene { get; }

		// Node heirarchy
		public ReadOnlyObservableCollection<Node> Children { get; }
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
			string name = GetType().Name.PascalToDisplay();
			if (name.EndsWith(" Node"))
			{
				name = name.Remove(name.Length - " Node".Length);
			}

			Name = name;
			Scene = scene ?? Scene.Main;
			Parent = null;
			Children = new(children);
		}

		public virtual void Dispose()
		{
			// Make sure we're not still selected.
			Selection.Deselect(this);

			// Remove self from scene tree.
			Parent = null;

			foreach (var child in children)
			{
				child.Dispose();
			}
		}

		public virtual void Tick()
		{
			Transform = EvaluateTransform();
		}

		public virtual Matrix4 EvaluateTransform()
		{
			// Grab base transform.
			Matrix4 result = Matrix4.CreateTransform(Position, Rotation, Scale);

			// Apply parent transform.
			if (parent != null)
			{
				result *= parent.Transform;
			}

			return result;
		}

		public virtual void DrawGizmos(GizmosContext context) {}

		/// <summary>
		/// Enumerates up the node heirarchy, toward the scene root.
		/// </summary>
		public IEnumerable<Node> EnumerateUpward()
		{
			if (Parent != null)
			{
				yield return Parent;

				foreach (var node in Parent.EnumerateUpward())
				{
					yield return node;
				}
			}
		}
	}
}