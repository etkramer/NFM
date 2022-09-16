using System;
using Basic.Loaders;
using Engine.Common;
using Engine.Resources;
using Engine.Plugins;
using Engine.Mathematics;
using System.Reflection;

namespace Basic
{
	public class BasicPlugin : Plugin
	{
		public override void OnStart()
		{
			string[] searchPaths = new[]
			{
				"../Content/"
			};

			MountPoint mount = MountPoint.Create("User Content", "USER");

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
			Shader opaque = new Shader(Embed.GetString("Shaders/Opaque.hlsl", typeof(BasicPlugin).Assembly));
			opaque.SetBlendMode(BlendMode.Opaque);
			opaque.AddTexture("BaseColor", Texture2D.Purple);
			opaque.AddTexture("Normal", Texture2D.Normal);
			opaque.AddTexture("ORM", Texture2D.FromColor(new Color(1, 0.5f, 0)));

			//...and submit.
			Asset.Submit(new Asset<Shader>("Shaders/Opaque.hlsl", mount, opaque));

			// Create Transparent shader
			Shader transparent = new Shader(Embed.GetString("Shaders/Transparent.hlsl", typeof(BasicPlugin).Assembly));
			transparent.SetBlendMode(BlendMode.Transparent);
			transparent.AddTexture("BaseColor", Texture2D.Purple);
			transparent.AddTexture("Normal", Texture2D.Normal);
			transparent.AddTexture("ORM", Texture2D.FromColor(new Color(1, 0.5f, 0)));

			//...and submit.
			Asset.Submit(new Asset<Shader>("Shaders/Transparent.hlsl", mount, transparent));
		}
	}
}