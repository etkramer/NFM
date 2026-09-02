using System;
using NFM;
using NFM.Common;
using ValveKeyValue;

namespace SourceEngine.Filesystem;

/// <summary>A game's gameinfo.txt, reduced to the ordered content search paths it declares.</summary>
public sealed class GameInfo
{
	public string Title { get; private init; }

	/// <summary>Absolute content directories in override order - the first one holding a file wins.</summary>
	public IReadOnlyList<string> SearchPaths { get; private init; }

	public static GameInfo Read(string gameDir)
	{
		gameDir = Path.GetFullPath(gameDir);
		string path = Path.Combine(gameDir, "gameinfo.txt");

		if (!File.Exists(path))
		{
			throw new FileNotFoundException($"No gameinfo.txt in {gameDir}");
		}

		KVObject root;
		using (var stream = File.OpenRead(path))
		{
			root = KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Deserialize(stream);
		}

		return new GameInfo()
		{
			Title = root["game"]?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? Path.GetFileName(gameDir),
			SearchPaths = ReadSearchPaths(root, gameDir)
		};
	}

	private static List<string> ReadSearchPaths(KVObject root, string gameDir)
	{
		// Everything's addressed relative to the directory holding hl2.exe.
		string baseDir = Path.GetFullPath(Path.Combine(gameDir, ".."));

		List<string> paths = [];
		KVObject searchPaths = FindChild(FindChild(root, "FileSystem"), "SearchPaths");

		foreach (KVObject entry in searchPaths?.Children ?? Enumerable.Empty<KVObject>())
		{
			// "Game", "Game+Mod", "mod" - anything that mounts content, as opposed to bins or write paths.
			if (!entry.Name.Contains("game", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			string token = entry.Value.ToString(System.Globalization.CultureInfo.InvariantCulture).Trim();
			if (!TryExpand(token, gameDir, baseDir, out string expanded))
			{
				continue;
			}

			if (Directory.Exists(expanded) && !paths.Contains(expanded, StringComparer.OrdinalIgnoreCase))
			{
				paths.Add(expanded);
			}
		}

		// A gameinfo with no usable Game path would mount nothing at all.
		if (paths.Count == 0)
		{
			paths.Add(gameDir);
		}

		return paths;
	}

	private static KVObject FindChild(KVObject parent, string name)
	{
		return parent?.Children.FirstOrDefault(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
	}

	private static bool TryExpand(string token, string gameDir, string baseDir, out string expanded)
	{
		expanded = null;

		token = token.Replace('\\', '/');
		token = token.Replace("|gameinfo_path|", gameDir + "/");
		token = token.Replace("|all_source_engine_paths|", baseDir + "/");

		if (token.EndsWith(".vpk", StringComparison.OrdinalIgnoreCase))
		{
			Log.Warn($"Skipping VPK search path '{token}' - packages aren't mounted yet");
			return false;
		}

		expanded = Path.GetFullPath(Path.IsPathRooted(token) ? token : Path.Combine(baseDir, token));
		return true;
	}
}
