using System;
using System.Reflection;
using System.Text.Json;

namespace Engine.Plugins
{
	public abstract class Plugin
	{
		public virtual void OnStart() {}

		protected JsonDocument LoadConfig(string configName = null)
		{
			if (configName == null)
			{
				configName = GetType().Name;
			}

			// Get full config path from name.
			string runtimeDir = Directory.GetParent(typeof(Plugin).Assembly.Location).Parent.FullName;
			string configPath = $"{runtimeDir}/Config/{configName}.json";

			// Check if config actually exists.
			if (!File.Exists(configPath))
				return null;

			using (var configStream = File.OpenRead(configPath))
			{
				JsonDocumentOptions opts = new()
				{
					AllowTrailingCommas = true,
					CommentHandling = JsonCommentHandling.Skip,
				};

				try
				{
					return JsonDocument.Parse(configStream, opts);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
					return null;
				}
			}
		}
	}
}
