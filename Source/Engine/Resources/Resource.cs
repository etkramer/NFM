using System;

namespace Engine.Resources
{
	public abstract class Resource : IDisposable
	{
		public Asset Source { get; internal set; } = null;

		public bool IsLoaded { get; internal set; }

		public virtual void OnLoad() {}
		public virtual void Dispose() {}
	}
}
