using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.OpenGL;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using Avalonia.Utilities;
using Avalonia.Win32.Input;
using Avalonia.Win32.Interop;
using Engine.Platform;
using static Avalonia.Win32.Interop.UnmanagedMethods;

namespace Avalonia
{
    public static class Win32ApplicationExtensions
    {
        public static T UseWin32<T>(this T builder) where T : AppBuilderBase<T>, new()
        {
            return builder.UseWindowingSubsystem(() => Win32.Win32Platform.Initialize(AvaloniaLocator.Current.GetService<Win32PlatformOptions>() ?? new Win32PlatformOptions()), "Win32");
        }
    }

    /// <summary>
    /// Platform-specific options which apply to Windows.
    /// </summary>
    public class Win32PlatformOptions
    {
        /// <summary>
        /// Enables ANGLE for Windows. For every Windows version that is above Windows 7, the default is true otherwise it's false.
        /// </summary>
        /// <remarks>
        /// GPU rendering will not be enabled if this is set to false.
        /// </remarks>
        public bool? AllowEglInitialization { get; set; }
        
        public IList<string> EglRendererBlacklist { get; set; } = new List<string>
        {
            "Microsoft Basic Render"
        };

        /// <summary>
        /// Enables multitouch support. The default value is true.
        /// </summary>
        /// <remarks>
        /// Multitouch allows a surface (a touchpad or touchscreen) to recognize the presence of more than one point of contact with the surface at the same time.
        /// </remarks>
        public bool? EnableMultitouch { get; set; } = true;

        /// <summary>
        /// Embeds popups to the window when set to true. The default value is false.
        /// </summary>
        public bool OverlayPopups { get; set; }

        /// <summary>
        /// Avalonia would try to use native Widows OpenGL when set to true. The default value is false.
        /// </summary>
        public bool UseWgl { get; set; }

        public IList<GlVersion> WglProfiles { get; set; } = new List<GlVersion>
        {
            new GlVersion(GlProfileType.OpenGL, 4, 0),
            new GlVersion(GlProfileType.OpenGL, 3, 2),
        };
    }
}

namespace Avalonia.Win32
{
    public class Win32Platform : IPlatformThreadingInterface, IPlatformSettings, IWindowingPlatform, IPlatformIconLoader, IPlatformLifetimeEventsImpl
    {
        private static Thread uiThread;
        private UnmanagedMethods.WndProc wndProcDelegate;
        private readonly List<Delegate> delegateReferences = new List<Delegate>();

        public Win32Platform()
        {
            SetDpiAwareness();
            CreateMessageWindow();
        }

        public static Win32Platform Instance { get; } = new Win32Platform();
        public IntPtr Handle { get; set; }

        /// <summary>
        /// Gets the actual WindowsVersion. Same as the info returned from RtlGetVersion.
        /// </summary>
        public static Version WindowsVersion { get; } = RtlGetVersion();

        internal static bool UseOverlayPopups => Options.OverlayPopups;
        public static Win32PlatformOptions Options { get; private set; }

