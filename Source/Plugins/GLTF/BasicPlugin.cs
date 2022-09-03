using System;
using Basic.Loaders;
using Engine.Common;
using Engine.Resources;
using Engine.Plugins;
using Engine.Mathematics;

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
					string shortPath = Path.GetRelativePath(searchPath, fullPath).Split('.')[0];

					if (extension == ".glb")
					{
						Asset<Model> modelAsset = new Asset<Model>(shortPath, basicPrefix, new GLTFLoader(fullPath));
						Asset.Submit(modelAsset);
					}

					if (extension == ".hlsl")
					{
						Shader shader = LoadPBRShader(fullPath);
						Asset.Submit(new Asset<Shader>(shortPath, basicPrefix, shader));
					}
				}
			}
		}

		private Shader LoadPBRShader(string path)
		{
			string source = null;
			using (StreamReader reader = new StreamReader(path))
			{
				source = reader.ReadToEnd();
			}

			Shader shader = new Shader(source);
			shader.SetBlendMode(BlendMode.Opaque);
			shader.AddTexture("BaseColor", Texture2D.Purple);
			shader.AddTexture("Normal", Texture2D.Normal);

			return shader;
		}
	}
}