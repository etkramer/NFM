using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace NFM.Generators;

public static class RoslynExtensions
{
	public static string GetFullName(this ITypeSymbol symbol)
	{
		string name = symbol.Name;
		GetFullNameRecurse(symbol, ref name);
		return name;
	}

	public static string GetFullNamespace(this ITypeSymbol symbol)
	{
		string name = null;
		GetFullNameRecurse(symbol, ref name);
		return name;
	}

	private static void GetFullNameRecurse(INamespaceOrTypeSymbol symbol, ref string output)
	{
		if (symbol.ContainingNamespace != null)
		{
			if (!string.IsNullOrWhiteSpace(symbol.ContainingNamespace.Name))
			{
				output = (output == null) ? symbol.ContainingNamespace.Name : $"{symbol.ContainingNamespace.Name}.{output}";
			}
			
			GetFullNameRecurse(symbol.ContainingNamespace, ref output);
		}
	}
}
