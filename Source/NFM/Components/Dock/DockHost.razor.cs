using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace NFM.Components;

public sealed partial class DockHost : IAsyncDisposable
{
    const int SaveDelayMs = 500;

    [Parameter, EditorRequired]
    public required string ConfigPath { get; set; }

    [Parameter, EditorRequired]
    public required Func<DockLayout> DefaultLayout { get; set; }

    [Inject]
    public required IJSRuntime JS { get; set; }

    public DockLayout Layout => layout ??= Load();

    DockLayout? layout;
    ElementReference rootElement;

    IJSObjectReference? module;
    DotNetObjectReference<DockHost>? selfReference;

    int saveGeneration;

    public DockHost()
    {
        ShouldRenderOnEvent = true;
    }

    string ResolvedConfigPath => Path.Combine(AppContext.BaseDirectory, "..", ConfigPath);

    DockLayout Load()
    {
        DockLayout? loaded = null;

        try
        {
            if (File.Exists(ResolvedConfigPath))
            {
                loaded = DockLayout.Deserialize(File.ReadAllText(ResolvedConfigPath));
            }
        }
        catch (Exception exception)
        {
            Log.Warn($"Failed to load dock layout: {exception.Message}");
        }

        DockLayout result = loaded ?? DefaultLayout();
        result.Changed += OnLayoutChanged;

        return result;
    }

    void OnLayoutChanged()
    {
        _ = InvokeAsync(StateHasChanged);
        _ = SaveLater();
    }

    /// <summary>
    /// Writes the layout to disk. Drags produce a burst of changes, so the write is debounced.
    /// </summary>
    async Task SaveLater()
    {
        int generation = ++saveGeneration;
        await Task.Delay(SaveDelayMs);

        if (generation != saveGeneration)
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(Guard.NotNull(Path.GetDirectoryName(ResolvedConfigPath)));
            await File.WriteAllTextAsync(ResolvedConfigPath, Layout.Serialize());
        }
        catch (Exception exception)
        {
            Log.Warn($"Failed to save dock layout: {exception.Message}");
        }
    }

    public void Activate(DockTab tab) => Layout.Activate(tab);

    public void Close(DockTab tab) => Layout.Close(tab);

    public void Open(Type panelType) => Layout.Open(panelType);

    [JSInvokable]
    public void OnClosed(string tabId)
    {
        if (Layout.FindTab(tabId) is DockTab tab)
        {
            Layout.Close(tab);
        }
    }

    [JSInvokable]
    public void OnResized(string nodeId, double ratio)
    {
        if (Layout.FindNode(nodeId) is DockSplit split)
        {
            // The frame already carries this ratio, so no re-render is needed.
            split.Ratio = Math.Clamp(ratio, DockLayout.MinRatio, 1 - DockLayout.MinRatio);
            _ = SaveLater();
        }
    }

    /// <summary>
    /// Interop serialization has no string enum converter, so the zone arrives as a name.
    /// </summary>
    [JSInvokable]
    public void OnDropped(string tabId, string nodeId, string zone, int index)
    {
        if (
            Enum.TryParse(zone, out DockZone parsed)
            && Layout.FindTab(tabId) is DockTab tab
            && Layout.FindNode(nodeId) is DockTabs target
        )
        {
            Layout.Dock(tab, target, parsed, index);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            selfReference = DotNetObjectReference.Create(this);
            module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/dock.js");
            await module.InvokeVoidAsync("init", rootElement, selfReference);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public async ValueTask DisposeAsync()
    {
        if (layout is not null)
        {
            layout.Changed -= OnLayoutChanged;
        }

        if (module is not null)
        {
            await module.InvokeVoidAsync("dispose");
            await module.DisposeAsync();
        }

        selfReference?.Dispose();
        Dispose();
    }
}
