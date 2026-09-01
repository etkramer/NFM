using System.Reflection;
using System.Text.Json;
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
		Converters =
		{
			new Vector2Converter(),
			new Vector3Converter(),
			new Vector4Converter(),
			new ColorConverter()
		}
	};

	private class NodeData
	{
		public string Type { get; set; } = string.Empty;
		public Dictionary<string, JsonElement> Properties { get; set; } = [];
		public List<NodeData> Children { get; set; } = [];
	}

	/// <summary>
	/// Serializes the given nodes, along with their descendants. Any node that already appears as a
	/// descendant of another is skipped, so overlapping subtrees are only written once.
	/// </summary>
	public static string Serialize(IEnumerable<Node> nodes)
	{
		HashSet<Node> roots = [.. nodes];
		List<NodeData> data = [.. roots.Where(node => !HasAncestorIn(node, roots)).Select(Pack)];

		return JsonSerializer.Serialize(data, options);
	}

	/// <summary>
	/// Rebuilds the node trees described by the given JSON, returning their roots. Returns an empty
	/// list if the JSON doesn't describe nodes at all.
	/// </summary>
	public static List<Node> Deserialize(string json, Scene? scene)
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
			if (Unpack(item, scene, null) is Node node)
			{
				nodes.Add(node);
			}
		}

		return nodes;
	}

	private static NodeData Pack(Node node)
	{
		NodeData data = new() { Type = Guard.NotNull(node.GetType().FullName) };

		foreach (PropertyInfo property in GetSavedProperties(node.GetType()))
		{
			object? value = property.GetValue(node);

			try
			{
				data.Properties[property.Name] = value is GameResource resource
					? JsonSerializer.SerializeToElement(resource.Source?.Path, options)
					: JsonSerializer.SerializeToElement(value, property.PropertyType, options);
			}
			catch (Exception e) when (e is JsonException or NotSupportedException)
			{
				Log.Warn($"Couldn't serialize {data.Type}.{property.Name} ({property.PropertyType.Name})");
			}
		}

		foreach (Node child in node.Children)
		{
			data.Children.Add(Pack(child));
		}

		return data;
	}

	private static Node? Unpack(NodeData data, Scene? scene, Node? parent)
	{
		if (ResolveType(data.Type) is not Type type)
		{
			return null;
		}

		Node node = (Node)Guard.NotNull(Activator.CreateInstance(type, parent?.Scene ?? scene));

		if (parent is not null)
		{
			node.Parent = parent;
		}

		foreach (PropertyInfo property in GetSavedProperties(type))
		{
			if (!property.CanWrite || !data.Properties.TryGetValue(property.Name, out JsonElement element))
			{
				continue;
			}

			// Resources are stored as asset paths, and have to be loaded back in asynchronously.
			if (typeof(GameResource).IsAssignableFrom(property.PropertyType))
			{
				if (element.ValueKind is JsonValueKind.String)
				{
					_ = AssignResourceAsync(node, property, Guard.NotNull(element.GetString()));
				}

				continue;
			}

			try
			{
				property.SetValue(node, element.Deserialize(property.PropertyType, options));
			}
			catch (Exception e) when (e is JsonException or NotSupportedException)
			{
				Log.Warn($"Couldn't deserialize {data.Type}.{property.Name} ({property.PropertyType.Name})");
			}
		}

		foreach (NodeData child in data.Children)
		{
			Unpack(child, scene, node);
		}

		return node;
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

	private static IEnumerable<PropertyInfo> GetSavedProperties(Type type) => type
		.GetProperties(ReflectionHelper.BindingFlagsAllNonStatic)
		.Where(property => property.HasAttribute<InspectAttribute>() && property.CanRead);

	private static Type? ResolveType(string name) => string.IsNullOrEmpty(name) ? null : ReflectionHelper.LoadedAssemblies
		.Append(typeof(Node).Assembly)
		.Distinct()
		.Select(assembly => assembly.GetType(name))
		.FirstOrDefault(type => type is not null
			&& type.InheritsFrom(typeof(Node))
			&& !type.IsAbstract
			&& type.GetConstructor([typeof(Scene)]) is not null);
}
