using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NFM.Common
{
	public static class StringHelper
	{
		static readonly Regex pascalToDislayQuery = new(@"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", RegexOptions.Compiled);

		public static string PascalToDisplay(this string input, bool titleCase = true)
		{
			return pascalToDislayQuery.Replace(input, " $0");
		}
	}
}
