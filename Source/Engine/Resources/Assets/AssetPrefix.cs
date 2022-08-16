using System;

namespace Engine.Resources
{
	public class AssetPrefix
	{
		public readonly string Name;
		public readonly string ID;

		private AssetPrefix(string name, string id)
		{
			Name = name;
			ID = id.Trim().ToUpper();
		}

		public static AssetPrefix Create(string name, string id)
		{
			return new AssetPrefix(name, id);
		}

		public string MakeFullPath(string path)
		{
			path = path.Replace('\\', '/').Trim();
			return $"{ID}:{path}";
		}
	}
}
