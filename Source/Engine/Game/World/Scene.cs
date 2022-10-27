using System;

namespace Engine.World
{
	public partial class Scene : IDisposable
	{
		public static List<Scene> All { get; } = new();

		[Notify, Save] public static Scene Main { get; set; } = new();

		[Notify] public ReadOnlyObservableCollection<Node> Nodes { get; }
		[Save] private ObservableCollection<Node> nodes { get; set; } = new();

		public Scene()
		{
			All.Add(this);
			Nodes = new(nodes);

			TransformBuffer.Name = "Transform Buffer";
			InstanceBuffer.Name = "Instance Buffer";
		}

		/// <summary>
		/// Adds a Node to the scene.
		/// </summary>
		public void Spawn(Node node)
		{
			if (!nodes.Contains(node))
			{
				nodes.Add(node);
			}
		}

		/// <summary>
		/// Removes a Node from the scene, but does *not* destroy it. Dispose() must still be called manually.
		/// </summary>
		public void Despawn(Node node)
		{
			nodes.Remove(node);
		}

		public void Dispose()
		{
			for (int i = nodes.Count - 1; i >= 0; i--)
			{
				nodes[i].Dispose();
			}

			DispatchQueue.Add(() =>
			{
				InstanceBuffer.Dispose();
				TransformBuffer.Dispose();
			}, 0);

			All.Remove(this);
		}
	}
}
