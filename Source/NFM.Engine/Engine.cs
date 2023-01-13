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
using NFM.Rendering;
using NFM.World;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NFM")]

namespace NFM
{
	public static class Engine
	{
		public static event Action<double> OnTick = delegate {};

		public static void Init()
		{
			// Boot up renderer and load plugins.
			Renderer.Init();
			PluginSystem.Init();

			// Kick off model loading early.
			_ = Asset.LoadAsync<Model>("USER:/Objects/TransmissionTest.glb");

			// Create default nodes with new project.
			Project.OnProjectCreated += () =>
			{
				// Create example model
				var model = new ModelNode(null);
				model.Position = new Vector3(0);
				model.Rotation = new Vector3(0);
				model.Model = Asset.LoadAsync<Model>("USER:/Objects/TransmissionTest.glb").Result;
			};
		}

		public static void Update()
		{
			// Begin the new frame.
			OnTick.Invoke(Metrics.FrameTime);

			// Tick scenes.
			foreach (var scene in Scene.All)
			{
				scene.Tick();
			}

			// Render the frame.
			Renderer.RenderFrame();
		}

		public static void Cleanup()
		{
			// Cleanup the renderer.
			Renderer.Cleanup();
		}
	}
}