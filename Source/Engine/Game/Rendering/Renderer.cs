using System;
using Engine.Frontend;
using Engine.GPU;
using Engine.World;
using Vortice.Direct3D12;

namespace Engine.Rendering
{
	public static class Renderer
	{
		private static List<RenderStep> globalStage= new();
		private static List<RenderStep> viewportStage= new();

		public static void AddStep(RenderStep step, RenderStage stage)
		{
			switch (stage)
			{
				case RenderStage.Global:
					globalStage.Add(step);
					break;
				case RenderStage.Viewport:
					viewportStage.Add(step);
					break;
			}

			step.Init();
		}

		public static void Init()
		{
			AddStep(new SceneUpdateStep(), RenderStage.Global);
			AddStep(new PrepassStep(), RenderStage.Viewport);
			AddStep(new MaterialStep(), RenderStage.Viewport);
			AddStep(new ResolveStep(), RenderStage.Viewport);
		}

		public static void Render()
		{
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

			// Build and execute viewport-level commands on viewport command lists.
			foreach (var viewport in Viewport.All)
			{
				RenderStep.Viewport = viewport;
				CommandList viewportList = viewport.CommandList;

				RenderStep.List.PushEvent("Viewport");
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
				RenderStep.List.PopEvent();

				// Make sure the viewport's backbuffer is in the right state for presentation.
				viewportList.RequestState(viewport.Host.Swapchain.RT, ResourceStates.Present);

				// Make sure this viewport's commands are executing while we submit the next.
				viewportList.Execute();
			}

			// Submit default command list and wait for completion.
			Graphics.SubmitAndWait();

			// Present all windows.
			foreach (var viewport in Viewport.All)
			{
				viewport.Host.Swapchain.Present();

				// Reopen viewport command list.
				viewport.CommandList.Reset();
			}
		}

		public static void Cleanup()
		{
			Graphics.Flush();
		}
	}
}
