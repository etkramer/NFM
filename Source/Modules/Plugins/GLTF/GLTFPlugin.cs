using System;
using GLTF.Loaders;
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

		string[] searchPaths = new[]
		{
			"../Content/"
		};

		foreach (string searchPath in searchPaths)
		{
			foreach (string path in Directory.GetFiles(searchPath, "*", SearchOption.AllDirectories))
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

	private void LoadShaders(MountPoint mount)
	{
		// Create Opaque shader
		Shader opaque = new Shader(Embed.GetString("Shaders/Opaque.hlsl"));
		opaque.SetBlendMode(BlendMode.Opaque);
		opaque.AddTexture("BaseColor", Texture2D.Purple);
		opaque.AddTexture("Normal", Texture2D.Normal);
		opaque.AddTexture("ORM", Texture2D.FromColor(new Color(1, 0.5f, 0)));

		// Create Transparent shader
		Shader transparent = new Shader(Embed.GetString("Shaders/Transparent.hlsl"));
		transparent.SetBlendMode(BlendMode.Transparent);
		transparent.AddTexture("BaseColor", Texture2D.Purple);
		transparent.AddTexture("Normal", Texture2D.Normal);
		transparent.AddTexture("ORM", Texture2D.FromColor(new Color(1, 0.5f, 0)));

		//...and submit both.
		Asset.Submit(new Asset<Shader>("Shaders/Opaque.hlsl", mount, new CachedResourceLoader<Shader>(opaque)));
		Asset.Submit(new Asset<Shader>("Shaders/Transparent.hlsl", mount, new CachedResourceLoader<Shader>(transparent)));
	}
}