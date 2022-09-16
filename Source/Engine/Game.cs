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
using System.Threading;

namespace Engine
{
	public static class Game
	{
		public static event Action<double> OnTick = delegate {};

		public static async Task Init()
		{
			// Init basic systems.
			GPUContext.Init();
			Renderer.Init();

			// Load all plugins.
			PluginSystem.LoadAll();

			// Kick off model loading early.
			var loadTask = Asset.Load<Model>("USER:Objects/Heavy.glb");

			Project.OnProjectCreated += OnProjectCreated;
		}

		public static void OnProjectCreated()
		{
			// Spawn actors
			new CameraActor().Spawn();
			new PointLightActor().Spawn();
			var modelObject = new ModelActor().Spawn<ModelActor>();

			modelObject.Position = new Vector3(0, -0.4f, 1);
			modelObject.Rotation = new Vector3(0, 180, 0);
			modelObject.Model = Asset.Load<Model>("USER:Objects/Heavy.glb").Result;

			// If we need a GC, now's a good time.
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
		}

		public static void Update()
		{
			OnTick.Invoke(Graphics.FrameTime);

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