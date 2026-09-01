namespace NFM.Components;

public enum DockOrientation
{
    Horizontal,
    Vertical
}

/// <summary>
/// Where a dragged tab lands relative to an existing group.
/// </summary>
public enum DockZone
{
    Center,
    Left,
    Right,
    Top,
    Bottom
}

public abstract class DockNode
{
    public string Id { get; } = Guid.NewGuid().ToString("N");

    public DockSplit? Parent { get; internal set; }
}

public sealed class DockSplit : DockNode
{
    public DockOrientation Orientation { get; set; }

    /// <summary>
    /// Share of the available space given to <see cref="First"/>, in 0..1.
    /// </summary>
    public double Ratio { get; set; } = 0.5;

    DockNode first = null!;
    DockNode second = null!;

    public DockNode First
    {
        get => first;
        set
        {
            first = value;
            value.Parent = this;
        }
    }

    public DockNode Second
    {
        get => second;
        set
        {
            second = value;
            value.Parent = this;
        }
    }

    public DockSplit(DockOrientation orientation, DockNode first, DockNode second, double ratio = 0.5)
    {
        Orientation = orientation;
        Ratio = ratio;
        First = first;
        Second = second;
    }
}

public sealed class DockTabs : DockNode
{
    public List<DockTab> Tabs { get; } = [];

    public int ActiveIndex { get; set; }

    public DockTab? Active =>
        ActiveIndex >= 0 && ActiveIndex < Tabs.Count ? Tabs[ActiveIndex] : null;

    public DockTabs() { }

    public DockTabs(params DockTab[] tabs) => Tabs.AddRange(tabs);
}

public sealed class DockTab
{
    public string Id { get; } = Guid.NewGuid().ToString("N");

    public required Type PanelType { get; init; }

    public string Name => PanelRegistry.Get(PanelType).Name;

    public bool IsTransparent => PanelRegistry.Get(PanelType).IsTransparent;

    public static DockTab Of<TPanel>() where TPanel : IPanel => new() { PanelType = typeof(TPanel) };
}
