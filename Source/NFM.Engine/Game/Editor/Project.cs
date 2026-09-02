using NFM.Graphics;
using NFM.World;

namespace NFM;

public static class Project
{
	public static string? Path { get; set; }
	public static string Name => Path is null ? "Untitled" : System.IO.Path.GetFileNameWithoutExtension(Path);

	/// <summary>
	/// The project the editor boots into. Loaded like any other, but never written to.
	/// </summary>
	private static string StartupPath => System.IO.Path.Combine(FileUtils.GetBasePath(), "Config", "StartupProject.json");

	/// <summary>
	/// The view saved with the open project, if it had one.
	/// </summary>
	private static string? view;

	public static event Action OnProjectCreated = delegate {};

	public static void Reset()
	{
		Path = null;
		view = null;

		// Anything the stack was holding belongs to the scene on its way out.
		History.Clear();

		Scene.Main.Dispose();
		Scene.Main = new Scene();

		OnProjectCreated.Invoke();
	}

	/// <summary>
	/// Replaces the open project with the one saved at the given path. Assets referenced by the saved
	/// nodes are loaded along the way, so this only settles a while after the scene is swapped.
	/// </summary>
	public static async Task LoadAsync(string path)
	{
		if (Read(path) is not string json)
		{
			return;
		}

		Reset();
		Path = path;

		await Apply(json);
	}

	/// <summary>
	/// Opens the project the editor boots into, leaving it unnamed - saving it asks for a path like
	/// any other new project.
	/// </summary>
	public static async Task LoadStartupAsync()
	{
		Reset();

		if (File.Exists(StartupPath) && Read(StartupPath) is string json)
		{
			await Apply(json);
		}
	}

	/// <summary>
	/// Restores the open project's saved view onto a work camera, for cameras built after the load.
	/// </summary>
	public static Task ApplyView(Node camera)
	{
		return view is null ? Task.CompletedTask : NodeSerializer.ApplyView(view, camera);
	}

	private static string? Read(string path)
	{
		try
		{
			return File.ReadAllText(path);
		}
		catch (Exception e) when (e is IOException or UnauthorizedAccessException)
		{
			Log.Warn($"Couldn't open project: {e.Message}");
			return null;
		}
	}

	private static async Task Apply(string json)
	{
		view = await NodeSerializer.DeserializeProject(json, Scene.Main);

		foreach (Viewport viewport in Viewport.All)
		{
			await ApplyView(viewport.Camera);
		}
	}

	public static void Save(string path)
	{
		string json = NodeSerializer.SerializeProject(Scene.Main, Viewport.All.FirstOrDefault()?.Camera);

		try
		{
			File.WriteAllText(path, json);
		}
		catch (Exception e) when (e is IOException or UnauthorizedAccessException)
		{
			Log.Warn($"Couldn't save project: {e.Message}");
			return;
		}

		Path = path;
	}
}