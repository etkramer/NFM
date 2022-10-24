﻿using System;
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

		private static List<SceneStep> sceneStage= new();
		private static List<CameraStep> cameraStage= new();

		public static void AddStep<T>(T step) where T : RenderStep
		{
			if (step is SceneStep)
			{
				sceneStage.Add(step as SceneStep);
			}
			else if (step is CameraStep)
			{
				cameraStage.Add(step as CameraStep);
			}

			step.Init();
		}

		public static void Init()
		{
			GPUContext.Init(2);
			DefaultCommandList.Name = "Default List";
			DefaultCommandList.Open();

			AddStep(new SceneUpdateStep());
			AddStep(new SkinningStep());
			AddStep(new PrepassStep());
			AddStep(new MaterialStep());
			AddStep(new GizmosStep());
			AddStep(new ResolveStep());
		}

		public static void RenderFrame()
		{
			// Run scene render steps.
			foreach (Scene scene in Scene.All)
			{
				// Execute per-scene render steps.
				foreach (var step in sceneStage)
				{
					step.Scene = scene;

					step.List.BeginEvent($"{step.GetType().Name} (scene)");
					step.Run();
					step.List.EndEvent();
				}
			}

			// Execute default command list and wait for it on the GPU.
			DefaultCommandList.Close();
			DefaultCommandList.Execute();

			// Render to each viewport.
			foreach (var viewport in Viewport.All)
			{
				RenderCamera(viewport.Camera, viewport.Host.Swapchain);
			}

			// Wait for completion.
			Graphics.WaitFrame();

			// Reopen default command list
			DefaultCommandList.Open();
		}

		public static void RenderCamera(CameraNode camera, Swapchain swapchain)
		{
			RenderCamera(camera, swapchain.RT, (o) => o.RequestState(swapchain.RT, ResourceStates.Present));
			swapchain.Present();
		}
		
		public static void RenderCamera(CameraNode camera, Texture texture) => RenderCamera(camera, texture, null);
		private static void RenderCamera(CameraNode camera, Texture texture, Action<CommandList> beforeExecute)
		{
			var rt = RenderTarget.Get(texture.Size);
			rt.CommandList.Open();
			rt.UpdateView(camera);

			foreach (CameraStep step in cameraStage)
			{
				step.RT = rt;
				step.Camera = camera;

				step.List.BeginEvent($"{step.GetType().Name} (camera)");
				step.Run();
				step.List.EndEvent();
			}

			rt.CommandList.ResolveTexture(rt.ColorTarget, texture);
			rt.CommandList.ClearRenderTarget(rt.ColorTarget);
			rt.CommandList.ClearDepth(rt.DepthBuffer);

			beforeExecute?.Invoke(rt.CommandList);
			rt.CommandList.Close();
			rt.CommandList.Execute();
		}

		public static void Cleanup()
		{
			Graphics.Flush();
		}
	}
}
