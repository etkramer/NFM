using System;
using Engine.Frontend;
using Engine.GPU;
using Engine.World;
using Vortice.Direct3D12;

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
		private static List<RenderStep> viewportStage= new();

		public static void AddStep(RenderStep step, RenderStage stage)
		{
			switch (stage)
			{
				case RenderStage.Global:
					Debug.Assert(!globalStage.Any(o => o.GetType() == step.GetType()), "Cannot add multiple render steps of the same type.");
					globalStage.Add(step);
					break;
				case RenderStage.Scene:
					Debug.Assert(!sceneStage.Any(o => o.GetType() == step.GetType()), "Cannot add multiple render steps of the same type.");
					sceneStage.Add(step);
					break;
				case RenderStage.Viewport:
					Debug.Assert(!viewportStage.Any(o => o.GetType() == step.GetType()), "Cannot add multiple render steps of the same type.");
					viewportStage.Add(step);
					break;
			}

			step.Init();
		}

		public static T GetStep<T>() where T : RenderStep
		{
			foreach (var step in viewportStage)
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
			AddStep(new SkinningStep(), RenderStage.Viewport);
			AddStep(new PrepassStep(), RenderStage.Viewport);
			AddStep(new MaterialStep(), RenderStage.Viewport);
			AddStep(new ResolveStep(), RenderStage.Viewport);
		}

		public static void Render()
		{
			// Execute global render steps.
			foreach (RenderStep step in globalStage)
			{
				RenderStep.List = DefaultCommandList;
				RenderStep.Viewport = null;
				RenderStep.Scene = null;

				RenderStep.List.PushEvent($"{step.GetType().Name} (global)");
				step.Run();
				RenderStep.List.PopEvent();
			}

			// Loop through scenes.
			foreach (Scene scene in Scene.All)
			{
				RenderStep.List = DefaultCommandList;
				RenderStep.Viewport = null;
				RenderStep.Scene = scene;

				// Execute per-scene render steps.
				foreach (RenderStep step in sceneStage)
				{
					RenderStep.List.PushEvent($"{step.GetType().Name} (scene)");
					step.Run();
					RenderStep.List.PopEvent();
				}
			}

			// Execute default command list and wait for it on the GPU.
			DefaultCommandList.Execute();

			// Build and execute per-viewport commands. Consider doing this in parallel.
			foreach (var viewport in Viewport.All)
			{
				// Switch to this viewport's command list.
				RenderStep.List = viewport.CommandList;
				RenderStep.Viewport = viewport;
				RenderStep.Scene = RenderStep.Viewport.Scene;

				foreach (RenderStep step in viewportStage)
				{
					RenderStep.List.PushEvent($"{step.GetType().Name} (viewport)");
					step.Run();
					RenderStep.List.PopEvent();
				}

				// Make sure the viewport's backbuffer is in the right state for presentation.
				RenderStep.List.RequestState(viewport.Host.Swapchain.RT, ResourceStates.Present);

				// Make sure this viewport's commands are executing while we submit the next.
				viewport.CommandList.Execute();
			}

			// Reset global state.
			RenderStep.List = null;
			RenderStep.Viewport = null;
			RenderStep.Scene = null;

			// Present swapchains.
			Viewport.All.ForEach(o => o.Host.Swapchain.Present());

			// Wait for completion.
			Graphics.WaitFrame();
		}

		public static void Cleanup()
		{
			Graphics.Flush();
		}
	}
}
