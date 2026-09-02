using System;
using NFM;
using NFM.Common;
using NFM.Mathematics;
using NFM.Resources;
using SourceEngine.Filesystem;
using SourceEngine.Formats;

namespace SourceEngine.Loaders;

/// <summary>Loads a VMT - params pass through raw, the shader interprets them.</summary>
public class VmtLoader : ResourceLoader<Material>
{
	private readonly SourceFileSystem fileSystem;

	public string Path;

	public VmtLoader(SourceFileSystem fileSystem, string path)
	{
		this.fileSystem = fileSystem;
		Path = path;
	}

	public override async Task<Material> Load()
	{
		VmtFile vmt = VmtFile.Read(fileSystem, Path);

		// TODO: $alphatest wants a masked mode, which the visbuffer can't discard for yet.
		BlendMode blendMode = vmt.GetBool("$additive") ? BlendMode.Additive
			: vmt.GetBool("$translucent") ? BlendMode.Transparent
			: BlendMode.Opaque;

		bool isTransparent = blendMode != BlendMode.Opaque;

		// $nocull only reaches blended geometry; opaque uses one scene-wide cull mode.
		FaceMode faceMode = vmt.GetBool("$nocull") && isTransparent ? FaceMode.TwoSided : FaceMode.FrontOnly;

		bool isUnlit = vmt.Shader is "UnlitGeneric" or "UnlitTwoTexture" or "Sprite";
		string shaderPath = SourceEnginePlugin.ShaderPath(isUnlit ? "UnlitGeneric" : "VertexLitGeneric", blendMode, faceMode);

		Task<Shader> shader = Asset.LoadAsync<Shader>($"{SourceEnginePlugin.ShaderMount}:/{shaderPath}");
		Task<Texture2D> baseTask = LoadTexture(vmt.GetString("$basetexture"));

		if (isUnlit)
		{
			Material unlit = new(await shader);

			unlit.SetTexture("BaseTexture", await baseTask);
			unlit.SetColor("Color", vmt.GetColor("$color2", vmt.GetColor("$color", Color.White)));

			return unlit;
		}

		Task<Texture2D> bumpTask = LoadTexture(vmt.GetString("$bumpmap"));
		Task<Texture2D> exponentTask = LoadTexture(vmt.GetString("$phongexponenttexture"));

		Material material = new(await shader);

		Texture2D baseTexture = await baseTask;
		Texture2D bumpMap = await bumpTask;

		material.SetTexture("BaseTexture", baseTexture);
		material.SetTexture("BumpMap", bumpMap);
		material.SetTexture("ExponentTexture", await exponentTask);

		material.SetColor("Color", vmt.GetColor("$color2", vmt.GetColor("$color", Color.White)));
		material.SetColor("SelfIllumTint", vmt.GetBool("$selfillum") ? vmt.GetColor("$selfillumtint", Color.White) : Color.Black);

		if (vmt.GetBool("$phong"))
		{
			material.SetFloat("PhongExponent", vmt.GetFloat("$phongexponent", 5));
			material.SetFloat("PhongBoost", MathF.Max(vmt.GetFloat("$phongboost", 1), 0));
			material.SetInt("PhongMaskSource", PickPhongMask(vmt, baseTexture, bumpMap, isTransparent));
		}

		return material;
	}

	/// <summary>Picks the phong mask source - $basetexture alpha if flagged, $bumpmap alpha, else none.</summary>
	private static int PickPhongMask(VmtFile vmt, Texture2D baseTexture, Texture2D bumpMap, bool isTransparent)
	{
		// A translucent material's base alpha is its opacity, so only the bump map can mask it.
		if (vmt.GetBool("$basemapalphaphongmask") && baseTexture is not null && !isTransparent)
		{
			return 1;
		}

		return bumpMap is not null ? 2 : 0;
	}

	/// <summary>Resolves a texture reference to its registered asset.</summary>
	private async Task<Texture2D> LoadTexture(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return null;
		}

		string gamePath = $"materials/{SourceFileSystem.Normalize(name)}.vtf";

		if (!fileSystem.TryResolveAsset(gamePath, out string assetPath))
		{
			Log.Warn($"{Path} references missing texture '{name}'");
			return null;
		}

		return await Asset.LoadAsync<Texture2D>(assetPath);
	}
}
