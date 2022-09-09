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

			AssetPrefix basicPrefix = AssetPrefix.Create("User Content", "USER");

			foreach (string searchPath in searchPaths)
			{
				foreach (string path in Directory.GetFiles(searchPath, "*", SearchOption.AllDirectories))
				{
					string fullPath = Path.GetFullPath(path);
					string extension = Path.GetExtension(fullPath);
					string shortPath = Path.GetRelativePath(searchPath, fullPath);

					if (extension == ".glb")
					{
						Asset<Model> modelAsset = new Asset<Model>(shortPath, basicPrefix, new GLTFLoader(fullPath));
						Asset.Submit(modelAsset);
					}
				}
			}

			LoadShaders(basicPrefix);
		}

		private void LoadShaders(AssetPrefix prefix)
		{
			string source = Embed.GetString("Shaders/Opaque.hlsl", typeof(BasicPlugin).Assembly);

			Shader shader = new Shader(source);
			shader.SetBlendMode(BlendMode.Opaque);
			shader.AddTexture("BaseColor", Texture2D.Purple);
			shader.AddTexture("Normal", Texture2D.Normal);
			shader.AddTexture("ORM", Texture2D.FromColor(new Color(1, 0.5f, 0)));

			Asset.Submit(new Asset<Shader>("Shaders/Opaque.hlsl", prefix, shader));
		}
	}
}