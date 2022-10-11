using System;
using System.Collections.Generic;
using Avalonia.Rendering;
using Engine.Frontend;
using Engine.GPU;
using Engine.World;
using SharpDX.DXGI;
using Vortice.Direct3D12;
using Vortice.Mathematics;
using static Avalonia.OpenGL.GlInterface;

namespace Engine.Rendering
{
	public static class Renderer
	{
		/// <summary>
		/// Command list used for any command that needs to be done before the renderer does it's thing.
		/// </summary>
		public static CommandList DefaultCommandList { get; private set; } = new CommandList();

		private static List<RenderStep> globalStage= new();
		private static List<RenderStep> sceneStage= new();
		private static List<RenderStep> cameraStage= new();

		public static void AddStep(RenderStep step, RenderStage stage)
		{
			switch (stage)
			{
				case RenderStage.Global:
					Debug.Assert(!globalStage.Any(o => o.GetType() == step.GetType()), "Cannot add multiple render steps of the same type.");
					step.List = DefaultCommandList;
					step.RT = null;
					step.Scene = null;

					globalStage.Add(step);
					break;
				case RenderStage.Scene:
					Debug.Assert(!sceneStage.Any(o => o.GetType() == step.GetType()), "Cannot add multiple render steps of the same type.");
					step.List = DefaultCommandList;
					step.RT = null;

					sceneStage.Add(step);
					break;
				case RenderStage.Camera:
					Debug.Assert(!cameraStage.Any(o => o.GetType() == step.GetType()), "Cannot add multiple render steps of the same type.");

					cameraStage.Add(step);
					break;
			}

			step.Init();
		}

		public static T GetStep<T>() where T : RenderStep
		{
			foreach (var step in cameraStage)
			{
				if (step is T)
					return step as T;
			}
			foreach (var step in sceneStage)
			{
				if (step is T)
					return step as T;
			}
			foreach (var step in globalStage)
			{
				if (step is T)
					return step as T;
			}

			return null;
		}

		public static void Init()
		{
			GPUContext.Init(2);
			DefaultCommandList.Name = "Default List";

			AddStep(new SceneUpdateStep(), RenderStage.Scene);
			AddStep(new SkinningStep(), RenderStage.Scene);
			AddStep(new PrepassStep(), RenderStage.Camera);
			AddStep(new MaterialStep(), RenderStage.Camera);
			AddStep(new GizmosStep(), RenderStage.Camera);
			AddStep(new ResolveStep(), RenderStage.Camera);
		}

		public static void BeginFrame()
		{
			// Run global render steps.
			foreach (RenderStep step in globalStage)
			{
				step.List.PushEvent($"{step.GetType().Name} (global)");
				step.Run();
				step.List.PopEvent();
			}

			// Run scene render steps.
			foreach (Scene scene in Scene.All)
			{
				// Execute per-scene render steps.
				foreach (RenderStep step in sceneStage)
				{
					step.Scene = scene;

					step.List.PushEvent($"{step.GetType().Name} (scene)");
					step.Run();
					step.List.PopEvent();
				}
			}

			// Execute default command list and wait for it on the GPU.
			DefaultCommandList.Execute();
		}

		public static void EndFrame()
		{
			// Wait for completion.
			Graphics.WaitFrame();
		}

		public static void Render()
		{
			// Run global render steps.
			foreach (RenderStep step in globalStage)
			{
				step.List.PushEvent($"{step.GetType().Name} (global)");
				step.Run();
				step.List.PopEvent();
			}

			// Run scene render steps.
			foreach (Scene scene in Scene.All)
			{
				// Execute per-scene render steps.
				foreach (RenderStep step in sceneStage)
				{
					step.Scene = scene;

					step.List.PushEvent($"{step.GetType().Name} (scene)");
					step.Run();
					step.List.PopEvent();
				}
			}

			// Execute default command list and wait for it on the GPU.
			DefaultCommandList.Execute();

			// Render to each viewport.
			foreach (var viewport in Viewport.All)
			{
				RenderCamera(viewport.Camera, viewport.Host.Swapchain);
			}

			// Wait for completion.
			Graphics.WaitFrame();
		}

		public static void RenderCamera(CameraNode camera, Swapchain swapchain)
		{
			RenderCamera(camera, swapchain.RT, (o) => o.RequestState(swapchain.RT, ResourceStates.Present));
			swapchain.Present();
		}
		
		public static void RenderCamera(CameraNode camera, Texture texture) => RenderCamera(camera, texture, null);
		public static void RenderCamera(CameraNode camera, Texture texture, Action<CommandList> beforeExecute)
		{
			var rt = RenderTarget.Get(texture.Size);
			rt.UpdateView(camera);

			foreach (RenderStep step in cameraStage)
			{
				step.RT = rt;
				step.Camera = camera;
				step.List = rt.CommandList;
				step.Scene = camera.Scene;

				step.List.PushEvent($"{step.GetType().Name} (camera)");
				step.Run();
				step.List.PopEvent();
			}

			rt.CommandList.ResolveTexture(rt.ColorTarget, texture);
			rt.CommandList.ClearRenderTarget(rt.ColorTarget);
			rt.CommandList.ClearDepth(rt.DepthBuffer);

			beforeExecute?.Invoke(rt.CommandList);
			rt.CommandList.Execute();
		}

		public static void Cleanup()
		{
			Graphics.Flush();
		}
	}
}
