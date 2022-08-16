using System;
using Basic.Loaders;
using Engine.Core;
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
					string shortName = Path.GetFileNameWithoutExtension(fullPath);
					string shortPath = Path.GetRelativePath(searchPath, fullPath).Split('.')[0];

					if (extension == ".glb")
					{
						Asset<Model> modelAsset = new Asset<Model>(shortPath, basicPrefix, new GLTFLoader(fullPath));
						Asset.Submit(modelAsset);
					}

					if (extension == ".hlsl")
					{
						Asset<Shader> shaderAsset = new Asset<Shader>(shortPath, basicPrefix, new ShaderLoader(fullPath));
						Asset.Submit(shaderAsset);
					}
				}
			}
		}
	}
}