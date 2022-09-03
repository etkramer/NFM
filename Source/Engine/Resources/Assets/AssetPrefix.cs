using System;

namespace Engine.Resources
{
	public sealed class AssetPrefix
	{
		public static List<AssetPrefix> All = new();

		public readonly string Name;
		public readonly string ID;

		private AssetPrefix(string name, string id)
		{
			id = id.Trim().ToUpper();
			Debug.Assert(!string.IsNullOrWhiteSpace(id) && !id.Any(x => char.IsWhiteSpace(x)), "Prefix ID must not contain whitespace!");

			Name = name;
			ID = id;
			All.Add(this);
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