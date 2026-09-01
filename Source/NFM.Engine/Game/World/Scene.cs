using NFM.Graphics;

namespace NFM.World;

public partial class Scene : IDisposable
{
	public static List<Scene> All { get; } = [];

	[Notify] public static Scene Main { get; set; } = new();

	[Notify] public IEnumerable<Node> RootNodes => rootNodes;
	private ObservableCollection<Node> rootNodes { get; set; } = [];

	internal RenderScene RenderData { get; } = new();

	public Scene()
	{
		All.Add(this);
	}

	/// <summary>
	/// Adds a Node as a scene root. Should NEVER be called manually.
	/// </summary>
	internal void AddRootNode(Node node)
	{
		rootNodes.Add(node);
	}

	/// <summary>
	/// Removes a Node as a scene root. Should NEVER be called manually.
	/// </summary>
	internal bool RemoveRootNode(Node node)
	{
		return rootNodes.Remove(node);
	}

	public void Dispose()
	{
		for (int i = rootNodes.Count - 1; i >= 0; i--)
		{
			rootNodes[i].Dispose();
		}

		RenderData.Dispose();

		All.Remove(this);
	}
}
