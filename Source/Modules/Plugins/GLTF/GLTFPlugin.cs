using System;
using GLTF.Loaders;
using NFM;
using NFM.Common;
using NFM.Resources;
using NFM.Plugins;
using NFM.Mathematics;

namespace GLTF;

public class GLTFPlugin : Plugin
{
	public override void OnStart()
	{
		MountPoint mount = MountPoint.Create("User Content", "USER");

		string[] searchPaths = [FileUtils.GetContentPath()];

		foreach (var searchPath in searchPaths.Where(Directory.Exists))
		{
			foreach (var path in Directory.GetFiles(searchPath, "*", SearchOption.AllDirectories))
			{
				string fullPath = Path.GetFullPath(path);
				string extension = Path.GetExtension(fullPath);
				string shortPath = Path.GetRelativePath(searchPath, fullPath);

				if (extension == ".glb")
				{
					Asset<Model> modelAsset = new Asset<Model>(shortPath, mount, new GLTFLoader(fullPath));
					Asset.Submit(modelAsset);
				}
			}
		}

		LoadShaders(mount);
	}

	void LoadShaders(MountPoint mount)
	{
		// Create Opaque shader
		var opaque = new Shader(Embed.GetString("Shaders/Opaque.hlsl"))
        {
            BlendMode = BlendMode.Opaque
        };
		AddMaterialParams(opaque);

		// Create Transparent shader
		var transparent = new Shader(Embed.GetString("Shaders/Transparent.hlsl"))
        {
            BlendMode = BlendMode.Transparent
        };
		AddMaterialParams(transparent);

		//...and submit both.
		Asset.Submit(new Asset<Shader>("Shaders/Opaque.hlsl", mount, new CachedResourceLoader<Shader>(opaque)));
		Asset.Submit(new Asset<Shader>("Shaders/Transparent.hlsl", mount, new CachedResourceLoader<Shader>(transparent)));
	}
	static void AddMaterialParams(Shader shader)
	{
		shader.AddTextureParam("BaseColor", Texture2D.White);
		shader.AddTextureParam("Normal", Texture2D.Normal);
		shader.AddTextureParam("ORM", Texture2D.White);
		shader.AddTextureParam("Emissive", Texture2D.White);

		shader.AddColorParam("BaseColorFactor", Color.White);
		shader.AddColorParam("EmissiveFactor", Color.Black);
		shader.AddFloatParam("RoughnessFactor", 1);
		shader.AddFloatParam("MetallicFactor", 1);
	}
}