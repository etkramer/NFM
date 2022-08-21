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
			//Asset.GetAsync<Model>("USER:Objects/FlightHelmet").Wait();
			Asset.GetAsync<Model>("USER:Objects/Sponza").Wait();

			Project.OnProjectCreated += OnProjectCreated;
		}

		public static void OnProjectCreated()
		{
			new Actor("Empty 1").Spawn();
			new Actor("Empty 2").Spawn();
			new CameraActor().Spawn();

			// Model
			/*ModelActor object5 = new ModelActor("Flight Helmet").Spawn<ModelActor>();
			object5.Rotation = new(0, 45, 0);
			object5.Position = new(0, -0.4f, -1.2f);
			object5.Model = Asset.GetAsync<Model>("USER:Objects/FlightHelmet").Result;*/

			// Model
			ModelActor object6 = new ModelActor("Sponza Atrium").Spawn<ModelActor>();
			object6.Position = new(0, -2, 1);
			object6.Model = Asset.GetAsync<Model>("USER:Objects/Sponza").Result;
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