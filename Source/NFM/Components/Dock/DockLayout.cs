using System.Text.Json;
using System.Text.Json.Serialization;

namespace NFM.Components;

/// <summary>
/// A tree of split and tab-group nodes describing the editor layout, plus the operations that
/// rearrange it. Tab instances are stable across every operation, so the panels hosting them are
/// never torn down.
/// </summary>
public sealed class DockLayout
{
    public const double MinRatio = 0.05;

    public DockNode Root { get; private set; }

    /// <summary>
    /// Every open tab, in creation order. Panels render against this list, so its ordering must
    /// stay independent of the tree's shape.
    /// </summary>
    public IReadOnlyList<DockTab> Tabs => tabs;

    readonly List<DockTab> tabs = new();

    public event Action? Changed;

    public DockLayout(DockNode root)
    {
        Root = root;
        foreach (DockTabs group in Groups())
        {
            tabs.AddRange(group.Tabs);
        }
    }

    public IEnumerable<DockTabs> Groups() => Descend(Root).OfType<DockTabs>();

    static IEnumerable<DockNode> Descend(DockNode node)
    {
        yield return node;

        if (node is DockSplit split)
        {
            foreach (DockNode child in Descend(split.First))
            {
                yield return child;
            }

            foreach (DockNode child in Descend(split.Second))
            {
                yield return child;
            }
        }
    }

    public DockNode? FindNode(string id) => Descend(Root).FirstOrDefault(o => o.Id == id);

    public DockTab? FindTab(string id) => tabs.FirstOrDefault(o => o.Id == id);

    public DockTabs? OwnerOf(DockTab tab) => Groups().FirstOrDefault(o => o.Tabs.Contains(tab));

    public void NotifyChanged() => Changed?.Invoke();

    public void SetRatio(DockSplit split, double ratio)
    {
        split.Ratio = Math.Clamp(ratio, MinRatio, 1 - MinRatio);
        Changed?.Invoke();
    }

    public void Activate(DockTab tab)
    {
        if (OwnerOf(tab) is not DockTabs owner)
        {
            return;
        }

        owner.ActiveIndex = owner.Tabs.IndexOf(tab);
        Changed?.Invoke();
    }

    /// <summary>
    /// Opens a panel, focusing an existing tab for it if one is already open.
    /// </summary>
    public DockTab Open(Type panelType)
    {
        if (tabs.FirstOrDefault(o => o.PanelType == panelType) is DockTab existing)
        {
            Activate(existing);
            return existing;
        }

        DockTab tab = new() { PanelType = panelType };
        DockTabs target = Groups().FirstOrDefault() ?? Grow();

        tabs.Add(tab);
        target.Tabs.Add(tab);
        target.ActiveIndex = target.Tabs.Count - 1;

        Changed?.Invoke();
        return tab;
    }

    DockTabs Grow()
    {
        DockTabs group = new();
        Root = group;
        return group;
    }

    public void Close(DockTab tab)
    {
        if (OwnerOf(tab) is not DockTabs owner)
        {
            return;
        }

        tabs.Remove(tab);
        Remove(owner, tab);
        Prune(owner);

        Changed?.Invoke();
    }

    /// <summary>
    /// Moves a tab onto <paramref name="target"/>. A center drop appends to the target's tab strip
    /// at <paramref name="index"/>; any other zone splits the target and puts the tab in a new group
    /// on that side.
    /// </summary>
    public void Dock(DockTab tab, DockTabs target, DockZone zone, int index = -1)
    {
        DockTabs? owner = OwnerOf(tab);

        // Splitting a group off itself would just recreate the same layout.
        if (owner == target && zone is not DockZone.Center && target.Tabs.Count == 1)
        {
            return;
        }

        // The index came from a strip that still contained the tab, so removing it shifts anything
        // after its old position down by one.
        if (owner == target && index > owner.Tabs.IndexOf(tab))
        {
            index--;
        }

        if (owner is not null)
        {
            Remove(owner, tab);
        }

        if (zone is DockZone.Center)
        {
            int at = index < 0 || index > target.Tabs.Count ? target.Tabs.Count : index;
            target.Tabs.Insert(at, tab);
            target.ActiveIndex = at;
        }
        else
        {
            DockTabs group = new(tab);
            bool isFirst = zone is DockZone.Left or DockZone.Top;
            DockOrientation orientation =
                zone is DockZone.Left or DockZone.Right
                    ? DockOrientation.Horizontal
                    : DockOrientation.Vertical;

            // Captured up front: constructing the split reparents the target onto it.
            DockSplit? parent = target.Parent;
            bool wasFirst = parent?.First == target;

            DockSplit split = isFirst
                ? new DockSplit(orientation, group, target)
                : new DockSplit(orientation, target, group);

            Attach(parent, wasFirst, split);
        }

        if (owner is not null && owner != target)
        {
            Prune(owner);
        }

        Changed?.Invoke();
    }

