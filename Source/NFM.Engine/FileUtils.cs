namespace NFM;

public static class FileUtils
{
	private static readonly string s_exeDir = Path.GetFullPath(AppContext.BaseDirectory);

	/// <summary>
	/// Returns the full path to the runtime directory (./).
	/// </summary>
	public static string GetBasePath() => Path.GetFullPath(Path.Combine(s_exeDir, ".."));

	/// <summary>
	/// Returns the full path to the binaries directory (./Binaries/).
	/// </summary>
	public static string GetBinariesPath() => Path.Combine(GetBasePath(), "Binaries");

	/// <summary>
	/// Returns the full path to the content directory (./Content/).
	/// </summary>
	public static string GetContentPath() => Path.Combine(GetBasePath(), "Content");

	/// <summary>
	/// Returns the full path to the plugins directory (./Plugins/).
	/// </summary>
	public static string GetPluginsPath() => Path.Combine(GetBasePath(), "Plugins");
}
