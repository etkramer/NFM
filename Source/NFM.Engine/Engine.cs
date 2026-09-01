global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.IO;
global using System.Linq;
global using System.Runtime.InteropServices;
global using System.Threading.Tasks;
global using NFM.Common;
global using NFM.Mathematics;
global using NFM.Threading;
using System.Runtime.CompilerServices;
using NFM.Graphics;
using NFM.Plugins;

[assembly: InternalsVisibleTo("NFM")]

namespace NFM;

public static class Engine
{
	internal static void Init()
	{
		// Route anything awaited during startup through the dispatcher, not the thread pool.
		Dispatcher.Install();

		// Boot up renderer and load plugins.
		Renderer.Init();
		PluginSystem.Init();
	}

	internal static void Update()
	{
		// Refresh per-frame input state.
		Input.Update();

		// Dispatch any pending tasks.
        Dispatcher.Tick();

		// Render the frame.
		Renderer.RenderFrame();
	}

	internal static void Cleanup()
	{
		// Cleanup the renderer.
		Renderer.Cleanup();
	}
}