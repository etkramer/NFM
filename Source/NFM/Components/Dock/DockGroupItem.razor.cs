namespace NFM.Components;

public interface IDockGroupItem
{
    public string Name { get; }

    /// <summary>
    /// True when the panel composites against something behind the page, and the dock frame
    /// must not paint over that area.
    /// </summary>
    public bool IsTransparent { get; }
}
