using System;

namespace Engine.Core
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class SaveAttribute : Attribute
	{
		public bool Global { get; set; } = false;

		public SaveAttribute()
		{

		}
	}
}
