using System;
using System.Globalization;
using NFM;
using NFM.Common;
using NFM.Mathematics;
using SourceEngine.Filesystem;
using ValveKeyValue;

namespace SourceEngine.Formats;

/// <summary>A parsed .vmt - shader name plus a flat param bag, patches already resolved.</summary>
public sealed class VmtFile
{
	private const int MaxPatchDepth = 8;

	private static readonly KVSerializer Serializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);

	public string Shader { get; private set; }
	public Dictionary<string, string> Parameters { get; } = new(StringComparer.OrdinalIgnoreCase);

	public static VmtFile Read(SourceFileSystem fileSystem, string diskPath) => Read(fileSystem, diskPath, 0);

	private static VmtFile Read(SourceFileSystem fileSystem, string diskPath, int depth)
	{
		KVObject root;

		try
		{
			using (var stream = File.OpenRead(diskPath))
			{
				root = Serializer.Deserialize(stream);
			}
		}
		catch (Exception e) when (e is KeyValueException or InvalidOperationException)
		{
			// A handful of shipped materials are genuinely malformed; they still get to be an asset.
			Log.Warn($"Couldn't parse {diskPath} - {e.Message}");
			return new VmtFile() { Shader = "UnlitGeneric" };
		}

		// A patch material is a thin overlay on whichever material it includes.
		if (root.Name.Equals("patch", StringComparison.OrdinalIgnoreCase) && depth < MaxPatchDepth)
		{
			return ApplyPatch(fileSystem, root, depth);
		}

		VmtFile vmt = new() { Shader = root.Name };
		vmt.Collect(root);

		return vmt;
	}

	private static VmtFile ApplyPatch(SourceFileSystem fileSystem, KVObject root, int depth)
	{
		string include = root.Children
			.FirstOrDefault(o => o.Name.Equals("include", StringComparison.OrdinalIgnoreCase))
			?.Value.ToString(CultureInfo.InvariantCulture);

		if (include is null || !fileSystem.TryResolve(include, out string diskPath))
		{
			Log.Warn($"Patch material includes '{include}', which doesn't resolve");
			return new VmtFile() { Shader = "UnlitGeneric" };
		}

		VmtFile vmt = Read(fileSystem, diskPath, depth + 1);

		foreach (KVObject child in root.Children)
		{
			if (child.Name.Equals("insert", StringComparison.OrdinalIgnoreCase) || child.Name.Equals("replace", StringComparison.OrdinalIgnoreCase))
			{
				vmt.Collect(child);
			}
		}

		return vmt;
	}

	// Nested blocks are proxies, fallbacks and the like - none of which we translate.
	private void Collect(KVObject block)
	{
		foreach (KVObject child in block.Children)
		{
			if (child.Value.ValueType != KVValueType.Collection)
			{
				Parameters[child.Name] = child.Value.ToString(CultureInfo.InvariantCulture);
			}
		}
	}

	public bool Has(string name) => Parameters.ContainsKey(name);

	public string GetString(string name) => Parameters.GetValueOrDefault(name);

	public bool GetBool(string name) => GetFloat(name, 0) != 0;

	public float GetFloat(string name, float fallback)
	{
		string value = GetString(name);
		return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result) ? result : fallback;
	}

	/// <summary>Reads a "[1 .5 .2]" or "{255 128 64}" triple, the latter being 0-255.</summary>
	public Color GetColor(string name, Color fallback)
	{
		string value = GetString(name)?.Trim();
		if (string.IsNullOrEmpty(value))
		{
			return fallback;
		}

		bool isByteRange = value.StartsWith('{');
		string[] parts = value.Trim('[', ']', '{', '}').Split(' ', StringSplitOptions.RemoveEmptyEntries);

		if (parts.Length < 3)
		{
			// A bare scalar is a legal shorthand for a grey.
			return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float grey) ? new Color(grey, grey, grey) : fallback;
		}

		float[] channels = new float[3];
		for (int i = 0; i < 3; i++)
		{
			if (!float.TryParse(parts[i], NumberStyles.Float, CultureInfo.InvariantCulture, out channels[i]))
			{
				return fallback;
			}

			if (isByteRange)
			{
				channels[i] /= 255f;
			}
		}

		return new Color(channels[0], channels[1], channels[2]);
	}
}
