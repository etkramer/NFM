using System;
using System.ComponentModel.DataAnnotations;
using Engine.Editor;
using Engine.GPU;
using Engine.Rendering;

namespace Engine.World
{
	[Icon('\uE3C2')]
	public class Actor : ISelectable, IDisposable
	{
		// Properties (inspectable)
		[Save, Inspect] public string Name { get; set; }
		[Save, Inspect] public Vector3 Position { get; set; } = Vector3.Zero;
		[Save, Inspect] public Vector3 Rotation { get; set; } = Vector3.Zero;
		[Save, Inspect] public Vector3 Scale { get; set; } = Vector3.One;
		
		public Scene Scene { get; private set; }
		public ReadOnlyObservableCollection<Actor> Children { get; }
		[Save, Notify] public Actor Parent
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
		private Actor parent;
		private readonly ObservableCollection<Actor> children = new();

		// Transform buffer
		public bool IsTransformDirty = true;
		public BufferAllocation<GPUTransform> TransformHandle;

		public Actor(string name = null)
		{
			Children = new(children);

			if (name == null)
			{
				name = GetType().Name.PascalToDisplay();

				if (name.EndsWith(" Actor"))
				{
					name = name.Remove(name.Length - 6);
				}
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
			// Remove self from scene tree.
			Scene?.Despawn(this);
			Parent = null;
			TransformHandle?.Dispose();

			// Dispose children.
			for (int i = Children.Count - 1; i >= 0; i--)
			{
				Children[i].Dispose();
			}

			// Make sure we're not still selected.
			Selection.Deselect(this);
		}

		/// <summary>
		/// Spawns the actor into the scene with the given parent
		/// </summary>
		public Actor Spawn(Actor parent) => Spawn<Actor>(parent);
		public TThis Spawn<TThis>(Actor parent) where TThis : Actor
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
		public Actor Spawn(Scene scene = null) => Spawn<Actor>(scene);
		public TThis Spawn<TThis>(Scene scene = null) where TThis : Actor
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
	}
}