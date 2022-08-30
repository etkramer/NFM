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

		public static void Init()
		{
			// Init basic systems.
			GPUContext.Init();
			Renderer.Init();

			// Load all plugins.
			PluginSystem.LoadAll();

			// Precache model.
			Asset.GetAsync<Model>("USER:Objects/FlightHelmet").Wait();
			Asset.GetAsync<Model>("USER:Objects/Sponza").Wait();

			Project.OnProjectCreated += OnProjectCreated;
		}

		public static void OnProjectCreated()
		{
			new CameraActor().Spawn();
			new PointLightActor().Spawn();
			
			// Helmet
			var helmObject = new ModelActor("Flight Helmet").Spawn<ModelActor>();
			helmObject.Position = new Vector3(0, -0.4f, 1);
			helmObject.Rotation = new Vector3(0, 180, 0);
			helmObject.Model = Asset.GetAsync<Model>("USER:Objects/FlightHelmet").Result;

			// Sponza
			var sponzaObject = new ModelActor("Sponza").Spawn<ModelActor>();
			sponzaObject.Position = new Vector3(0, -0.4f, 0);
			sponzaObject.Rotation = new Vector3(0, 0, 0);
			sponzaObject.Model = Asset.GetAsync<Model>("USER:Objects/Sponza").Result;

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