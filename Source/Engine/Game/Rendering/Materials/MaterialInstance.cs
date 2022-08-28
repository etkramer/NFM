using System;
using System.Linq;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class MaterialInstance : IDisposable
	{
		public static GraphicsBuffer<byte> MaterialBuffer = new(ModelActor.MaxInstanceCount * 64, 4, isRaw: true);

		[Inspect] public Material Material { get; set; }
		[Inspect] public ShaderStack ShaderStack { get; set; }

		public BufferHandle<byte> MaterialHandle = null;

		public MaterialInstance(Material baseMaterial)
		{
			Material = baseMaterial;
			ShaderStack = new ShaderStack(Material.Shader);

			UpdateMaterialData();

			// NOTE: Should start by sorting commands front-to-back before the depth prepass, just to prove sorting is working. Also, less overdraw is good.

			// Basic outline:
			// 1. Build and cull draw commands as per usual.
			// 2. Sort draw commands into buckets by shader. Do this by first storing the number of commands with a given shader in a "Shader Count" buffer
			//		basically "InterlockedAdd(ShaderCount[ShaderID], 1)", then use a full prefix sum (*not* WavePrefixSum) to get the value for the "Shader Start" buffer.
			// 3. Send one DrawIndirect call per shader, offsetting and clamping the dispatchThreadID by the start/length.
			// 4. Shaders can use their instance IDs to look themselves up in the material buffer, which contains all material
			// parameters (descriptor indexes and constants). Material buffer can be accessed at arbitrary offsets with ByteAddressBuffer.
		}

		private void UpdateMaterialData()
		{
			MaterialHandle?.Free();
			List<byte> materialData = new();

			// Add shader ID to material data.
			int shaderID = 1;
			materialData.AddRange(GetBytes(shaderID));

			// Loop through all shader parameters
			foreach (var param in ShaderStack.Parameters)
			{
				// Grab the default value
				object value = param.Value;

				// Is parameter overriden by material?
				if (Material.OverrideParameters.TryFirst(o => o.Name == param.Name, out var overrideParam))
				{
					value = overrideParam.Value;
				}

				if (param.Type == typeof(Color))
				{
					materialData.AddRange(GetBytes((Color)param.Value));
				}
			}

			Debug.Assert(materialData.Count % 4 == 0, "The size of all material parameters must be divisible by 4.");

			// Upload data to GPU.
			MaterialHandle = MaterialBuffer.Allocate(materialData.Count);
			MaterialBuffer.SetData(MaterialHandle, materialData.ToArray());
		}

		private unsafe byte[] GetBytes<T>(T data) where T : unmanaged
		{
			byte[] buffer = new byte[sizeof(T)];
			Marshal.Copy((IntPtr)(&data), buffer, 0, sizeof(T));
			return buffer;
		}

		public void Dispose()
		{
			MaterialHandle?.Free();
		}
	}
}