    static void Remove(DockTabs owner, DockTab tab)
    {
        int removed = owner.Tabs.IndexOf(tab);
        owner.Tabs.RemoveAt(removed);
        owner.ActiveIndex = Math.Clamp(
            owner.ActiveIndex > removed ? owner.ActiveIndex - 1 : owner.ActiveIndex,
            0,
            Math.Max(owner.Tabs.Count - 1, 0)
        );
    }

    /// <summary>
    /// Collapses an emptied group, lifting its sibling into the split's place. The root is always
    /// kept, even when empty.
    /// </summary>
    void Prune(DockTabs group)
    {
        if (group.Tabs.Count > 0 || group.Parent is not DockSplit split)
        {
            return;
        }

        Replace(split, split.First == group ? split.Second : split.First);
    }

    void Replace(DockNode node, DockNode replacement) =>
        Attach(node.Parent, node.Parent?.First == node, replacement);

    void Attach(DockSplit? parent, bool asFirst, DockNode child)
    {
        if (parent is null)
        {
            Root = child;
            child.Parent = null;
        }
        else if (asFirst)
        {
            parent.First = child;
        }
        else
        {
            parent.Second = child;
        }
    }

    public string Serialize() => JsonSerializer.Serialize(ToDto(Root), SerializerOptions);

    public static DockLayout? Deserialize(string json)
    {
        NodeDto? dto = JsonSerializer.Deserialize<NodeDto>(json, SerializerOptions);
        DockNode? root = dto is null ? null : FromDto(dto);

        // A null result tells the caller to fall back to the default layout.
        return root is null ? null : new DockLayout(root);
    }

    static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    sealed record NodeDto
    {
        public string[]? Tabs { get; init; }
        public int Active { get; init; }

        public DockOrientation? Orientation { get; init; }
        public double Ratio { get; init; }
        public NodeDto? First { get; init; }
        public NodeDto? Second { get; init; }
    }

    static NodeDto ToDto(DockNode node) =>
        node switch
        {
            DockSplit split
                => new NodeDto
                {
                    Orientation = split.Orientation,
                    Ratio = split.Ratio,
                    First = ToDto(split.First),
                    Second = ToDto(split.Second)
                },
            DockTabs group
                => new NodeDto
                {
                    Tabs = group.Tabs.Select(o => PanelRegistry.KeyOf(o.PanelType)).ToArray(),
                    Active = group.ActiveIndex
                },
            _ => throw new NotSupportedException(node.GetType().Name)
        };

    static DockNode? FromDto(NodeDto dto)
    {
        if (dto.Orientation is DockOrientation orientation && dto.First is not null && dto.Second is not null)
        {
            DockNode? first = FromDto(dto.First);
            DockNode? second = FromDto(dto.Second);

            // A side that resolved to nothing collapses into its sibling.
            if (first is null || second is null)
            {
                return first ?? second;
            }

            return new DockSplit(orientation, first, second, Math.Clamp(dto.Ratio, MinRatio, 1 - MinRatio));
        }

        DockTab[] resolved = (dto.Tabs ?? [])
            .Select(PanelRegistry.Resolve)
            .OfType<Type>()
            .Select(o => new DockTab { PanelType = o })
            .ToArray();

        if (resolved.Length is 0)
        {
            return null;
        }

        return new DockTabs(resolved)
        {
            ActiveIndex = Math.Clamp(dto.Active, 0, resolved.Length - 1)
        };
    }
}
