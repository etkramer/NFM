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
			// Boot up renderer and load plugins.
			Renderer.Init();
			PluginSystem.Init();

			// Kick off model loading early.
			_ = Asset.LoadAsync<Model>("USER:/Objects/Heavy.glb");

			Project.OnProjectCreated += OnProjectCreated;
		}

		public static void OnProjectCreated()
		{
			// Create example model
			var model = new ModelNode();
			model.Position = new Vector3(0);
			model.Rotation = new Vector3(0);
			model.Model = Asset.LoadAsync<Model>("USER:/Objects/Heavy.glb").Result;
			model.Spawn();
		}

		public static void Update()
		{
			// Begin the new frame.
			Renderer.BeginFrame();
			OnTick.Invoke(Graphics.FrameTime);

			// Invoke dispose queue.
			DispatchQueue.Invoke(0);

			// Render to each viewport.
			foreach (var viewport in Viewport.All)
			{
				Renderer.RenderCamera(viewport.Camera, viewport.Host.Swapchain);
			}

			// End the frame.
			Renderer.EndFrame();
		}

		public static void Cleanup()
		{
			// Cleanup the renderer.
			Renderer.Cleanup();
		}
	}
}