using System;
using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Graphics;

public class MaterialInstance : IDisposable
{
	public static GraphicsBuffer<byte> MaterialBuffer = new(Scene.MaxInstances * 64, isRaw: true);

	#region Permutations
	private static List<MaterialInstance> all = new();
	private static List<Type> requestedPermutationTypes = new();
	public static void RequestPermutation<T>() where T : ShaderPermutation, new()
	{
		if (!requestedPermutationTypes.Contains(typeof(T)))
		{
			requestedPermutationTypes.Add(typeof(T));

			foreach (var instance in all)
			{
				instance.permutations.Add(ShaderPermutation.FindOrCreate<T>(instance.Shaders));
			}
		}
	}

	public IEnumerable<ShaderPermutation> Permutations => permutations;
	private List<ShaderPermutation> permutations = new();

	#endregion

	[Inspect] public Material Material { get; set; }

	public ShaderParameter[] Parameters;
	public ObservableCollection<Shader> Shaders = new();

	public MaterialInstance(Material baseMaterial)
	{
		Material = baseMaterial;
		Shaders.Add(Material.Shader);

		foreach (var type in requestedPermutationTypes)
		{
			permutations.Add(ShaderPermutation.FindOrCreate(type, Shaders));
		}

		all.Add(this);
	}

	public void Dispose()
	{
		all.Remove(this);
	}
}