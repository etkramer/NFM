using System.IO;
using System.Reflection;

namespace Engine.Common
{
	public static class Embed
	{
		public static string GetString(string path, Assembly assembly)
		{
			string resourceName = $"{assembly.GetName().Name}.{path.Replace('/', '.')}";

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
				{
					throw new FileNotFoundException($"Failed to locate content at \"{path}\" in assembly {assembly.GetName().Name}");
				}

				using (StreamReader reader = new(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}
}