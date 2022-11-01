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

		public Scene Scene { get; }

		// Transform buffer
		public Node(Scene scene)
		{
			string name = GetType().Name.PascalToDisplay();
			if (name.EndsWith(" Node"))
			{
				name = name.Remove(name.Length - " Node".Length);
			}

			Name = name;
			Scene = scene ?? Scene.Main;
			Scene.AddNode(this);
		}

		public virtual void Dispose()
		{
			// Make sure we're not still selected.
			Selection.Deselect(this);

			// Remove self from scene tree.
			Scene?.RemoveNode(this);
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