using System;
using System.Text.Json;
using NFM;
using NFM.Common;
using NFM.Mathematics;
using NFM.Plugins;
using NFM.Resources;
using SourceEngine.Filesystem;
using SourceEngine.Loaders;

namespace SourceEngine;

public class SourceEnginePlugin : Plugin
{
	/// <summary>A Source unit is an inch, NFM works in metres.</summary>
	public const float UnitScale = 0.0254f;

	/// <summary>Mount holding the surface shaders every game's materials are built on.</summary>
	public const string ShaderMount = "SRC";

	public override void OnStart()
	{
		MountPoint shaderMount = MountPoint.Create("Source Engine", ShaderMount);
		LoadShaders(shaderMount);

		foreach (string gameDir in ReadGameDirs())
		{
			try
			{
				MountGame(gameDir);
			}
			catch (Exception e)
			{
				Log.Error($"Couldn't mount Source game at {gameDir} - {e.Message}");
			}
		}
	}

	// Games share mods - every TF2-era game mounts hl2 - so a mod is only ever mounted once.
	private readonly Dictionary<string, ModMount> mods = new(StringComparer.OrdinalIgnoreCase);

	private void MountGame(string gameDir)
	{
		GameInfo gameInfo = GameInfo.Read(gameDir);

		List<ModMount> searchOrder = [];
		List<ModMount> fresh = [];

		foreach (string directory in gameInfo.SearchPaths)
		{
			if (!mods.TryGetValue(directory, out ModMount mod))
			{
				string name = Path.GetFileName(directory);
				mod = new ModMount(directory, MountPoint.Create(name, MakeMountID(name)));

				mods.Add(directory, mod);
				fresh.Add(mod);
			}

			searchOrder.Add(mod);
		}

		SourceFileSystem fileSystem = new(searchOrder);
		Log.Info($"Mounting {gameInfo.Title} ({string.Join(", ", searchOrder.Select(o => $"{o.Mount.ID}:/"))})");

		foreach (ModMount mod in fresh)
		{
			Log.Info($"Registered {Register(fileSystem, mod)} assets under {mod.Mount.ID}:/");
		}
	}

	private static int Register(SourceFileSystem fileSystem, ModMount mod)
	{
		int count = 0;

		foreach (string fullPath in Directory.EnumerateFiles(mod.Directory, "*", SearchOption.AllDirectories))
		{
			string gamePath = SourceFileSystem.Normalize(Path.GetRelativePath(mod.Directory, fullPath));

			count += Path.GetExtension(gamePath) switch
			{
				".vtf" => Asset.Submit(new Asset<Texture2D>(gamePath, mod.Mount, new VtfLoader(fullPath))) ? 1 : 0,
				".vmt" => Asset.Submit(new Asset<Material>(gamePath, mod.Mount, new VmtLoader(fileSystem, fullPath))) ? 1 : 0,

				// Skip unsupported or incomplete models.
				".mdl" when MdlLoader.IsSupported(fullPath)
					=> Asset.Submit(new Asset<Model>(gamePath, mod.Mount, new MdlLoader(fileSystem, fullPath))) ? 1 : 0,

				_ => 0
			};
		}

		return count;
	}

	private IEnumerable<string> ReadGameDirs()
	{
		if (!TryLoadConfig(out JsonDocument document) || document is null)
		{
			Log.Warn("No SourceEnginePlugin.json - no Source games mounted");
			yield break;
		}

		using (document)
		{
			if (!document.RootElement.TryGetProperty("Games", out JsonElement games))
			{
				yield break;
			}

			foreach (JsonElement game in games.EnumerateArray())
			{
				if (game.TryGetProperty("Path", out JsonElement path) && path.GetString() is string value)
				{
					yield return value;
				}
			}
		}
	}

	// Mount IDs address assets, so they can't carry whitespace or a path separator.
	private static string MakeMountID(string modName)
	{
		string id = new(modName.Where(char.IsLetterOrDigit).ToArray());
		return string.IsNullOrEmpty(id) ? "SOURCE" : id.ToUpperInvariant();
	}

	private void LoadShaders(MountPoint mount)
	{
		Submit("Shaders/VertexLitGeneric.hlsl", BlendMode.Opaque);
		Submit("Shaders/VertexLitGenericTransparent.hlsl", BlendMode.Transparent);

		void Submit(string path, BlendMode blendMode)
		{
			Shader shader = new(Embed.GetString(path))
			{
				BlendMode = blendMode
			};

			shader.AddTextureParam("BaseTexture", Texture2D.White);
			shader.AddTextureParam("BumpMap", Texture2D.Normal);
			shader.AddTextureParam("ExponentTexture", Texture2D.Black);

			shader.AddColorParam("Color", Color.White);
			shader.AddColorParam("SelfIllumTint", Color.Black);
			shader.AddFloatParam("PhongExponent", 0);
			shader.AddFloatParam("PhongBoost", 0);
			shader.AddIntParam("PhongMaskSource", 0);

			Asset.Submit(new Asset<Shader>(path, mount, new CachedResourceLoader<Shader>(shader)));
		}
	}
}
