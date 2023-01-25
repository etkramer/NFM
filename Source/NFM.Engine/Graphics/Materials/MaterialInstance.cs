using System;
using System.Linq;
using NFM.Common;
using NFM.GPU;
using NFM.GPU.Native;
using NFM.Resources;
using NFM.World;

namespace NFM.Graphics;

public class MaterialInstance : IDisposable
{
	public static GraphicsBuffer<byte> MaterialBuffer = new(Scene.MaxInstances * 64, isRaw: true);

	[Inspect] public Material Material { get; set; }
	[Inspect] public ShaderStack ShaderStack { get; set; }

	public MaterialInstance(Material baseMaterial)
	{
		Material = baseMaterial;
		ShaderStack = new ShaderStack(Material.Shader);
	}

	public void Dispose()
	{

	}
}