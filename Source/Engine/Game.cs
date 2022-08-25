global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Threading.Tasks;
global using System.Runtime.InteropServices;
global using System.IO;
global using System.Linq;
global using Engine.Core;
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
		public static void Init()
		{
			// Init basic systems.
			GPUContext.Init();
			Renderer.Init();

			// Load all plugins.
			PluginSystem.LoadAll();

			// Precache model.
			Asset.GetAsync<Model>("USER:Objects/FlightHelmet").Wait();

			Project.OnProjectCreated += OnProjectCreated;
		}

		public static void OnProjectCreated()
		{
			new Actor("Empty").Spawn();
			new PointLightActor().Spawn();
			new CameraActor().Spawn();

			var helmObject = new ModelActor("Flight Helmet")
				.Spawn<ModelActor>();
			helmObject.Position = new Vector3(0, -0.4f, -1);
			helmObject.Rotation = new Vector3(0, 25, 0);
			helmObject.Model = Asset.GetAsync<Model>("USER:Objects/FlightHelmet").Result;

			// Try to make sure the big GC happens *before* we start presenting.
			GC.Collect(2, GCCollectionMode.Forced, true);
		}

		public static void Update()
		{
			// Render the frame.
			Renderer.Render();

			// Run the dispose queue.
			Queue.Run(-1);
		}

		public static void Cleanup()
		{
			// Cleanup the renderer.
			Renderer.Cleanup();

			// Run the dispose queue.
			Queue.Run(-1);
		}
	}
}