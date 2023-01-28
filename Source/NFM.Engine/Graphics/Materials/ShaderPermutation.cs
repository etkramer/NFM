using System;
using System.Xml.XPath;
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

		// Create new MaterialPermutation instance
		var result = Activator.CreateInstance(type) as ShaderPermutation;
		result.shaders = shaders.ToArray();
		result.ID = lastID++;

		// Set it up (can't use a parameterized constructor here)
		result.Init(null);

		//... and return it
		(all[type] as IList<ShaderPermutation>).Add(result);
		return result;
	}
}
