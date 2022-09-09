using System;
using System.IO;
using Engine.World;

namespace Engine
{
	public static class Project
	{
		[Notify] public static string Path { get; set; }
		[Notify("Path")] public static string Name => Path == null ? "Untitled" : System.IO.Path.GetFileNameWithoutExtension(Path);

		public static event Action OnProjectCreated = delegate {};

		public static void Create()
		{
			Scene.Main.Dispose();
			Scene.Main = new Scene();
			OnProjectCreated.Invoke();
		}

		public static void Load(string path)
		{
			Path = path;
		}

		public static void Save(string path)
		{
			Path = path ?? Path;
		}
	}
}