using System;
using System.Reflection;

namespace Engine.Plugins
{
	public static class PluginSystem
	{
		public static readonly List<Plugin> Plugins = new();

		public static void LoadAll()
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
					string expectedAssemblyName = Path.GetFileName(fullPath);
					string expectedAssemblyPath = $"{fullPath}/{expectedAssemblyName}.dll";

					if (File.Exists(expectedAssemblyPath))
					{
						// Load plugin assembly.
						Assembly assembly = Assembly.LoadFrom(expectedAssemblyPath);

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
			Parallel.ForEach(Plugins, (o, e) => o.OnStart());
		}
	}
}
