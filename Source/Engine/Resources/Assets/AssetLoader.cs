using System;

namespace Engine.Resources
{
	/// <summary>
	/// Describes a loader used when an unloaded asset is requested
	/// </summary>
	public abstract class AssetLoader {}

	/// <inheritdoc/>
	public abstract class AssetLoader<T> : AssetLoader where T : Resource
	{
		public abstract Task<T> Load();
		public virtual void Unload() {}
	}
}