using System;

namespace NFM.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class SaveAttribute : Attribute
	{
		/// <summary>
		/// Should this be saved as part of the project or the user settings?
		/// </summary>
		public bool IsUserConfig { get; set; } = false;

		public SaveAttribute()
		{

		}
	}
}