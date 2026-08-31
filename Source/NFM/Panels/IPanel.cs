using Microsoft.AspNetCore.Components;

namespace NFM.Components;

public interface IPanel : IComponent
{
    static abstract string Name { get; }

    /// <summary>
    /// True when the panel composites against something behind the page, and the dock frame
    /// must not paint over that area.
    /// </summary>
    static virtual bool IsTransparent => false;
}
