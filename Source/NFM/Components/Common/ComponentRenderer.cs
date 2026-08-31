using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace NFM.Components;

public sealed class ComponentRenderer : ComponentBase
{
	[Parameter, EditorRequired]
	public required Type Type { get; set; }

	[Parameter(CaptureUnmatchedValues = true)]
	public IDictionary<string, object>? AdditionalAttributes { get; set; }

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenComponent(1, Type);

		if (AdditionalAttributes is not null)
		{
			builder.AddMultipleAttributes(2, AdditionalAttributes);
		}

		builder.CloseComponent();
	}
}
