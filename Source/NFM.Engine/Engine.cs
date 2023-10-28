global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Threading.Tasks;
global using System.Runtime.InteropServices;
global using System.IO;
global using System.Linq;
global using NFM.Common;
global using NFM.Mathematics;
using NFM.Threading;
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

		// Kick off model loading early.
		_ = Asset.LoadAsync<Model>("USER:/Objects/Spaceship.glb");

		// Create default nodes with new project.
		Project.OnProjectCreated += async () =>
		{
			// Create example model
			var model = new ModelNode(null);
			model.Model = await Asset.LoadAsync<Model>("USER:/Objects/Spaceship.glb");
			model.Scale = new Vector3(0.01f, 0.01f, 0.01f);
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