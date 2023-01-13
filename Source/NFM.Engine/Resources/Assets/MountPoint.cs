using System;

namespace NFM.Resources
{
	public sealed class MountPoint
	{
		public static List<MountPoint> All = new();

		public readonly string Name;
		public readonly string ID;

		private MountPoint(string name, string id)
		{
			id = id.Trim().ToUpper();
			Debug.Assert(!string.IsNullOrWhiteSpace(id) && !id.Any(x => char.IsWhiteSpace(x)), "Mount ID must not contain whitespace!");

			Name = name;
			ID = id;
			All.Add(this);
		}

		public static MountPoint Create(string name, string id)
		{
			return new MountPoint(name, id);
		}

		public string MakeFullPath(string path)
		{
			path = path.Replace('\\', '/').Trim();
			return $"{ID}:/{path}";
		}
	}
}