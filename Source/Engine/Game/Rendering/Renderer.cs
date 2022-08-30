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
		private static List<RenderStep> viewportStage= new();

		public static void AddStep(RenderStep step, RenderStage stage)
		{
			switch (stage)
			{
				case RenderStage.Global:
					Debug.Assert(!globalStage.Any(o => o.GetType() == step.GetType()), "Cannot add multiple render steps of the same type.");
					globalStage.Add(step);
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
			foreach (var step in globalStage)
			{
				if (step is T)
					return step as T;
			}

			return null;
		}

		public static void Init()
		{
			AddStep(new SceneUpdateStep(), RenderStage.Global);
			AddStep(new PrepassStep(), RenderStage.Viewport);
			AddStep(new MaterialStep(), RenderStage.Viewport);
			AddStep(new LightingStep(), RenderStage.Viewport);
			AddStep(new ResolveStep(), RenderStage.Viewport);
		}

		public static void Render()
		{
			// Switch to the default command list.
			RenderStep.List = DefaultCommandList;

			// Build and execute global commands on default command list.
			RenderStep.List.PushEvent("Global");
			foreach (RenderStep step in globalStage)
			{
				RenderStep.Viewport = null;
				RenderStep.Scene = Scene.Main;

				if (RenderStep.Scene != null)
				{
					RenderStep.List.PushEvent(step.GetType().Name);
					step.Run();
					RenderStep.List.PopEvent();
				}
			}
			RenderStep.List.PopEvent();

			// Execute default command list and wait for it on the GPU.
			DefaultCommandList.Execute(true);

			// Build and execute viewport-level commands on viewport command lists.
			foreach (var viewport in Viewport.All)
			{
				// Switch to this viewport's command list.
				RenderStep.List = viewport.CommandList;
				RenderStep.List.PushEvent("Viewport");
				RenderStep.Viewport = viewport;

				foreach (RenderStep step in viewportStage)
				{
					RenderStep.Scene = Scene.Main;

					if (RenderStep.Scene != null)
					{
						RenderStep.List.PushEvent(step.GetType().Name);
						step.Run();
						RenderStep.List.PopEvent();
					}
				}

				// Make sure the viewport's backbuffer is in the right state for presentation.
				RenderStep.List.RequestState(viewport.Host.Swapchain.RT, ResourceStates.Present);
				RenderStep.List.PopEvent();

				// Make sure this viewport's commands are executing while we submit the next.
				viewport.CommandList.Execute();
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
