using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using NFM.Common;
using NFM.Resources;
using NFM.Threading;
using NFM.World;

namespace NFM;

/// <summary>
/// Converts node trees to and from JSON, using each node's [Inspect] properties.
/// </summary>
public static class NodeSerializer
{
	private static readonly JsonSerializerOptions options = new()
	{
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		Converters =
		{
			new Vector2Converter(),
			new Vector3Converter(),
			new Vector4Converter(),
			new ColorConverter()
		}
	};

	private const int CurrentVersion = 1;

	private class ProjectData
	{
		public int Version { get; set; } = CurrentVersion;
		public List<NodeData> Nodes { get; set; } = [];

		/// <summary>
		/// The editor's view camera, restored onto the live one rather than spawned as a node.
		/// </summary>
		public NodeData? Camera { get; set; }
	}

	private class NodeData
	{
		/// <summary>
		/// The node's type, or null for an owned node - its owner decides what to spawn.
		/// </summary>
		public string? Type { get; set; }

		/// <summary>
		/// Set for an owned node, addressing it within its parent rather than creating it.
		/// </summary>
		public string? Key { get; set; }

		public Dictionary<string, JsonElement>? Properties { get; set; }
		public List<NodeData>? Children { get; set; }
	}

	/// <summary>
	/// Serializes the given nodes, along with their descendants. Any node that already appears as a
	/// descendant of another is skipped, so overlapping subtrees are only written once.
	/// </summary>
	public static string Serialize(IEnumerable<Node> nodes)
	{
		HashSet<Node> roots = [.. nodes];
		List<NodeData> data = [.. roots.Where(node => !HasAncestorIn(node, roots)).Select(Pack).OfType<NodeData>()];

		return JsonSerializer.Serialize(data, options);
	}

	/// <summary>
	/// Rebuilds the node trees described by the given JSON, returning their roots. Returns an empty
	/// list if the JSON doesn't describe nodes at all.
	/// </summary>
	public static async Task<List<Node>> Deserialize(string json, Scene? scene)
	{
		List<NodeData>? data;

		try
		{
			data = JsonSerializer.Deserialize<List<NodeData>>(json, options);
		}
		catch (JsonException)
		{
			return [];
		}

		List<Node> nodes = [];
		foreach (NodeData item in data ?? [])
		{
			if (await Unpack(item, scene, null) is Node node)
			{
				nodes.Add(node);
			}
		}

		return nodes;
	}

	/// <summary>
	/// Serializes a whole project - every scene root that isn't transient, plus the editor's view camera.
	/// </summary>
	public static string SerializeProject(Scene scene, Node? camera)
	{
		ProjectData data = new()
		{
			Nodes = [.. scene.RootNodes.Where(node => !node.IsTransient).Select(Pack).OfType<NodeData>()],
			Camera = camera is null ? null : Pack(camera)
		};

		return JsonSerializer.Serialize(data, options);
	}

	/// <summary>
	/// Rebuilds a project into the given scene, returning its saved view for <see cref="ApplyView"/>.
	/// </summary>
	public static async Task<string?> DeserializeProject(string json, Scene scene)
	{
		ProjectData? data;

		try
		{
			data = JsonSerializer.Deserialize<ProjectData>(json, options);
		}
		catch (JsonException)
		{
			Log.Warn("Couldn't read project - the file isn't valid JSON");
			return null;
		}

		if (data is null)
		{
			return null;
		}

		if (data.Version > CurrentVersion)
		{
			Log.Warn($"Project was saved by a newer version ({data.Version}), and may not load correctly");
		}

		foreach (NodeData item in data.Nodes)
		{
			await Unpack(item, scene, null);
		}

		return data.Camera is null ? null : JsonSerializer.Serialize(data.Camera, options);
	}

	/// <summary>
	/// Restores a view from <see cref="DeserializeProject"/> onto a live camera.
	/// </summary>
	public static async Task ApplyView(string view, Node camera)
	{
		NodeData? data;

		try
		{
			data = JsonSerializer.Deserialize<NodeData>(view, options);
		}
		catch (JsonException)
		{
			return;
		}

		if (data is not null)
		{
			await UnpackProperties(camera, camera.GetType(), data.Properties, camera.GetType().Name);
		}
	}

	private static NodeData? Pack(Node node)
	{
		NodeData data = new()
		{
			Type = node.IsOwned ? null : Guard.NotNull(node.GetType().FullName),
			Key = node.OwnerKey,
			Properties = PackProperties(node)
		};

		foreach (Node child in node.Children)
		{
			if (Pack(child) is NodeData childData)
			{
				(data.Children ??= []).Add(childData);
			}
		}

		// An owned node the user hasn't touched is respawned as-is, so there's nothing to write.
		return node.IsOwned && data.Properties is null && data.Children is null ? null : data;
	}

