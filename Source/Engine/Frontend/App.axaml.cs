using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Engine.Frontend.Controls;

namespace Engine.Frontend
{
	public partial class App : Application
	{
		public static App Instance;

		public static void Main() => Run();

		public static void Run()
		{
			Win32PlatformOptions opts = new()
			{
				AllowEglInitialization = false,
				UseWgl = true,
			};

			AppBuilder.Configure<App>().UseWin32().With(opts).UseSkia().StartWithClassicDesktopLifetime(new string[0]);
			Game.Cleanup();
		}

		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
			Instance = this;
		}
		
		public override void OnFrameworkInitializationCompleted()
		{
			var desktopLifetime = ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
			desktopLifetime.ShutdownMode = ShutdownMode.OnMainWindowClose;

			// Show splash screen.
			SplashScreen splash = new SplashScreen();
			desktopLifetime.MainWindow = splash;

			Task startupTask = Task.Run(Game.Init);
			startupTask.ContinueWith((t) =>
				{
					if (t.IsFaulted)
					{
						// Capture stack trace.
						ExceptionDispatchInfo info = ExceptionDispatchInfo.Capture(t.Exception.InnerException);

						// Create exception dialog.
						Dispatcher.UIThread.Post(() => 
							new Popup(
									info.SourceException.GetType().Name,
									$"An unhandled exception has occured. If you wish to debug this event further, select Break. Otherwise, select Abort to end the program.\n" +
									$"{info.SourceException.GetType().Name}: {info.SourceException.Message}\n" +
									$"{info.SourceException.StackTrace}")
								.Button("Break", () => info.Throw())
								.Button("Abort", () => Environment.Exit(-1)).Open()
						);
					}
					else
					{
						Dispatcher.UIThread.Post(() =>
						{
							desktopLifetime.MainWindow = new MainWindow(); desktopLifetime.MainWindow.Show(); splash.Close();
						});
					}
				});
		}

		public void Shutdown()
		{
			(ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).TryShutdown();
		}
	}
}