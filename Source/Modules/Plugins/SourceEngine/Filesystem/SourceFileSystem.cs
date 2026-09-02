using System;
using System.Diagnostics.CodeAnalysis;
using NFM.Resources;

namespace SourceEngine.Filesystem;

/// <summary>A mod directory and the mount its content is addressed through - "tf" under TF:/.</summary>
public sealed record ModMount(string Directory, MountPoint Mount);

/// <summary>A game's mods, searched in gameinfo override order, first match wins.</summary>
public sealed class SourceFileSystem
{
	public IReadOnlyList<ModMount> Mods { get; }

	public SourceFileSystem(IReadOnlyList<ModMount> mods)
	{
		Mods = mods;
	}

	/// <summary>Finds the highest-priority copy of a game-relative path.</summary>
	public bool TryResolve(string gamePath, [NotNullWhen(true)] out string diskPath)
	{
		return TryFind(gamePath, out _, out diskPath);
	}

	/// <summary>Resolves a game-relative path to the asset path its mod registered it under.</summary>
	public bool TryResolveAsset(string gamePath, [NotNullWhen(true)] out string assetPath)
	{
		if (TryFind(gamePath, out ModMount mod, out _))
		{
			assetPath = mod.Mount.MakeFullPath(Normalize(gamePath));
			return true;
		}

		assetPath = null;
		return false;
	}

	private bool TryFind(string gamePath, out ModMount mod, out string diskPath)
	{
		gamePath = Normalize(gamePath);

		foreach (ModMount candidate in Mods)
		{
			string path = Path.Combine(candidate.Directory, gamePath);
			if (File.Exists(path))
			{
				(mod, diskPath) = (candidate, path);
				return true;
			}
		}

		(mod, diskPath) = (null, null);
		return false;
	}

	/// <summary>Source paths arrive with either slash, stray leading separators and mixed case.</summary>
	public static string Normalize(string path)
	{
		path = path.Replace('\\', '/').Trim().ToLowerInvariant();

		while (path.Contains("//"))
		{
			path = path.Replace("//", "/");
		}

		return path.TrimStart('/');
	}
}