        public Size DoubleClickSize => new Size(
            UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CXDOUBLECLK),
            UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CYDOUBLECLK));

        public TimeSpan DoubleClickTime => TimeSpan.FromMilliseconds(UnmanagedMethods.GetDoubleClickTime());

        public static void Initialize()
        {
            Initialize(new Win32PlatformOptions());
        }

        public static void Initialize(Win32PlatformOptions options)
        {
            Options = options;
            AvaloniaLocator.CurrentMutable
                .Bind<IClipboard>().ToSingleton<ClipboardImpl>()
                .Bind<ICursorFactory>().ToConstant(CursorFactory.Instance)
                .Bind<IKeyboardDevice>().ToConstant(WindowsKeyboardDevice.Instance)
                .Bind<IPlatformSettings>().ToConstant(Instance)
                .Bind<IPlatformThreadingInterface>().ToConstant(Instance)
				.Bind<IRenderTimer>().ToConstant(new FastRenderTimer())
                .Bind<IRenderLoop>().ToConstant(new FastRenderLoop())
                .Bind<ISystemDialogImpl>().ToSingleton<SystemDialogImpl>()
                .Bind<IWindowingPlatform>().ToConstant(Instance)
                .Bind<PlatformHotkeyConfiguration>().ToConstant(new PlatformHotkeyConfiguration(KeyModifiers.Control)
                {
                    OpenContextMenu =
                    {
                        // Add Shift+F10
                        new KeyGesture(Key.F10, KeyModifiers.Shift)
                    }
                })
                .Bind<IPlatformIconLoader>().ToConstant(Instance)
                .Bind<NonPumpingLockHelper.IHelperImpl>().ToConstant(new NonPumpingSyncContext.HelperImpl())
                .Bind<IMountedVolumeInfoProvider>().ToConstant(new WindowsMountedVolumeInfoProvider())
                .Bind<IPlatformLifetimeEventsImpl>().ToConstant(Instance);

            Win32GlManager.Initialize();

            uiThread = Thread.CurrentThread;

            if (OleContext.Current != null)
                AvaloniaLocator.CurrentMutable.Bind<IPlatformDragSource>().ToSingleton<DragSource>();
        }

        public void RunLoop(CancellationToken cancellationToken)
        {
			while (!cancellationToken.IsCancellationRequested)
			{
				while (PeekMessage(out MSG msg, IntPtr.Zero, 0, 0, 0x0001))
				{
					TranslateMessage(ref msg);
					DispatchMessage(ref msg);
				}
			}
        }

		private List<(double, Stopwatch, Action)> timers = new();
		public void TimerTick()
		{
			for (int i = timers.Count - 1; i >= 0; i--)
			{
				var timer = timers[i];

				if (timer.Item2.Elapsed.TotalSeconds > timer.Item1 || timer.Item1 == 0d)
				{
					timer.Item3.Invoke();
					timer.Item2.Restart();
				}
			}
		}

        public IDisposable StartTimer(DispatcherPriority priority, TimeSpan interval, Action callback)
        {
			timers.Add(new(interval.TotalSeconds, Stopwatch.StartNew(), callback));

			return Disposable.Create(() =>
			{
				timers.Remove(timers.First(o => o.Item3 == callback && o.Item1 == interval.TotalSeconds));
			});
        }

        private static readonly int SignalW = unchecked((int)0xdeadbeaf);
        private static readonly int SignalL = unchecked((int)0x12345678);

        public void Signal(DispatcherPriority prio)
        {
            UnmanagedMethods.PostMessage(
                Handle,
                (int) UnmanagedMethods.WindowsMessage.WM_DISPATCH_WORK_ITEM,
                new IntPtr(SignalW),
                new IntPtr(SignalL));
        }

        public bool CurrentThreadIsLoopThread => uiThread == Thread.CurrentThread;

        public event Action<DispatcherPriority?> Signaled;

        public event EventHandler<ShutdownRequestedEventArgs> ShutdownRequested;

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using Win32 naming for consistency.")]
        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == (int) UnmanagedMethods.WindowsMessage.WM_DISPATCH_WORK_ITEM && wParam.ToInt64() == SignalW && lParam.ToInt64() == SignalL)
            {
                Signaled?.Invoke(null);
            }

            if(msg == (uint)WindowsMessage.WM_QUERYENDSESSION)
            {
                if (ShutdownRequested != null)
                {
                    var e = new ShutdownRequestedEventArgs();

                    ShutdownRequested(this, e);

                    if(e.Cancel)
                    {
                        return IntPtr.Zero;
                    }
                }
            }
            
            TrayIconImpl.ProcWnd(hWnd, msg, wParam, lParam);

            return UnmanagedMethods.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private void CreateMessageWindow()
        {
            // Ensure that the delegate doesn't get garbage collected by storing it as a field.
            wndProcDelegate = new UnmanagedMethods.WndProc(WndProc);

            UnmanagedMethods.WNDCLASSEX wndClassEx = new UnmanagedMethods.WNDCLASSEX
            {
                cbSize = Marshal.SizeOf<UnmanagedMethods.WNDCLASSEX>(),
                lpfnWndProc = wndProcDelegate,
                hInstance = UnmanagedMethods.GetModuleHandle(null),
                lpszClassName = "AvaloniaMessageWindow " + Guid.NewGuid(),
            };

            ushort atom = UnmanagedMethods.RegisterClassEx(ref wndClassEx);

            if (atom == 0)
            {
                throw new Win32Exception();
            }

            Handle = UnmanagedMethods.CreateWindowEx(0, atom, null, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            if (Handle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
        }

        public ITrayIconImpl CreateTrayIcon ()
        {
            return new TrayIconImpl();
        }

        public IWindowImpl CreateWindow()
        {
            return new WindowImpl();
        }

        public IWindowImpl CreateEmbeddableWindow()
        {
            var embedded = new EmbeddedWindowImpl();
            embedded.Show(true, false);
            return embedded;
        }

        public IWindowIconImpl LoadIcon(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return CreateIconImpl(stream);
            }
        }

        public IWindowIconImpl LoadIcon(Stream stream)
        {
            return CreateIconImpl(stream);
        }

        public IWindowIconImpl LoadIcon(IBitmapImpl bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream);
                return CreateIconImpl(memoryStream);
            }
        }

        private static IconImpl CreateIconImpl(Stream stream)
        {
            try
            {
                // new Icon() will work only if stream is an "ico" file.
                return new IconImpl(new System.Drawing.Icon(stream));
            }
            catch (ArgumentException)
            {
                // Fallback to Bitmap creation and converting into a windows icon. 
                using var icon = new System.Drawing.Bitmap(stream);
                var hIcon = icon.GetHicon();
                return new IconImpl(System.Drawing.Icon.FromHandle(hIcon));
            }
        }

        private static void SetDpiAwareness()
        {
            // Ideally we'd set DPI awareness in the manifest but this doesn't work for netcoreapp2.0
            // apps as they are actually dlls run by a console loader. Instead we have to do it in code,
            // but there are various ways to do this depending on the OS version.
            var user32 = LoadLibrary("user32.dll");
            var method = GetProcAddress(user32, nameof(SetProcessDpiAwarenessContext));

            if (method != IntPtr.Zero)
            {
                if (SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2) ||
                    SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE))
                {
                    return;
                }
            }

            var shcore = LoadLibrary("shcore.dll");
            method = GetProcAddress(shcore, nameof(SetProcessDpiAwareness));

            if (method != IntPtr.Zero)
            {
                SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE);
                return;
            }

            SetProcessDPIAware();
        }
    }
}
