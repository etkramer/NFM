using System.Drawing;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using NFM.Components;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.DirectComposition;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.WindowsAndMessaging;

namespace NFM.Hosting;

interface IComponentForm : IContainerControl
{
	bool UseContentSize { get; }

	int ClientWidth { set; }
	int ClientHeight { set; }

	void Show();
}

/// <summary>
/// A window hosting a Blazor component, composited over any 3D viewports the page asks for.
/// </summary>
class ComponentForm<TComponent> : Form, IComponentForm where TComponent : IComponent
{
	/// <summary>
	/// When set, the page drives the window size instead of the other way around.
	/// </summary>
	public bool UseContentSize { get; init; }

	public int ClientWidth { set => ClientSize = new Size(value, ClientSize.Height); }
	public int ClientHeight { set => ClientSize = new Size(ClientSize.Width, value); }

	private IDCompositionDevice device = null!;
	private IDCompositionTarget target = null!;
	private IDCompositionVisual rootVisual = null!;
	private IDCompositionVisual viewportsVisual = null!;
	private IDCompositionVisual webviewVisual = null!;

	private CoreWebView2CompositionController? controller;
	private CompositionWebViewManager? manager;
	private ServiceProvider? services;

	private readonly Dictionary<int, ViewportHost> viewports = [];
	private ViewportHost? hovered;
	private ViewportHost? captured;

	protected float DpiScale => DeviceDpi / 96f;

