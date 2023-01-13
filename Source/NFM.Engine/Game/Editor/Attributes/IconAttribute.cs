using System;

namespace NFM
{
	[AttributeUsage(AttributeTargets.Class)]
	public class IconAttribute : Attribute
	{
		public char IconGlyph { get; set; }

		public IconAttribute(char iconGlyph)
		{
			IconGlyph = iconGlyph;
		}
	}
}
