namespace NFM.Resources;

public sealed class Material : GameResource
{
	public Shader Shader { get; }

	public List<ShaderParameter> MaterialOverrides { get; } = new();

	public Material(Shader shader)
	{
		Shader = shader;
	}

	public void SetColor(string param, Color value)
	{
		MaterialOverrides.Add(new ShaderParameter()
		{
			Name = param,
			Value = value
		});
	}

	public void SetTexture(string param, Texture2D value)
	{
		if (value is null)
		{
			return;
		}

		MaterialOverrides.Add(new ShaderParameter()
		{
			Name = param,
			Value = value
		});
	}
}