	public ComponentForm()
	{
		Text = "NFM";
		Icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath!);
		BackColor = ColorTranslator.FromHtml("#1a1a1a");
		StartPosition = FormStartPosition.CenterScreen;
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);

		SetDarkTitleBar();
		InitComposition();

		_ = InitWebview();
	}

	protected override void OnLoad(EventArgs e)
	{
		if (UseContentSize)
		{
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
		}
		else
		{
			int height = (int)((Screen.PrimaryScreen?.Bounds.Height ?? 1080) * 0.95);
			ClientSize = new Size((int)(height * (16 / 9d)), height);

			CenterToScreen();
		}

		base.OnLoad(e);
	}

	/// <summary>
	/// Shows the window as a modal dialog, blocking its owner until it closes.
	/// </summary>
	public Task<DialogResult> ShowDialogAsync(Form? owner = null)
	{
		TaskCompletionSource<DialogResult> completion = new();
		owner ??= Program.MainForm;

		PInvoke.SetWindowLongPtr((HWND)Handle, WINDOW_LONG_PTR_INDEX.GWLP_HWNDPARENT, owner.Handle);

		void OnShownCallback(object? sender, EventArgs e)
		{
			Shown -= OnShownCallback;

			MethodInfo runDialog = typeof(Application).GetMethod("RunDialog", BindingFlags.NonPublic | BindingFlags.Static)!;
			runDialog.Invoke(null, [this]);

			completion.SetResult(DialogResult);
		}

		Shown += OnShownCallback;
		Show();

		return completion.Task;
	}

	private unsafe void SetDarkTitleBar()
	{
		int enabled = 1;
		PInvoke.DwmSetWindowAttribute((HWND)Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &enabled, sizeof(int));
	}

	private unsafe void InitComposition()
	{
		Guid iid = typeof(IDCompositionDevice).GUID;
		PInvoke.DCompositionCreateDevice(null, &iid, out object deviceObj).ThrowOnFailure();
		device = (IDCompositionDevice)deviceObj;

		device.CreateTargetForHwnd((HWND)Handle, true, out target);
		device.CreateVisual(out rootVisual);
		target.SetRoot(rootVisual);

		device.CreateVisual(out viewportsVisual);
		device.CreateVisual(out webviewVisual);

		// Viewports underneath, webview above them - HTML has to be able to draw over the 3D.
		rootVisual.AddVisual(viewportsVisual, false, null);
		rootVisual.AddVisual(webviewVisual, true, viewportsVisual);

		device.Commit();
	}

	private async Task InitWebview()
	{
		string userData = Path.Combine(Path.GetTempPath(), "NFM", "WebView2");
		string contentRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");

		CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, userData,
			new CoreWebView2EnvironmentOptions("--disable-smooth-scrolling --autoplay-policy=no-user-gesture-required"));

		controller = await environment.CreateCoreWebView2CompositionControllerAsync(Handle);
		controller.RootVisualTarget = webviewVisual;
		controller.DefaultBackgroundColor = System.Drawing.Color.Transparent;
		controller.ShouldDetectMonitorScaleChanges = false;
		controller.RasterizationScale = DpiScale;
		controller.Bounds = new Rectangle(Point.Empty, ClientSize);
		controller.IsVisible = true;

		// Composition hosting leaves the cursor to the host - the webview only reports what it wants.
		controller.CursorChanged += (o, e) => Cursor = new Cursor(controller.Cursor);

		CoreWebView2 webview = controller.CoreWebView2;
		CoreWebView2Settings settings = webview.Settings;
		settings.AreDevToolsEnabled = Common.Debug.IsDebugBuild;
		settings.AreBrowserAcceleratorKeysEnabled = false;
		settings.AreDefaultContextMenusEnabled = false;
		settings.AreDefaultScriptDialogsEnabled = false;
		settings.IsZoomControlEnabled = false;
		settings.IsStatusBarEnabled = false;
		settings.IsPasswordAutosaveEnabled = false;
		settings.IsGeneralAutofillEnabled = false;

		webview.DocumentTitleChanged += (o, e) => Text = webview.DocumentTitle;

		ServiceCollection collection = new();
		collection.AddWindowsFormsBlazorWebView();
		collection.AddSingleton<Form>(this);
		services = collection.BuildServiceProvider();

		manager = await CompositionWebViewManager.CreateAsync(webview, environment, services,
			new FormDispatcher(this), contentRoot, "index.html");
		manager.OnHostMessage += OnHostMessage;

		await manager.AddRootComponentAsync(typeof(RootHelperComponent<TComponent>), "#app-root", ParameterView.Empty);
		await manager.AddRootComponentAsync(typeof(Microsoft.AspNetCore.Components.Web.HeadOutlet), "head::after", ParameterView.Empty);
		manager.Navigate("/");

		device.Commit();
		controller.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);
	}

	// The first request to show comes from Application.Run, before there's anything to look at.
	// RootHelperComponent calls Show() again once the page has rendered.
	private bool hasSuppressedSetVisible;

	protected override void SetVisibleCore(bool value)
	{
		base.SetVisibleCore(hasSuppressedSetVisible && value);
		hasSuppressedSetVisible = true;
	}

	#region Viewports

	private void OnHostMessage(string json)
	{
		using JsonDocument document = JsonDocument.Parse(json);
		JsonElement root = document.RootElement;

		switch (root.GetProperty("kind").GetString())
		{
			case "viewportRect":
				SetViewportRect(root.GetProperty("id").GetInt32(), ToPhysical(root));
				break;
			case "viewportRemoved":
				RemoveViewport(root.GetProperty("id").GetInt32());
				break;
			case "viewportHover":
				JsonElement id = root.GetProperty("id");
				hovered = id.ValueKind == JsonValueKind.Null ? null : viewports.GetValueOrDefault(id.GetInt32());
				break;
			case "key":
				Keys key = ParseKey(root.GetProperty("code").GetString());
				bool down = root.GetProperty("down").GetBoolean();

				if (key is Keys.F12 && down && Common.Debug.IsDebugBuild)
				{
					controller?.CoreWebView2.OpenDevToolsWindow();
				}

				Input.UpdateKey(key, down);
				break;
		}
	}

	private Rectangle ToPhysical(JsonElement rect) => new(
		(int)(rect.GetProperty("x").GetDouble() * DpiScale),
		(int)(rect.GetProperty("y").GetDouble() * DpiScale),
		(int)(rect.GetProperty("w").GetDouble() * DpiScale),
		(int)(rect.GetProperty("h").GetDouble() * DpiScale));

	private void SetViewportRect(int id, Rectangle rect)
	{
		if (rect.Width < 2 || rect.Height < 2)
		{
			return;
		}

		Vector2i position = new(rect.X, rect.Y);
		Vector2i size = new(rect.Width, rect.Height);

		if (!viewports.TryGetValue(id, out ViewportHost? host))
		{
			host = new ViewportHost(id, device, viewportsVisual, size);
			viewports.Add(id, host);
		}

		host.SetRect(position, size);
		device.Commit();
	}

	private void RemoveViewport(int id)
	{
		if (!viewports.Remove(id, out ViewportHost? host))
		{
			return;
		}

		if (hovered == host) hovered = null;
		if (captured == host) captured = null;

		host.Dispose();
		device.Commit();
	}

	#endregion

	#region Input

	// The webview is the topmost visual, so it gets every mouse event. We read the same events on the
	// way past to drive the engine, using the hover the page reports to decide which viewport owns them.
	private void SendMouse(CoreWebView2MouseEventKind kind, MouseEventArgs e, uint data = 0)
	{
		controller?.SendMouseInput(kind, VirtualKeys(e), data, new Point(e.X, e.Y));

		ViewportHost? active = captured ?? hovered;
		Vector2i position = new(e.X, e.Y);

		Input.UpdateMouse(new Vector2(e.X, e.Y),
			Control.MouseButtons.HasFlag(MouseButtons.Left),
			Control.MouseButtons.HasFlag(MouseButtons.Right),
			Control.MouseButtons.HasFlag(MouseButtons.Middle),
			active);

		foreach (ViewportHost host in viewports.Values)
		{
			host.SetCursor(host == active ? position : null);
		}
	}

	private static CoreWebView2MouseEventVirtualKeys VirtualKeys(MouseEventArgs e)
	{
		CoreWebView2MouseEventVirtualKeys keys = CoreWebView2MouseEventVirtualKeys.None;

		if (Control.MouseButtons.HasFlag(MouseButtons.Left)) keys |= CoreWebView2MouseEventVirtualKeys.LeftButton;
		if (Control.MouseButtons.HasFlag(MouseButtons.Right)) keys |= CoreWebView2MouseEventVirtualKeys.RightButton;
		if (Control.MouseButtons.HasFlag(MouseButtons.Middle)) keys |= CoreWebView2MouseEventVirtualKeys.MiddleButton;
		if (ModifierKeys.HasFlag(Keys.Control)) keys |= CoreWebView2MouseEventVirtualKeys.Control;
		if (ModifierKeys.HasFlag(Keys.Shift)) keys |= CoreWebView2MouseEventVirtualKeys.Shift;

		return keys;
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		SendMouse(CoreWebView2MouseEventKind.Move, e);
		base.OnMouseMove(e);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		Focus();
		controller?.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);

		captured ??= hovered;
		Capture = true;

		captured?.OnPress(e.Button == System.Windows.Forms.MouseButtons.Left);

		SendMouse(e.Button switch
		{
			MouseButtons.Right => CoreWebView2MouseEventKind.RightButtonDown,
			MouseButtons.Middle => CoreWebView2MouseEventKind.MiddleButtonDown,
			_ => CoreWebView2MouseEventKind.LeftButtonDown,
		}, e);

		base.OnMouseDown(e);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		SendMouse(e.Button switch
		{
			MouseButtons.Right => CoreWebView2MouseEventKind.RightButtonUp,
			MouseButtons.Middle => CoreWebView2MouseEventKind.MiddleButtonUp,
			_ => CoreWebView2MouseEventKind.LeftButtonUp,
		}, e);

		captured?.OnRelease(e.Button == System.Windows.Forms.MouseButtons.Left);

		if (Control.MouseButtons == System.Windows.Forms.MouseButtons.None)
		{
			Capture = false;
			captured = null;
		}

		base.OnMouseUp(e);
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		SendMouse(CoreWebView2MouseEventKind.Wheel, e, (uint)e.Delta);
		base.OnMouseWheel(e);
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		controller?.SendMouseInput(CoreWebView2MouseEventKind.Leave, CoreWebView2MouseEventVirtualKeys.None, 0, Point.Empty);
		base.OnMouseLeave(e);
	}

	protected override void OnDeactivate(EventArgs e)
	{
		Input.ReleaseAll();
		base.OnDeactivate(e);
	}

	/// <summary>
	/// Maps a DOM KeyboardEvent.code, which names the physical key rather than the character it produces.
	/// </summary>
	private static Keys ParseKey(string? code) => code switch
	{
		null => Keys.None,
		['K', 'e', 'y', char letter] => Enum.Parse<Keys>(letter.ToString()),
		['D', 'i', 'g', 'i', 't', char digit] => Enum.Parse<Keys>($"D{digit}"),
		"ShiftLeft" => Keys.LShiftKey,
		"ShiftRight" => Keys.RShiftKey,
		"ControlLeft" => Keys.LControlKey,
		"ControlRight" => Keys.RControlKey,
		"AltLeft" => Keys.LMenu,
		"AltRight" => Keys.RMenu,
		"ArrowLeft" => Keys.Left,
		"ArrowRight" => Keys.Right,
		"ArrowUp" => Keys.Up,
		"ArrowDown" => Keys.Down,
		"Backspace" => Keys.Back,
		"Minus" => Keys.OemMinus,
		"Equal" => Keys.Oemplus,
		"BracketLeft" => Keys.OemOpenBrackets,
		"BracketRight" => Keys.OemCloseBrackets,
		"Backslash" => Keys.OemPipe,
		"Semicolon" => Keys.OemSemicolon,
		"Quote" => Keys.OemQuotes,
		"Comma" => Keys.Oemcomma,
		"Period" => Keys.OemPeriod,
		"Slash" => Keys.OemQuestion,
		"Backquote" => Keys.Oemtilde,
		_ => Enum.TryParse(code, out Keys key) ? key : Keys.None,
	};

	#endregion

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);

		if (controller is not null)
		{
			controller.Bounds = new Rectangle(Point.Empty, ClientSize);
		}
	}

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		foreach (ViewportHost host in viewports.Values)
		{
			host.Dispose();
		}

		viewports.Clear();

		controller?.Close();
		services?.Dispose();

		base.OnFormClosed(e);
	}
}
