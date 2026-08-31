using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace NFM.Components;

public sealed class DockManager : ComponentBase
{
    [Parameter, EditorRequired]
    public required string ConfigPath { get; set; }

    [Parameter, EditorRequired]
    public required Layout DefaultLayout { get; set; }

    public sealed record class Layout
    {
        public required string[] Tabs { get; set; }
        public double Split { get; set; }

        public Layout? Left { get; set; }
        public Layout? Right { get; set; }
        public Layout? Top { get; set; }
        public Layout? Bottom { get; set; }
    }

    Layout? rootLayout;

    public void SaveToDisk()
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "..", this.ConfigPath);
        var configDirectory = Guard.NotNull(Path.GetDirectoryName(configPath));
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }

        using (var stream = File.Open(configPath, FileMode.Create, FileAccess.Write))
        using (var writer = new StreamWriter(stream))
        {
            var json = JsonSerializer.Serialize(this.rootLayout, SerializerOptions);
            writer.Write(json);
        }
    }

    static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "..", this.ConfigPath);
        this.rootLayout ??= File.Exists(configPath)
            ? JsonSerializer.Deserialize<Layout>(File.ReadAllText(configPath), SerializerOptions)
            : this.DefaultLayout;

        BuildRenderTree(builder, Guard.NotNull(this.rootLayout));
    }

    static void BuildRenderTree(RenderTreeBuilder builder, Layout layout)
    {
        builder.OpenComponent<CascadingValue<Layout>>(GetSequence());
        builder.AddAttribute(GetSequence(), "Value", layout);
        builder.AddAttribute(
            GetSequence(),
            "ChildContent",
            (RenderFragment)(
                (RenderTreeBuilder builder) =>
                {
                    builder.OpenComponent<DockPanel>(GetSequence());
                    {
                        builder.AddAttribute(
                            GetSequence(),
                            "Content",
                            (RenderFragment)(
                                (RenderTreeBuilder builder) =>
                                {
                                    builder.OpenComponent<DockGroup>(GetSequence());
                                    builder.AddAttribute(
                                        GetSequence(),
                                        "ChildContent",
                                        (RenderFragment)(
                                            (RenderTreeBuilder builder) =>
                                            {
                                                foreach (var typeName in layout.Tabs)
                                                {
                                                    var panelType = Guard.NotNull(
                                                        Type.GetType(typeName)
                                                    );
                                                    var itemType =
                                                        typeof(DockGroupItem<>).MakeGenericType(
                                                            panelType
                                                        );

                                                    builder.OpenComponent(GetSequence(), itemType);
                                                    builder.CloseComponent();
                                                }
                                            }
                                        )
                                    );
                                    builder.CloseComponent();
                                }
                            )
                        );

                        if (layout.Left is not null)
                        {
                            builder.AddAttribute(
                                GetSequence(),
                                "Left",
                                (RenderFragment)(
                                    (RenderTreeBuilder builder) =>
                                        BuildRenderTree(builder, layout.Left)
                                )
                            );
                        }

                        if (layout.Right is not null)
                        {
                            builder.AddAttribute(
                                GetSequence(),
                                "Right",
                                (RenderFragment)(
                                    (RenderTreeBuilder builder) =>
                                        BuildRenderTree(builder, layout.Right)
                                )
                            );
                        }

                        if (layout.Top is not null)
                        {
                            builder.AddAttribute(
                                GetSequence(),
                                "Top",
                                (RenderFragment)(
                                    (RenderTreeBuilder builder) =>
                                        BuildRenderTree(builder, layout.Top)
                                )
                            );
                        }

                        if (layout.Bottom is not null)
                        {
                            builder.AddAttribute(
                                GetSequence(),
                                "Bottom",
                                (RenderFragment)(
                                    (RenderTreeBuilder builder) =>
                                        BuildRenderTree(builder, layout.Bottom)
                                )
                            );
                        }
                    }
                    builder.CloseComponent();
                }
            )
        );
        builder.CloseComponent();
    }

    // This is similar to what the razor compiler does per
    // https://learn.microsoft.com/en-us/aspnet/core/blazor/advanced-scenarios#guidance-and-conclusions
    static int GetSequence([CallerLineNumber] int lineNumber = 0) => lineNumber;
}
