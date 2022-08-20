using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Engine.Aspects;
using Engine.Editor;

namespace Engine.World
{
	public class Actor : ISelectable, IDisposable
	{
		// Properties (inspectable)
		[Save, Inspect] public string Name { get; set; }
		[Save, Inspect] public Vector3 Position { get; set; } = Vector3.Zero;
		[Save, Inspect] public Vector3 Rotation { get; set; } = Vector3.Zero;
		[Save, Inspect] public Vector3 Scale { get; set; } = Vector3.One;
		
		// Properties (code-only)
		[Save, Notify] public Actor Parent { get => parent; set { parent?.children.Remove(this); value?.children.Add(this); parent = value; }}
		public Scene Scene { get; private set; }
		public ReadOnlyObservableCollection<Actor> Children { get; }

		// Fields
		private Actor parent;
		private readonly ObservableCollection<Actor> children = new();

		public Actor(string name = null, Actor parent = null)
		{
			Children = new(children);

			if (name == null)
			{
				name = GetType().Name.PascalToDisplay();

				if (name.EndsWith(" Actor"))
					name = name.Remove(name.Length - 6);
			}

			Name = name.Trim();
			Parent = parent;
		}

		public virtual void Dispose()
		{
			// Remove self from scene tree.
			Scene?.Remove(this);
			Parent = null;

			// Dispose children.
			for (int i = Children.Count - 1; i >= 0; i--)
				Children[i].Dispose();

			// Make sure we're not still selected.
			Selection.Deselect(this);
		}

		/// <summary>
		/// Spawn the actor into the world
		/// </summary>
		public Actor Spawn(Scene scene = null) => Spawn<Actor>(scene);
		public TThis Spawn<TThis>(Scene scene = null) where TThis : Actor
		{
			if (scene == null)
			{
				Scene = Scene.Main;
			}

			if (parent == null)
			{
				Scene.Add(this);
			}

			return this as TThis;
		}

		string ISelectable.GetName() => Name;
	}
}