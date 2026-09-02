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
	public object? Value;
	public Type Type;
}

[Icon("gradient", IconColor = "#b98ce0")]
public sealed class Shader : GameResource
{
	public string ShaderSource { get; }

	public List<ShaderParameter> Parameters { get; } = [];
	public required BlendMode BlendMode { get; init; } = BlendMode.Opaque;

	public Shader(string source)
	{
		ShaderSource = source;
	}

	public void AddColorParam(string paramName, Color defaultValue = default)
	{
		Parameters.Add(new ShaderParameter()
		{
			Name = paramName,
			Value = defaultValue,
			Type = typeof(Color)
		});
	}

	public void AddFloatParam(string paramName, float defaultValue = default)
	{
		Parameters.Add(new ShaderParameter()
		{
			Name = paramName,
			Value = defaultValue,
			Type = typeof(float)
		});
	}

	public void AddTextureParam(string paramName, Texture2D? defaultValue = default)
	{
		Parameters.Add(new ShaderParameter()
		{
			Name = paramName,
			Value = defaultValue,
			Type = typeof(Texture2D)
		});
	}
}