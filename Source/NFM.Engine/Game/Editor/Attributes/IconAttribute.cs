namespace NFM;

[AttributeUsage(AttributeTargets.Class)]
public class IconAttribute : Attribute
{
	public string IconGlyph { get; set; }

	public IconAttribute(string iconGlyph)
	{
		IconGlyph = iconGlyph;
	}
}
