using Microsoft.AspNetCore.Components;

namespace NFM.Components;

public partial class FontIcon
{
    [Parameter, EditorRequired]
    public required string Glyph { get; set; }

    [Parameter]
    public int FontSize { get; set; } = 12;

    [Parameter]
    public string FontFamily { get; set; } = "Material Icons";

    [Parameter]
    public string Color { get; set; } = "unset";
}
