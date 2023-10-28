using System.Diagnostics.CodeAnalysis;
using NFM.GPU;
using NFM.GPU.Native;
using NFM.Resources;
using NFM.World;

namespace NFM.Graphics;

class RenderMaterial : IDisposable
{
	public static TypedBuffer<byte> MaterialBuffer = new(Scene.MaxInstances * 64, isRaw: true);

	#region Permutations
	private static List<(IEnumerable<Shader>, int)> stackIDs = new();

	private static List<RenderMaterial> all = new();
	private static List<Type> requestedPermutationTypes = new();
	public static void RequestPermutation<T>() where T : ShaderPermutation, new()
	{
		lock (requestedPermutationTypes)
		lock (all)
		{
			if (!requestedPermutationTypes.Contains(typeof(T)))
			{
				requestedPermutationTypes.Add(typeof(T));

				foreach (var instance in all)
				{
					instance.permutations.Add(ShaderPermutation.FindOrCreate<T>(instance));
				}
			}
		}
	}

	public IEnumerable<ShaderPermutation> Permutations => permutations;
	private List<ShaderPermutation> permutations = new();

	#endregion

	[Inspect] public Material Source { get; set; }

	public ShaderParameter[] Parameters { get; }
	public ObservableCollection<Shader> Shaders { get; } = new();

	public BufferAllocation<byte> MaterialHandle { get; private set; }

	public int StackID { get; private set; }
	private static int lastID = 0;

	public RenderMaterial(Material baseMaterial)
	{
		Source = baseMaterial;
		Shaders.Add(Source.Shader);

		all.Add(this);

		// Calculate StackID
		var matchingStack = stackIDs.FirstOrDefault(o => o.Item1.SequenceEqual(Shaders));
		if (matchingStack.Item1 == null)
		{
			StackID = lastID++;
			stackIDs.Add((Shaders.ToArray(), StackID));
		}
		else
		{
			StackID = matchingStack.Item2;
		}

		// Build parameters table
		Parameters = Shaders.SelectMany(o => o.Parameters).ToArray();
		for (int i = 0; i < Parameters.Length; i++)
		{
			var materialOverride = Source.MaterialOverrides.FirstOrDefault(o => o.Name == Parameters[i].Name);
			if (materialOverride.Name != null)
			{
				Parameters[i].Value = materialOverride.Value;
			}
		}

		// Update material data
		UpdateMaterialData();

		// Create requested permutations
		lock (requestedPermutationTypes)
		{
			foreach (var type in requestedPermutationTypes)
			{
				permutations.Add(ShaderPermutation.FindOrCreate(type, this));
			}
		}
	}

    [MemberNotNull(nameof(MaterialHandle))]
	private void UpdateMaterialData()
	{
		MaterialHandle?.Dispose();
		List<byte> materialData = new();

		// Add shader ID to material data.
		materialData.AddRange(StructureToByteArray(typeof(int), StackID));

		// Loop through all shader parameters
		foreach (var param in Parameters)
		{
			// Grab the default value
			object? value = param.Value;

			// Is parameter overriden by material?
			if (Source.MaterialOverrides.TryFirst(o => o.Name == param.Name, out var overrideParam))
			{
				value = overrideParam.Value;
			}

			if (param.Type == typeof(bool) && value is bool boolValue)
			{
				// Interpret bools as integers due to size mismatch (8-bit in C#, 32-bit in HLSL)
				materialData.AddRange(StructureToByteArray(typeof(int), boolValue ? 1 : 0));
			}
			else if (param.Type == typeof(Texture2D) && value is Texture2D textureValue)
			{
				materialData.AddRange(StructureToByteArray(typeof(int), textureValue.D3DResource.GetSRV().GetDescriptorIndex()));
			}
			else
			{
				materialData.AddRange(StructureToByteArray(param.Type, Guard.NotNull(value)));
			}
		}

		Guard.Require(materialData.Count % 4 == 0, "The size of all material parameters must be divisible by 4.");

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
		all.Remove(this);
		
		MaterialHandle.Dispose();
	}
}