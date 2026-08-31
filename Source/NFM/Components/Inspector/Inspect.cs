using NFM.Resources;

namespace NFM.Components;

/// <summary>
/// Maps inspected property types to the component that edits them.
/// </summary>
public static class Inspect
{
	private static readonly Type[] VectorTypes =
	[
		typeof(Vector2), typeof(Vector2i),
		typeof(Vector3), typeof(Vector3i),
		typeof(Vector4), typeof(Vector4i)
	];

	public static int ComponentCount(Type type)
	{
		if (type == typeof(Vector2) || type == typeof(Vector2i)) return 2;
		if (type == typeof(Vector3) || type == typeof(Vector3i)) return 3;
		if (type == typeof(Vector4) || type == typeof(Vector4i)) return 4;

		return 0;
	}

	public static Type? GetEditor(Type type)
	{
		if (type == typeof(bool))
		{
			return typeof(BoolEditor);
		}

		if (type == typeof(string))
		{
			return typeof(StringEditor);
		}

		if (VectorTypes.Contains(type))
		{
			return typeof(VectorEditor);
		}

		if (type.IsAssignableTo(typeof(GameResource)))
		{
			return typeof(ResourceEditor);
		}

		if (IsNumber(type))
		{
			return typeof(NumberEditor);
		}

		return null;
	}

	private static bool IsNumber(Type type)
	{
		return type.GetInterfaces().Any(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(System.Numerics.INumber<>));
	}
}
