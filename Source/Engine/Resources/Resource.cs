using System;

namespace Engine.Resources
{
	public abstract class Resource
	{
		public bool IsLoaded { get; internal set; }
		public virtual void OnLoad() {}
	}
}
