using System.Data;

namespace NFM.Components;

static class NumberParser
{
	private static readonly DataTable ComputeTable = new();

	public static bool TryParse(string? text, Type numberType, out object? number)
	{
		// Interpret an emptied field as zero.
		if (string.IsNullOrWhiteSpace(text))
		{
			number = Convert.ChangeType(0, numberType);
			return true;
		}

		try
		{
			text = ComputeTable.Compute(text, null).ToString();
		}
		catch
		{
			// No problem, probably just not a valid expression.
		}

		if (IsFloat(numberType))
		{
			if (double.TryParse(text, out double floatValue))
			{
				number = Convert.ChangeType(floatValue, numberType);
				return true;
			}
		}
		else if (IsUnsigned(numberType))
		{
			if (ulong.TryParse(text, out ulong unsignedValue))
			{
				number = Convert.ChangeType(unsignedValue, numberType);
				return true;
			}
		}
		else if (long.TryParse(text, out long signedValue))
		{
			number = Convert.ChangeType(signedValue, numberType);
			return true;
		}

		number = null;
		return false;
	}

	private static bool IsUnsigned(Type type) =>
		type == typeof(byte) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);

	private static bool IsFloat(Type type) =>
		type == typeof(float) || type == typeof(double) || type == typeof(decimal);
}
