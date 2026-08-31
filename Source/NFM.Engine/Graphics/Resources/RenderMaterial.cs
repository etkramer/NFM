using System.Diagnostics.CodeAnalysis;
using NFM.GPU;
using NFM.GPU.Native;
using NFM.Resources;

namespace NFM.Graphics;

class RenderMaterial : IDisposable
{
	public static TypedBuffer<byte> MaterialBuffer = new(RenderScene.MaxInstances * 64, isRaw: true);

	// Must match MAX_MATERIAL_STACKS in the material binning shaders, which index their buffers by StackID.
	public const int MaxStacks = 256;

	#region Cache

	private static Dictionary<MaterialKey, RenderMaterial> cache = new();

	/// <summary>
	/// Returns the shared material for a given source material and override set, creating it if needed.
	/// Every caller must <see cref="Dispose"/> the result exactly once.
	/// </summary>
	public static RenderMaterial Get(Material source, IReadOnlyList<ShaderParameter>? overrides = null)
	{
		MaterialKey key = new(source, overrides);

		if (!cache.TryGetValue(key, out var material))
		{
			material = cache[key] = new RenderMaterial(key);
		}

		material.refCount++;
		return material;
	}

	private readonly MaterialKey key;
	private int refCount = 0;

	#endregion

	#region Permutations

	private static List<(IEnumerable<Shader>, int)> stackIDs = new();

	private static List<RenderMaterial> all = new();
	private static List<Type> requestedPermutationTypes = new();
	public static void RequestPermutation<T>() where T : ShaderPermutation, new()
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

	public IEnumerable<ShaderPermutation> Permutations => permutations;
	private List<ShaderPermutation> permutations = new();

	#endregion

	[Inspect] public Material Source => key.Source;

	public ShaderParameter[] Parameters { get; }
	public ObservableCollection<Shader> Shaders { get; } = new();

	public BufferAllocation<byte> MaterialHandle { get; private set; }

	public int StackID { get; private set; }
	private static int lastID = 0;

	private RenderMaterial(MaterialKey key)
	{
		this.key = key;
		Shaders.Add(Source.Shader);

		all.Add(this);

		// Calculate StackID
		var matchingStack = stackIDs.FirstOrDefault(o => o.Item1.SequenceEqual(Shaders));
		if (matchingStack.Item1 is null)
		{
			StackID = lastID++;
			Guard.Require(StackID < MaxStacks, $"Ran out of shader stacks - {MaxStacks} is the most the material binning shaders can index.");

			stackIDs.Add((Shaders.ToArray(), StackID));
		}
		else
		{
			StackID = matchingStack.Item2;
		}

		// Build parameters table, layering the material's own overrides then this instance's on top.
		Parameters = Shaders.SelectMany(o => o.Parameters).ToArray();
		for (int i = 0; i < Parameters.Length; i++)
		{
			if (Source.MaterialOverrides.TryFirst(o => o.Name == Parameters[i].Name, out var materialOverride))
			{
				Parameters[i].Value = materialOverride.Value;
			}
			if (key.TryGetOverride(Parameters[i].Name, out var instanceOverride))
			{
				Parameters[i].Value = instanceOverride.Value;
			}
		}

		// Update material data
		UpdateMaterialData();

		// Create requested permutations
		foreach (var type in requestedPermutationTypes)
		{
			permutations.Add(ShaderPermutation.FindOrCreate(type, this));
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
			object? value = param.Value;

            // Before we do anything, make sure the value is loaded (if applicable).
            if (value is GameResource resource)
            {
                resource.EnsureFullyLoaded();
            }

			if (param.Type == typeof(bool) && value is bool boolValue)
			{
				// Interpret bools as integers due to size mismatch (8-bit in C#, 32-bit in HLSL)
				materialData.AddRange(StructureToByteArray(typeof(int), boolValue ? 1 : 0));
			}
			else if (param.Type == typeof(Texture2D) && value is Texture2D textureValue)
			{
                Guard.NotNull(textureValue.D3DResource);
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
		if (--refCount > 0)
		{
			return;
		}

		cache.Remove(key);
		all.Remove(this);

		MaterialHandle.Dispose();
	}
}

readonly struct MaterialKey : IEquatable<MaterialKey>
{
	public Material Source { get; }
	private readonly ShaderParameter[] overrides;

	public MaterialKey(Material source, IReadOnlyList<ShaderParameter>? overrides)
	{
		Source = source;
		this.overrides = overrides?.ToArray() ?? Array.Empty<ShaderParameter>();
	}

	public bool TryGetOverride(string name, out ShaderParameter result)
	{
		for (int i = 0; i < overrides.Length; i++)
		{
			if (overrides[i].Name == name)
			{
				result = overrides[i];
				return true;
			}
		}

		result = default;
		return false;
	}

	public bool Equals(MaterialKey other)
	{
		if (Source != other.Source || overrides.Length != other.overrides.Length)
		{
			return false;
		}

		for (int i = 0; i < overrides.Length; i++)
		{
			if (overrides[i].Name != other.overrides[i].Name || !Equals(overrides[i].Value, other.overrides[i].Value))
			{
				return false;
			}
		}

		return true;
	}

	public override bool Equals(object? obj) => obj is MaterialKey other && Equals(other);

	public override int GetHashCode()
	{
		HashCode hash = new();
		hash.Add(Source);

		for (int i = 0; i < overrides.Length; i++)
		{
			hash.Add(overrides[i].Name);
			hash.Add(overrides[i].Value);
		}

		return hash.ToHashCode();
	}
}
