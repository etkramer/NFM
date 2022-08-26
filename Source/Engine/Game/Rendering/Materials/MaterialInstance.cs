using System;
using System.Linq;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;

namespace Engine.Rendering
{
	public class MaterialInstance : IDisposable
	{
		[Inspect] public Material BaseMaterial { get; set; }
		public Shader BaseShader => BaseMaterial.Shader;

		public MaterialInstance(Material baseMaterial)
		{
			BaseMaterial = baseMaterial;

			// NOTE: Should start by sorting commands front-to-back before the depth prepass, just to prove sorting is working. Also, less overdraw is good.

			// Basic outline:
			// 1. Build and cull draw commands as per usual.
			// 2. Sort draw commands into buckets by shader. Do this by first storing the number of commands with a given shader in a "Shader Count" buffer
			//		basically "InterlockedAdd(ShaderCount[ShaderID], 1)", then use a full prefix sum (*not* WavePrefixSum) to get the value for the "Shader Start" buffer.
			// 3. Send one DrawIndirect call per shader, offsetting and clamping the dispatchThreadID by the start/length.
			// 4. Shaders can use their instance IDs to look themselves up in the material buffer, which contains all material
			// parameters (descriptor indexes and constants). Material buffer can be accessed at arbitrary offsets with ByteAddressBuffer.
		}

		public void Dispose()
		{

		}
	}
}