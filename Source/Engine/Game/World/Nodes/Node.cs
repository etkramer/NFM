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
		public Node()
		{
			string name = GetType().Name.PascalToDisplay();
			if (name.EndsWith(" Node"))
			{
				name = name.Remove(name.Length - " Node".Length);
			}

			Name = name;
		}

		public virtual void Dispose()
		{
			// Make sure we're not still selected.
			Selection.Deselect(this);

			// Remove self from scene tree.
			Scene?.Despawn(this);
		}

		/// <summary>
		/// Spawns the actor into the given scene
		/// </summary>
		public Node Spawn(Scene scene = null)
		{
			Scene = scene ?? Scene.Main;
			return this;
		}

		public virtual void OnDrawGizmos(GizmosContext context) {}
	}
}