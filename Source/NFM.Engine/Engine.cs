global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Threading.Tasks;
global using System.Runtime.InteropServices;
global using System.IO;
global using System.Linq;
global using NFM.Common;
global using NFM.Mathematics;
global using NFM.Threading;
using NFM.Plugins;
using NFM.Resources;
using NFM.Graphics;
using NFM.World;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NFM")]

namespace NFM;

public static class Engine
{
	internal static void Init()
	{
		// Boot up renderer and load plugins.
		Renderer.Init();
		PluginSystem.Init();

		Project.OnProjectCreated += async () =>
		{
            // Populate scene with nodes for testing...

            var model = new ModelNode(null);
			model.Model = await Asset.LoadAsync<Model>("USER:/Objects/Heavy.glb");
            model.Scale = new Vector3(0.5f);

            await Task.Delay(1000);
			
			var model2 = new ModelNode(null);
			model2.Model = await Asset.LoadAsync<Model>("USER:/Objects/Spaceship.glb");
			model2.Scale = new Vector3(0.001f);
            model2.Position = new Vector3(1, -1.5f, 1);
		};
	}

	internal static void Update()
	{
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