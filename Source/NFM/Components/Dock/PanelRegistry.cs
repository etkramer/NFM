using System.Reflection;

namespace NFM.Components;

/// <summary>
/// Discovers <see cref="IPanel"/> implementations and gives each a stable key for serialization.
/// </summary>
public static class PanelRegistry
{
    public sealed record Entry(Type Type, string Key, string Name, bool IsTransparent);

    static readonly Dictionary<string, Entry> byKey = [with(StringComparer.Ordinal)];
    static readonly Dictionary<Type, Entry> byType = [];

    static PanelRegistry()
    {
        const BindingFlags Statics = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        IEnumerable<Type> types = typeof(PanelRegistry).Assembly
            .GetTypes()
            .Where(o => !o.IsAbstract && !o.IsGenericTypeDefinition && o.IsAssignableTo(typeof(IPanel)));

        foreach (Type type in types)
        {
            // Name is a static abstract member, so it can only be read reflectively from here.
            if (type.GetProperty(nameof(IPanel.Name), Statics)?.GetValue(null) is not string name)
            {
                continue;
            }

            bool isTransparent =
                type.GetProperty(nameof(IPanel.IsTransparent), Statics)?.GetValue(null) is true;

            Entry entry = new(type, type.Name, name, isTransparent);
            byKey[entry.Key] = entry;
            byType[type] = entry;
        }
    }

    public static IReadOnlyCollection<Entry> All => byType.Values;

    public static Entry Get(Type type) =>
        byType.TryGetValue(type, out Entry? entry)
            ? entry
            : throw new ArgumentException($"{type.Name} is not a registered panel", nameof(type));

    public static Type? Resolve(string key) =>
        byKey.TryGetValue(key, out Entry? entry) ? entry.Type : null;

    public static string KeyOf(Type type) => Get(type).Key;
}
