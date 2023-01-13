using System;

namespace NFM.Resources
{
	public abstract class Resource : IDisposable
	{
		internal Asset Source { get; set; } = null;

		public virtual void Dispose() {}
	}
}
