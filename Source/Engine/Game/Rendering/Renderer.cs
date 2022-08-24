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
			// Grab the default command list.
			CommandList defaultList = Graphics.GetCommandList();
			RenderStep.List = defaultList;

			// Build and execute global commands on default command list.
			foreach (RenderStep step in globalStage)
			{
				RenderStep.Scene = Scene.Main;

				if (RenderStep.Scene != null)
				{
					defaultList.PushEvent(step.GetType().Name);
					step.Run();
					defaultList.PopEvent();
				}
			}

			// Build and execute viewport-level commands on viewport command lists.
			foreach (var viewport in Viewport.All)
			{
				RenderStep.Viewport = viewport;
				RenderStep.List = viewport.List;
				CommandList viewportList = viewport.List;

				foreach (RenderStep step in viewportStage)
				{
					RenderStep.Scene = Scene.Main;

					if (RenderStep.Scene != null)
					{
						step.Run();
					}
				}

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
				viewport.List.Reset();
			}
		}

		public static void Cleanup()
		{
			Graphics.Flush();
		}
	}
}
