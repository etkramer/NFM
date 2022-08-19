using System;

namespace Engine.Resources
{
	public abstract class Resource
	{
		public Asset Source { get; internal set; } = null;

		public bool IsLoaded { get; internal set; }
		public virtual void OnLoad() {}
	}
}
