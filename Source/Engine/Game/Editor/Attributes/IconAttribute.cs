using System;

namespace Engine
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
