using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;

namespace NFM.Plugins;

public abstract class Plugin
{
	public virtual void OnStart() {}

	protected bool TryLoadConfig(out JsonDocument document, string configName = null)
	{
		if (configName == null)
		{
			configName = GetType().Name;
		}

		// Get full config path from name.
		string runtimeDir = Directory.GetParent(typeof(Plugin).Assembly.Location).Parent.FullName;
		string configPath = $"{runtimeDir}/Config/{configName}.json";

		// Check if config actually exists.
		if (File.Exists(configPath))
		{
			// Open the file in readonly mode
			using (var configStream = File.OpenRead(configPath))
			{
				try
				{
					document = JsonDocument.Parse(configStream, new()
					{
						AllowTrailingCommas = true,
						CommentHandling = JsonCommentHandling.Skip,
					});

					return true;
				}
				catch (Exception e)
				{
					Debug.LogError(e);
				}
			}
		}

		document = default;
		return false;
	}
}