	private static Dictionary<string, JsonElement>? PackProperties(Node node)
	{
		Type type = node.GetType();
		PropertyInfo[] properties = GetSavedProperties(type);
		object?[]? defaults = node.SavedDefaults;

		Dictionary<string, JsonElement>? into = null;

		for (int i = 0; i < properties.Length; i++)
		{
			PropertyInfo property = properties[i];
			object? value = property.GetValue(node);

			if (defaults is not null && Equals(value, defaults[i]))
			{
				continue;
			}

			try
			{
				(into ??= [])[property.Name] = value is GameResource resource
					? JsonSerializer.SerializeToElement(resource.Source?.Path, options)
					: JsonSerializer.SerializeToElement(value, property.PropertyType, options);
			}
			catch (Exception e) when (e is JsonException or NotSupportedException)
			{
				Log.Warn($"Couldn't serialize {type.FullName}.{property.Name} ({property.PropertyType.Name})");
			}
		}

		return into;
	}

	/// <summary>
	/// Records a spawned node's property values, so a save can leave out anything still untouched.
	/// </summary>
	internal static void CaptureDefaults(Node node)
	{
		PropertyInfo[] properties = GetSavedProperties(node.GetType());
		object?[] defaults = new object?[properties.Length];

		for (int i = 0; i < properties.Length; i++)
		{
			defaults[i] = properties[i].GetValue(node);
		}

		node.SavedDefaults = defaults;
	}

	private static async Task<Node?> Unpack(NodeData data, Scene? scene, Node? parent)
	{
		Node node;

		if (data.Key is not null)
		{
			if (parent?.FindOwned(data.Key) is not Node owned)
			{
				Log.Warn($"Dropping saved state for '{data.Key}', which {parent?.GetType().Name} no longer owns");
				return null;
			}

			node = owned;
		}
		else
		{
			if (ResolveType(data.Type) is not Type type)
			{
				return null;
			}

			node = (Node)Guard.NotNull(Activator.CreateInstance(type, parent?.Scene ?? scene));

			if (parent is not null)
			{
				node.Parent = parent;
			}
		}

		// Resources have to land before children are unpacked - they're what spawns the owned ones.
		await UnpackProperties(node, node.GetType(), data.Properties, node.GetType().Name);

		foreach (NodeData child in data.Children ?? [])
		{
			await Unpack(child, scene, node);
		}

		return node;
	}

	private static async Task UnpackProperties(Node node, Type type, Dictionary<string, JsonElement>? properties, string typeName)
	{
		if (properties is null)
		{
			return;
		}

		foreach (PropertyInfo property in GetSavedProperties(type))
		{
			if (!property.CanWrite || !properties.TryGetValue(property.Name, out JsonElement element))
			{
				continue;
			}

			// Resources are stored as asset paths, and have to be loaded back in asynchronously.
			if (typeof(GameResource).IsAssignableFrom(property.PropertyType))
			{
				if (element.ValueKind is JsonValueKind.String)
				{
					await AssignResourceAsync(node, property, Guard.NotNull(element.GetString()));
				}

				continue;
			}

			try
			{
				property.SetValue(node, element.Deserialize(property.PropertyType, options));
			}
			catch (Exception e) when (e is JsonException or NotSupportedException)
			{
				Log.Warn($"Couldn't deserialize {typeName}.{property.Name} ({property.PropertyType.Name})");
			}
		}
	}

	private static async Task AssignResourceAsync(Node node, PropertyInfo property, string path)
	{
		GameResource? resource = await Asset.LoadAsync(path, property.PropertyType);

		if (resource is not null)
		{
			await Dispatcher.InvokeAsync(() => property.SetValue(node, resource));
		}
	}

	private static bool HasAncestorIn(Node node, HashSet<Node> nodes)
	{
		for (Node? parent = node.Parent; parent is not null; parent = parent.Parent)
		{
			if (nodes.Contains(parent))
			{
				return true;
			}
		}

		return false;
	}

	private static readonly ConcurrentDictionary<Type, PropertyInfo[]> savedProperties = [];

	private static PropertyInfo[] GetSavedProperties(Type type) => savedProperties.GetOrAdd(type, key => [.. key
		.GetProperties(ReflectionHelper.BindingFlagsAllNonStatic)
		.Where(property => property.HasAttribute<InspectAttribute>() && property.CanRead)]);

	private static Type? ResolveType(string? name) => string.IsNullOrEmpty(name) ? null : ReflectionHelper.LoadedAssemblies
		.Append(typeof(Node).Assembly)
		.Distinct()
		.Select(assembly => assembly.GetType(name))
		.FirstOrDefault(type => type is not null
			&& type.InheritsFrom(typeof(Node))
			&& !type.IsAbstract
			&& type.GetConstructor([typeof(Scene)]) is not null);
}
