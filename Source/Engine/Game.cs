global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Threading.Tasks;
global using System.Runtime.InteropServices;
global using System.IO;
global using System.Linq;
global using Engine.Common;
global using Engine.Mathematics;
using Engine.GPU;
using Engine.Plugins;
using Engine.Resources;
using Engine.Rendering;
using Engine.World;

namespace Engine
{
	public static class Game
	{
		public static event Action<double> OnTick = delegate {};

		public static async Task Init()
		{
			// Boot up renderer
			Renderer.Init();

			// Load all plugins.
			await PluginSystem.LoadAll();

			// Kick off model loading early.
			_ = Asset.Load<Model>("USER:/Objects/Heavy.glb");

			Project.OnProjectCreated += OnProjectCreated;
		}

		public static void OnProjectCreated()
		{
			// Create example model
			var model = new ModelNode().Spawn() as ModelNode;
			model.Position = new Vector3(0, 0, 0);
			model.Rotation = new Vector3(0);
			model.Model = Asset.Load<Model>("USER:/Objects/Heavy.glb").Result;

			// If we need a GC, now's a good time.
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
		}

		public static void Update()
		{
			OnTick.Invoke(Graphics.FrameTime);

			// Invoke dispose queue.
			Queue.Invoke(0);

			// Render the frame.
			Renderer.Render();
		}

		public static void Cleanup()
		{
			// Cleanup the renderer.
			Renderer.Cleanup();
		}
	}
}