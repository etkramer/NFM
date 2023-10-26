using System.Reflection;

namespace NFM.Plugins;

public static class PluginSystem
{
	public static readonly List<Plugin> Plugins = new();

	public static void Init()
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
					ReflectionHelper.RegisterAssembly(assembly);

					// Find the plugin type...
					var pluginType = assembly.GetTypes().FirstOrDefault(o => o.IsAssignableTo(typeof(Plugin)));
					if (pluginType != null)
					{
						var pluginInstance = (Plugin)Activator.CreateInstance(pluginType);
						Plugins.Add(pluginInstance);
					}
				}
			}
		}

		// Start up loaded plugins.
		Parallel.ForEach(Plugins, (o) => o.OnStart());
	}
}
