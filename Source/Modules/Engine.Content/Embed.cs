using System.IO;
using System.Reflection;

namespace Engine.Content
{
	public static class Embed
	{
		public static string GetString(string path, Assembly assembly = null)
		{
			if (assembly == null)
			{
				assembly = Assembly.GetExecutingAssembly();
			}

			path = $"{assembly.GetName().Name}.{path.Replace('/', '.')}";

			using (Stream stream = assembly.GetManifestResourceStream(path))
			{
				using (StreamReader reader = new(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}
}