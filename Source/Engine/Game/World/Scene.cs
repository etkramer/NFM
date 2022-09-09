using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine.World
{
	public partial class Scene : IDisposable
	{
		public static List<Scene> All { get; } = new();

		[Notify, Save] public static Scene Main { get; set; } = new();

		[Notify] public ReadOnlyObservableCollection<Actor> Actors { get; }
		[Save] private ObservableCollection<Actor> actors { get; set; } = new();

		public Scene()
		{
			Actors = new(actors);
			All.Add(this);
		}

		/// <summary>
		/// Adds an Actor to the scene.
		/// </summary>
		public void Spawn(Actor actor)
		{
			if (!actors.Contains(actor))
			{
				actors.Add(actor);
			}
		}

		/// <summary>
		/// Removes an Actor from the scene, but does *not* destroy it. Dispose() must still be called manually.
		/// </summary>
		public void Despawn(Actor actor)
		{
			actors.Remove(actor);
		}

		public void Dispose()
		{
			for (int i = actors.Count - 1; i >= 0; i--)
			{
				actors[i].Dispose();
			}

			All.Remove(this);
		}
	}
}
