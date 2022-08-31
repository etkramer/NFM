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
			AddStep(new SceneUpdateStep(), RenderStage.Scene);
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

				step.Run();
			}

			// Loop through scenes.
			foreach (Scene scene in Scene.All)
			{
				RenderStep.List = DefaultCommandList;
				RenderStep.Scene = scene;

				// Execute per-scene render steps.
				foreach (RenderStep step in sceneStage)
				{
					RenderStep.Viewport = null;
					step.Run();
				}
			}

			// Execute default command list and wait for it on the GPU.
			DefaultCommandList.Execute(true);

			// Loop through scenes (again).
			foreach (Scene scene in Scene.All)
			{
				RenderStep.Scene = scene;

				// Build and execute per-viewport commands on viewport command lists.
				foreach (var viewport in Viewport.All)
				{
					// Skip viewports that belong to other scenes.
					if (viewport.Scene != scene)
					{
						continue;
					}

					// Switch to this viewport's command list.
					RenderStep.List = viewport.CommandList;
					RenderStep.Viewport = viewport;

					foreach (RenderStep step in viewportStage)
					{
						step.Run();
					}

					// Make sure the viewport's backbuffer is in the right state for presentation.
					RenderStep.List.RequestState(viewport.Host.Swapchain.RT, ResourceStates.Present);
					RenderStep.List.PopEvent();

					// Make sure this viewport's commands are executing while we submit the next.
					viewport.CommandList.Execute();
				}
			}

			// Submit default command list and wait for completion.
			Graphics.WaitFrame();

			// Present all windows.
			foreach (var viewport in Viewport.All)
			{
				viewport.Host.Swapchain.Present();

				// Reopen viewport command list.
				viewport.CommandList.Reset();
			}

			// Reset update command list.
			DefaultCommandList.Reset();
		}

		public static void Cleanup()
		{
			Graphics.Flush();
		}
	}
}
