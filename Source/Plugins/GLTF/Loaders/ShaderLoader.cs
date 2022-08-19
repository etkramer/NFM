using System;
using Engine.Resources;

namespace Basic.Loaders
{
	public class ShaderLoader : AssetLoader<Shader>
	{
		public string Path;

		public ShaderLoader(string path)
		{
			Path = path;
		}

		public override async Task<Shader> Load()
		{
			string source;

			// Read shader source from path.
			using (StreamReader reader = new StreamReader(Path))
			{
				source = reader.ReadToEnd();
			}

			// Create and return shader object
			return new Shader()
			{
				ShaderSource = source
			};
		}
	}
}
