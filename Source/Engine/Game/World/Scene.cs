using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine.World
{
	public partial class Scene : IDisposable
	{
		private static List<Scene> all { get; } = new();
		public static ReadOnlyCollection<Scene> All { get; }

		[Notify, Save] public static Scene Main { get; set; } = new();

		[Save] private ObservableCollection<Actor> actors { get; set; } = new();
		[Notify] public ReadOnlyObservableCollection<Actor> Actors { get; }

		static Scene()
		{
			All = new ReadOnlyCollection<Scene>(all);
		}

		public Scene()
		{
			Actors = new(actors);
			all.Add(this);
		}

		public void Add(Actor actor)
		{
			if (!actors.Contains(actor))
			{
				actors.Add(actor);
			}
		}

		public void Remove(Actor actor)
		{
			actors.Remove(actor);
		}

		public void Dispose()
		{
			for (int i = actors.Count - 1; i >= 0; i--)
			{
				actors[i].Dispose();
			}

			all.Remove(this);
		}
	}
}
