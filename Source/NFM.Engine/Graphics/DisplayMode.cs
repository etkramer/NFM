namespace NFM.Graphics;

/// <summary>
/// Selects what the lighting pass writes to the color target. Values must match the DISPLAY_ constants in LightingCS.hlsl.
/// </summary>
public enum DisplayMode
{
	Lit,
	Unlit,
	DebugNormals,
	DebugMetallic,
	DebugSpecular,
	DebugRoughness,
	DebugShadows,
}
