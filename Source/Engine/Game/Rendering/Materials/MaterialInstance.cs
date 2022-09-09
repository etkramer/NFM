using System;
using System.Linq;
using Engine.Common;
using Engine.GPU;
using Engine.GPU.Native;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class MaterialInstance : IDisposable
	{
		public static GraphicsBuffer<byte> MaterialBuffer = new(ModelActor.MaxInstanceCount * 64, 4, isRaw: true);

		[Inspect] public Material Material { get; set; }
		[Inspect] public ShaderStack ShaderStack { get; set; }

		public BufferAllocation<byte> MaterialHandle = null;

		public MaterialInstance(Material baseMaterial)
		{
			Material = baseMaterial;
			ShaderStack = new ShaderStack(Material.Shader);

			UpdateMaterialData();
		}

		private void UpdateMaterialData()
		{
			MaterialHandle?.Dispose();
			List<byte> materialData = new();

			// Add shader ID to material data.
			materialData.AddRange(StructureToByteArray(typeof(int), ShaderStack.ProgramID));

			// Loop through all shader parameters
			foreach (var param in ShaderStack.Parameters)
			{
				// Grab the default value
				object value = param.Value;

				// Is parameter overriden by material?
				if (Material.MaterialOverrides.TryFirst(o => o.Name == param.Name, out var overrideParam))
				{
					value = overrideParam.Value;
				}

				if (param.Type == typeof(bool))
				{
					// Interpret bools as integers due to size mismatch (8-bit in C#, 32-bit in HLSL)
					materialData.AddRange(StructureToByteArray(typeof(int), (bool)value ? 1 : 0));
				}
				else if (param.Type == typeof(Texture2D))
				{
					materialData.AddRange(StructureToByteArray(typeof(int), (value as Texture2D).Resource.GetSRV().GetDescriptorIndex()));
				}
				else
				{
					materialData.AddRange(StructureToByteArray(param.Type, value));
				}
			}

			Debug.Assert(materialData.Count % 4 == 0, "The size of all material parameters must be divisible by 4.");

			// Upload data to GPU.
			MaterialHandle = MaterialBuffer.Allocate(materialData.Count);
			Renderer.DefaultCommandList.UploadBuffer(MaterialHandle, materialData.ToArray());
		}

		private byte[] StructureToByteArray(Type type, object data)
		{
			int dataSize = Marshal.SizeOf(type);

			IntPtr bufferptr = Marshal.AllocHGlobal(dataSize);
			Marshal.StructureToPtr(data, bufferptr, false);
			byte[] buffer = new byte[dataSize];
			Marshal.Copy(bufferptr, buffer, 0, dataSize);
			Marshal.FreeHGlobal(bufferptr);

			return buffer;
		}

		public void Dispose()
		{
			MaterialHandle?.Dispose();
		}
	}
}