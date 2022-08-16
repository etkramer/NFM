using System;
using System.Text.Json;

namespace Engine.Core
{
	public interface ICustomSave
	{
		public void CustomSave(CustomSaveContext context, out bool handled);
		public void CustomLoad(CustomLoadContext context, out bool handled);
	}

	public sealed class CustomSaveContext
	{
		private Utf8JsonWriter writer;

		public CustomSaveContext(Utf8JsonWriter writer)
		{
			this.writer = writer;
		}

		public void AddValue(string name, string value)
		{
			writer.WritePropertyName(name);
			writer.WriteStringValue(value);
		}
	}

	public sealed class CustomLoadContext
	{
		public T GetValue<T>(string name)
		{
			return default(T);
		}
	}
}
