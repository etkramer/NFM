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
			Graphics.PushEvent("Global");
			foreach (RenderStep step in globalStage)
			{
				RenderStep.Scene = Scene.Main;

				if (RenderStep.Scene != null)
				{
					Graphics.PushEvent(step.GetType().Name);
					step.Run();
					Graphics.PopEvent();
				}
			}
			Graphics.PopEvent();

			foreach (var viewport in Viewport.All)
			{
				Graphics.PushEvent("Viewport");
				RenderStep.Viewport = viewport;

				foreach (RenderStep step in viewportStage)
				{
					if (RenderStep.Scene != null)
					{
						Graphics.PushEvent(step.GetType().Name);
						step.Run();
						Graphics.PopEvent();
					}
				}

				Graphics.RequestState(viewport.Host.Swapchain.RT, ResourceStates.Present);
				Graphics.PopEvent();
			}

			// Submit graphics commands.
			Graphics.SubmitAndWait();

			// Present all windows.
			foreach (var viewport in Viewport.All)
			{
				viewport.Host.Swapchain.Present();
			}
		}

		public static void Cleanup()
		{
			Graphics.Flush();
		}
	}
}
