using System;

namespace Engine.Resources
{
	public abstract class AssetLoader {}
	public abstract class AssetLoader<T> : AssetLoader where T : Resource
	{
		public abstract Task<T> Load();
		public virtual void Unload() {}
	}
}