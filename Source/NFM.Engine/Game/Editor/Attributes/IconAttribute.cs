namespace NFM;

[AttributeUsage(AttributeTargets.Class)]
public class IconAttribute : Attribute
{
	public string IconGlyph { get; set; }

	/// <summary>
	/// CSS colour to tint the glyph with, or null to inherit.
	/// </summary>
	public string? IconColor { get; set; }

	public IconAttribute(string iconGlyph)
	{
		IconGlyph = iconGlyph;
	}
}
