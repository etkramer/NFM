namespace NFM.Resources;

public enum BlendMode
{
	Opaque,
	Masked,
	Transparent,
}

public struct ShaderParameter
{
	public string Name;
	public object Value;
	public Type Type;
}

public sealed class Shader : GameResource
{
	public string ShaderSource { get; set; }

	public List<ShaderParameter> Parameters { get; } = new();
	public BlendMode BlendMode = BlendMode.Opaque;

	public Shader(string source)
	{
		ShaderSource = source;
	}

	public void AddColor(string param, Color defaultValue = default)
	{
		Parameters.Add(new ShaderParameter()
		{
			Name = param,
			Value = defaultValue,
			Type = typeof(Color)
		});
	}

	public void AddTexture(string param, Texture2D defaultValue = default)
	{
		Parameters.Add(new ShaderParameter()
		{
			Name = param,
			Value = defaultValue,
			Type = typeof(Texture2D)
		});
	}

	public void SetBlendMode(BlendMode mode)
	{
		BlendMode = mode;
	}
}