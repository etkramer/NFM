using System.Text.Json;
using System.Text.Json.Serialization;

namespace NFM;

internal static class MathConverters
{
	public static float[] ReadArray(ref Utf8JsonReader reader, int length)
	{
		float[] values = JsonSerializer.Deserialize<float[]>(ref reader) ?? [];
		return values.Length == length ? values : new float[length];
	}

	public static void WriteArray(Utf8JsonWriter writer, params float[] values)
	{
		writer.WriteStartArray();

		foreach (float value in values)
		{
			writer.WriteNumberValue(value);
		}

		writer.WriteEndArray();
	}
}

internal class Vector2Converter : JsonConverter<Vector2>
{
	public override Vector2 Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
	{
		float[] values = MathConverters.ReadArray(ref reader, 2);
		return new(values[0], values[1]);
	}

	public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
	{
		MathConverters.WriteArray(writer, value.X, value.Y);
	}
}

internal class Vector3Converter : JsonConverter<Vector3>
{
	public override Vector3 Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
	{
		float[] values = MathConverters.ReadArray(ref reader, 3);
		return new(values[0], values[1], values[2]);
	}

	public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
	{
		MathConverters.WriteArray(writer, value.X, value.Y, value.Z);
	}
}

internal class Vector4Converter : JsonConverter<Vector4>
{
	public override Vector4 Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
	{
		float[] values = MathConverters.ReadArray(ref reader, 4);
		return new(values[0], values[1], values[2], values[3]);
	}

	public override void Write(Utf8JsonWriter writer, Vector4 value, JsonSerializerOptions options)
	{
		MathConverters.WriteArray(writer, value.X, value.Y, value.Z, value.W);
	}
}

internal class ColorConverter : JsonConverter<Color>
{
	public override Color Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
	{
		float[] values = MathConverters.ReadArray(ref reader, 4);
		return new(values[0], values[1], values[2], values[3]);
	}

	public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
	{
		MathConverters.WriteArray(writer, value.R, value.G, value.B, value.A);
	}
}
