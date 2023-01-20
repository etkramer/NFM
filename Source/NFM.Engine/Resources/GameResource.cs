using System;

namespace NFM.Resources;

public abstract class GameResource : IDisposable
{
	internal Asset Source { get; set; } = null;

	public virtual void Dispose() {}
}
