using System;
using System.Xml.XPath;
using Avalonia.Markup.Xaml.Templates;
using NFM.GPU;
using NFM.Resources;

namespace NFM.Graphics;

public abstract class ShaderPermutation : IDisposable
{
	public int ID { get; private set; }
	private static int lastID = 0;

	public IEnumerable<Shader> Shaders => shaders;
	private Shader[] shaders;

	protected ShaderPermutation() {}

	public abstract void Init(ShaderModule module);
	public abstract void Dispose();

	public static IReadOnlyDictionary<Type, IEnumerable<ShaderPermutation>> All => all;
	private static Dictionary<Type, IEnumerable<ShaderPermutation>> all = new();

	public static ShaderPermutation FindOrCreate<T>(IEnumerable<Shader> shaders) where T : ShaderPermutation, new() => FindOrCreate(typeof(T), shaders);
	public static ShaderPermutation FindOrCreate(Type type, IEnumerable<Shader> shaders)
	{
		// Try to find an existing, matching permutation
		if (all.TryGetValue(type, out var typedList))
		{
			var match = typedList.FirstOrDefault(o => o.shaders.SequenceEqual(shaders));
			
			if (match != null)
			{
				return match;
			}
		}
		else
		{
			all[type] = new List<ShaderPermutation>();
		}

		// Create a new permutation instance
		var result = Activator.CreateInstance(type) as ShaderPermutation;
		result.shaders = shaders.ToArray();
		result.ID = lastID++;

		// Set it up (can't use a parameterized constructor here)
		result.Init(new ShaderModule(BuildSource(shaders), ShaderStage.Library));

		//... and return it
		(all[type] as IList<ShaderPermutation>).Add(result);
		return result;
	}

	private static string BuildSource(IEnumerable<Shader> shaders)
	{
		string paramSource = "";
		string setupSource = "";

		int paramOffset = 4;
		foreach (var param in shaders.SelectMany(o => o.Parameters).Distinct())
		{
			// Override sizes where needed.
			int paramSize = param.Value switch
			{
				Texture2D => sizeof(uint),
				bool or byte or sbyte => Marshal.SizeOf(typeof(int)),
				_ => Marshal.SizeOf(param.Type)
			};

			// Add HLSL code for declaring parameters
			paramSource += param.Value switch
			{
				Texture2D => $"Texture2D<float4> {param.Name};\n",
				bool => $"bool {param.Name};\n",
				int or sbyte => $"int {param.Name};\n",
				uint or byte => $"uint {param.Name};\n",
				float => $"float {param.Name};\n",
				Vector4 or Color => $"float4 {param.Name};\n",
				Vector3 => $"float3 {param.Name};\n",
				Vector2 => $"float2 {param.Name};\n",
				Vector4i => $"int4 {param.Name};\n",
				Vector3i => $"int3 {param.Name};\n",
				Vector2i => $"int2 {param.Name};\n",

				_ => throw new NotSupportedException($"{param.Type.Name} is not a supported shader parameter type")
			};

			// Add HLSL code for loading parameters
			setupSource += param.Value switch
			{
				Texture2D => $"input.{param.Name} = ResourceDescriptorHeap[MaterialParams.Load(materialID + {paramOffset})];\n",
				bool => $"input.{param.Name} = (bool)MaterialParams.Load(materialID + {paramOffset});\n",
				int or sbyte => $"input.{param.Name} = asint(MaterialParams.Load(materialID + {paramOffset}));\n",
				uint or byte => $"input.{param.Name} = asuint(MaterialParams.Load(materialID + {paramOffset}));\n",
				float => $"input.{param.Name} = asfloat(MaterialParams.Load(materialID + {paramOffset}));\n",
				Vector4 or Color => $"input.{param.Name} = asfloat(MaterialParams.Load4(materialID + {paramOffset}));\n",
				Vector3 => $"input.{param.Name} = asfloat(MaterialParams.Load3(materialID + {paramOffset}));\n",
				Vector2 => $"input.{param.Name} = asfloat(MaterialParams.Load2(materialID + {paramOffset}));\n",
				Vector4i => $"input.{param.Name} = asint(MaterialParams.Load4(materialID + {paramOffset}));\n",
				Vector3i => $"input.{param.Name} = asint(MaterialParams.Load3(materialID + {paramOffset}));\n",
				Vector2i => $"input.{param.Name} = asint(MaterialParams.Load2(materialID + {paramOffset}));\n",

				_ => throw new NotSupportedException($"{param.Type.Name} is not a supported shader parameter type")
			};

			paramOffset += paramSize;
		}

		// Generate shader source from template
		return Embed.GetString("Shaders/Common/SFTemplate.hlsl")
			.Replace("#pragma MAIN", shaders.First().ShaderSource)
			.Replace("#pragma PARAMS", paramSource)
			.Replace("#pragma SETUP", setupSource);
	}
}
