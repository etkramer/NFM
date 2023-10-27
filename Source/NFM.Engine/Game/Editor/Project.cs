using NFM.World;

namespace NFM;

public static class Project
{
	public static string? Path { get; set; }
	public static string Name => Path is null ? "Untitled" : System.IO.Path.GetFileNameWithoutExtension(Path);

	public static event Action OnProjectCreated = delegate {};

	public static void Reset()
	{
		Scene.Main.Dispose();
		Scene.Main = new Scene();

		OnProjectCreated.Invoke();
	}

	public static void Load(string path)
	{
		Path = path;
		Reset();
	}

	public static void Save(string path)
	{
		Path = path ?? Path;
	}
}