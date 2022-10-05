using System;

namespace Engine.World
{
	public partial class Scene : IDisposable
	{
		public static List<Scene> All { get; } = new();

		[Notify, Save] public static Scene Main { get; set; } = new();

		[Notify] public ReadOnlyObservableCollection<Node> Nodes { get; }
		[Save] private ObservableCollection<Node> npdes { get; set; } = new();

		public Scene()
		{
			All.Add(this);
			Nodes = new(npdes);

			InstanceBuffer.Name = "Instance Buffer";
			TransformBuffer.Name = "Transform Buffer";
		}

		/// <summary>
		/// Adds a Node to the scene.
		/// </summary>
		public void Spawn(Node node)
		{
			if (!npdes.Contains(node))
			{
				npdes.Add(node);
			}
		}

		/// <summary>
		/// Removes a Node from the scene, but does *not* destroy it. Dispose() must still be called manually.
		/// </summary>
		public void Despawn(Node node)
		{
			npdes.Remove(node);
		}

		public void Dispose()
		{
			for (int i = npdes.Count - 1; i >= 0; i--)
			{
				npdes[i].Dispose();
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
