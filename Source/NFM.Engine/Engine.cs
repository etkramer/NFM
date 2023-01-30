global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Threading.Tasks;
global using System.Runtime.InteropServices;
global using System.IO;
global using System.Linq;
global using NFM.Common;
global using NFM.Mathematics;
using NFM.GPU;
using NFM.Plugins;
using NFM.Resources;
using NFM.Graphics;
using NFM.World;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NFM")]

namespace NFM;

public static class Engine
{
	public static event Action<double> OnTick = delegate {};

	internal static void Init()
	{
		// Boot up renderer and load plugins.
		Renderer.Init();
		PluginSystem.Init();

		// Kick off model loading early.
		_ = Asset.LoadAsync<Model>("USER:/Objects/Heavy.glb");

		// Create default nodes with new project.
		Project.OnProjectCreated += async () =>
		{
			// Create example model
			var model = new ModelNode(null);
			model.Model = await Asset.LoadAsync<Model>("USER:/Objects/Heavy.glb");

			/*for (int i = 0; i < 100000; i++)
			{
				Node node = new Node(null);
			}*/

			Selection.Select(model);
		};
	}

	internal static void Update()
	{
		// Begin the new frame.
		OnTick.Invoke(Metrics.FrameTime);

		// Render the frame.
		Renderer.RenderFrame();
	}

	internal static void Cleanup()
	{
		// Cleanup the renderer.
		Renderer.Cleanup();
	}
}