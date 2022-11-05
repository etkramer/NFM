using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using Engine.Editor;
using Engine.GPU;
using Engine.Rendering;
using ReactiveUI;

namespace Engine.World
{
	[Icon('\uE3C2')]
	public class Node : ISelectable, IDisposable
	{
		// Properties (inspectable)
		[Inspect] public string Name { get; set; }
		[Inspect] public Vector3 Position { get; set; } = Vector3.Zero;
		[Inspect] public Vector3 Rotation { get; set; } = Vector3.Zero;
		[Inspect] public Vector3 Scale { get; set; } = Vector3.One;

		public bool IsTransformValid { get; protected set; } = false;

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
				Debug.Assert(value == null || value.Scene == Scene,
					"Nodes can only be parented to other nodes from the same scene.");

				Debug.Assert(value != this,
					"Nodes cannot be parented to themselves.");

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

			// Track changes in transform
			this.WhenAnyValue(o => o.Position, o => o.Rotation, o => o.Scale)
				.Subscribe((o) => InvalidateTransform());
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

		public void InvalidateTransform()
		{
			IsTransformValid = false;

			foreach (var child in children)
			{
				child.InvalidateTransform();
			}
		}

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

		public virtual void OnDrawGizmos(GizmosContext context) {}
	}

	/// <summary>
	/// A node with a lifespan tied to it's owner and can't be reparented.
	/// </summary>
	public class ChildNode : Node
	{
		public Node Owner { get; }

		public ChildNode(Node owner) : base(owner.Scene)
		{
			Owner = owner;
		}
	}
}