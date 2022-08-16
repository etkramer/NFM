using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine.World
{
	public class Scene
	{
		[Notify, Save] public static Scene Main { get; set; }

		[Save] private ObservableCollection<Actor> actors { get; set; } = new();
		[Notify] public ReadOnlyObservableCollection<Actor> Actors { get; }

		public Scene()
		{
			Actors = new(actors);
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
	}
}
