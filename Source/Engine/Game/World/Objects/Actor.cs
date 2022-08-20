using System;
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
		
		public Scene Scene { get; private set; }
		public ReadOnlyObservableCollection<Actor> Children { get; }
		[Save, Notify] public Actor Parent
		{
			get => parent;
			set
			{
				if (parent != value)
				{
					// Remove from old parent.
					Scene?.Remove(this);
					parent?.children.Remove(this);

					// Add to new parent.
					parent = value;
					parent?.children.Add(this);	
					if (parent == null)
						Scene?.Add(this);
				}
			}
		}

		// Backing fields
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
			{
				Children[i].Dispose();
			}

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