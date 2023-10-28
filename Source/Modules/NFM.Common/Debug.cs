using System;
using System.Diagnostics;

namespace NFM.Common;

public static class Debug
{
	#if DEBUG
	public const bool IsDebugBuild = true;
	#else
	public const bool IsDebugBuild = false;
	#endif

	public static readonly bool IsDebugMode;

	static Debug()
	{
		IsDebugMode = IsDebugBuild && Debugger.IsAttached;
	}
}
