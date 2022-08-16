using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

namespace Engine.Core
{
	public static class StringHelper
	{
		static Regex pascalToDislayQuery = new Regex(@"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", RegexOptions.Compiled);

		public static string PascalToDisplay(this string input, bool titleCase = true)
		{
			return pascalToDislayQuery.Replace(input, " $0");
		}
	}
}
