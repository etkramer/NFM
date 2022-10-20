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

		private Scene scene;
		public Scene Scene
		{
			get => scene;
			set
			{
				if (scene != value)
				{
					scene?.Despawn(this);
					scene = value;
					scene?.Spawn(this);
				}
			}
		}

		// Transform buffer
		public bool IsTransformDirty = true;
		public BufferAllocation<GPUTransform> TransformHandle;

		public Node()
		{
			string name = GetType().Name.PascalToDisplay();
			if (name.EndsWith(" Node"))
			{
				name = name.Remove(name.Length - " Node".Length);
			}

			Name = name;

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
			TransformHandle?.Dispose();
		}

		/// <summary>
		/// Spawns the actor into the given scene
		/// </summary>
		public Node Spawn(Scene scene = null)
		{
			Scene = scene ?? Scene.Main;
			return this;
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