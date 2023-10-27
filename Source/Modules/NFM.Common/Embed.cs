using System.IO;
using System.Reflection;

namespace NFM.Common
{
	public static class Embed
	{
		public static string GetString(string path, Assembly? assembly = null)
		{
			if (assembly == null)
			{
				assembly = Assembly.GetCallingAssembly();
			}

			string resourceName = $"{assembly.GetName().Name}.{path.Replace('/', '.')}";

			using Stream? stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException($"Failed to locate content at \"{path}\" in assembly {assembly.GetName().Name}");
            using StreamReader reader = new(stream);
			return reader.ReadToEnd();
		}
	}
}