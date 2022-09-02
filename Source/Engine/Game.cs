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
		public static event Action<double> OnTick = delegate {};

		public static async Task Init()
		{
			// Init basic systems.
			GPUContext.Init();
			Renderer.Init();

			// Load all plugins.
			PluginSystem.LoadAll();

			// Kick off model loading. We're not awaiting this right now because
			// opening the window takes long enough to hide the load time.
			var loadSponza = Asset.GetAsync<Model>("USER:Objects/Sponza");
			var loadHelmet = Asset.GetAsync<Model>("USER:Objects/FlightHelmet");

			Project.OnProjectCreated += OnProjectCreated;
		}

		private static Mesh mesh;

		public static void OnProjectCreated()
		{
			new CameraActor().Spawn();
			new PointLightActor().Spawn();

			// Sponza
			var sponzaObject = new ModelActor("Sponza").Spawn<ModelActor>();
			sponzaObject.Position = new Vector3(0, -0.5f, 0);
			sponzaObject.Rotation = new Vector3(0, 0, 0);
			sponzaObject.Model = Asset.GetAsync<Model>("USER:Objects/Sponza").Result;
			
			// Helmet
			var helmObject = new ModelActor("Flight Helmet").Spawn<ModelActor>();
			helmObject.Position = new Vector3(0, -0.4f, 1);
			helmObject.Rotation = new Vector3(0, 180, 0);
			helmObject.Model = Asset.GetAsync<Model>("USER:Objects/FlightHelmet").Result;

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