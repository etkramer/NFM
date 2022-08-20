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

		// Fields.
		private Actor parent;
		private readonly ObservableCollection<Actor> children = new();

		public Actor(string name = null, Actor parent = null, Scene scene = null)
		{
			Children = new(children);

			if (name == null)
			{
				name = GetType().Name.PascalToDisplay();

				if (name.EndsWith(" Actor"))
					name = name.Remove(name.Length - 6);
			}

			if (scene == null)
			{
				scene = Scene.Main;
			}

			if (parent == null)
			{
				scene.Add(this);
			}

			Name = name.Trim();
			Scene = scene;
			Parent = parent;
		}

		string ISelectable.GetName() => Name;

		public virtual void Dispose()
		{
			Scene.Remove(this);
		}
	}
}