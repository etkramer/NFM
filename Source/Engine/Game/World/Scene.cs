using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Engine.GPU;
using Engine.Rendering;

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
			All.Add(this);
			Actors = new(actors);

			InstanceBuffer.Name = "Instance Buffer";
			TransformBuffer.Name = "Transform Buffer";
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

			Queue.Add(() =>
			{
				InstanceBuffer.Dispose();
				TransformBuffer.Dispose();
			}, 0);

			All.Remove(this);
		}
	}
}
