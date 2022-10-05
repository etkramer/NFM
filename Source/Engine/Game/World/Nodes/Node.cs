using System;
using System.ComponentModel.DataAnnotations;
using Engine.Editor;
using Engine.GPU;
using Engine.Rendering;

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
		
		public Scene Scene { get; private set; }
		public ReadOnlyObservableCollection<Node> Children { get; }
		[Save, Notify] public Node Parent
		{
			get => parent;
			set
			{
				if (parent != value && value != this)
				{
					// Remove from old parent.
					Scene?.Despawn(this);
					parent?.children.Remove(this);

					// Add to new parent.
					parent = value;
					parent?.children.Add(this);	
					if (parent == null)
					{
						Scene?.Spawn(this);
					}
				}
			}
		}
		
		// Backing fields
		private Node parent;
		private readonly ObservableCollection<Node> children = new();

		// Transform buffer
		public bool IsTransformDirty = true;
		public BufferAllocation<GPUTransform> TransformHandle;

		public Node()
		{
			Children = new(children);

			string name = GetType().Name.PascalToDisplay();
			if (name.EndsWith(" Node"))
			{
				name = name.Remove(name.Length - " Node".Length);
			}

			Name = name;
			Parent = parent;

			// Transform buffer
			(this as INotify).Subscribe(nameof(Position), () => IsTransformDirty = true);
			(this as INotify).Subscribe(nameof(Rotation), () => IsTransformDirty = true);
			(this as INotify).Subscribe(nameof(Scale), () => IsTransformDirty = true);
		}

		public virtual void Dispose()
		{
			// Make sure we're not still selected.
			Selection.Deselect(this);

			// Remove self from scene tree.
			Scene?.Despawn(this);
			Parent = null;
			TransformHandle?.Dispose();

			// Dispose children.
			for (int i = Children.Count - 1; i >= 0; i--)
			{
				Children[i].Dispose();
			}
		}

		/// <summary>
		/// Spawns the actor into the scene with the given parent
		/// </summary>
		public Node Spawn(Node parent) => Spawn<Node>(parent);
		public TThis Spawn<TThis>(Node parent) where TThis : Node
		{
			if (parent != null)
			{
				Parent = parent;
			}
			
			return this as TThis;
		}

		/// <summary>
		/// Spawns the actor into the given scene
		/// </summary>
		public Node Spawn(Scene scene = null) => Spawn<Node>(scene);
		public TThis Spawn<TThis>(Scene scene = null) where TThis : Node
		{
			if (scene == null)
			{
				Scene = Scene.Main;
			}
			else
			{
				Scene = scene;
			}

			if (parent == null)
			{
				Scene.Spawn(this);
			}

			return this as TThis;
		}

		public void UpdateTransform()
		{
			Matrix4 transform = Matrix4.CreateTransform(Position, Rotation, Scale);

			if (TransformHandle == null)
			{
				if (Scene != null)
				{
					TransformHandle = Scene.TransformBuffer.Allocate(1);
				}
				else
				{
					return;
				}
			}

			Renderer.DefaultCommandList.UploadBuffer(TransformHandle, new GPUTransform()
			{
				ObjectToWorld = transform,
				WorldToObject = transform.Inverse()
			});
		}

		public virtual void OnDrawGizmos(GizmosContext context) {}
	}
}