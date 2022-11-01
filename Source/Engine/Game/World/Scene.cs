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
			Nodes = new(nodes);
			All.Add(this);

			TransformBuffer.Name = "Transform Buffer";
			InstanceBuffer.Name = "Instance Buffer";
		}

		/// <summary>
		/// Adds a Node to the scene. Should ONLY be called from Node's constructor.
		/// </summary>
		internal void AddNode(Node node)
		{
			if (!nodes.Contains(node))
			{
				nodes.Add(node);
			}
		}

		/// <summary>
		/// Removes a Node from the scene, but does *not* dispose of it. Should ONLY be called from Node's Dispose() implementation.
		/// </summary>
		internal void RemoveNode(Node node)
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
