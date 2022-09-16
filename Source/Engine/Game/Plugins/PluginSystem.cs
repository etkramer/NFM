using System;
using System.Reflection;

namespace Engine.Plugins
{
	public static class PluginSystem
	{
		public static readonly List<Plugin> Plugins = new();

		public static async Task LoadAll()
		{
			string[] searchPaths = new[]
			{
				"../Plugins/"
			};

			// Load plugins from assemblies.
			foreach (string searchPath in searchPaths)
			{
				// Skip nonexistent search paths.
				if (!Directory.Exists(searchPath))
				{
					continue;
				}

				foreach (string fullPath in Directory.GetDirectories(searchPath, "*", SearchOption.TopDirectoryOnly))
				{
					string expectedName = Path.GetFileName(fullPath);
					string expectedPath = $"{fullPath}/{expectedName}.dll";

					// Check if the plugin .dll exists
					if (File.Exists(expectedPath))
					{
						// Load plugin assembly.
						Assembly assembly = Assembly.LoadFrom(expectedPath);

						foreach (Type type in assembly.GetTypes())
						{
							if (type.IsAssignableTo(typeof(Plugin)))
							{
								Plugin pluginInstance = (Plugin)Activator.CreateInstance(type);
								Plugins.Add(pluginInstance);
								break;
							}
						}
					}
				}
			}

			// Init loaded plugins.
			await Parallel.ForEachAsync(Plugins, async (o, ct) => o.OnStart());
		}
	}
}
