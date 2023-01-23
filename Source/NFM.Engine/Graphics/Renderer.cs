using System;
using System.Collections.Generic;
using Avalonia.Rendering;
using NFM.GPU;
using NFM.World;
using Vortice.Direct3D12;

namespace NFM.Graphics;

public static class Renderer
{
	/// <summary>
	/// "Shared" command list, guaranteed to be executed just before any frames are rendered.
	/// </summary>
	public static CommandList DefaultCommandList { get; private set; } = new CommandList();

	private static List<SceneStep> sceneSteps= new();

	public static void AddStep(SceneStep step)
	{
		sceneSteps.Add(step);
		step.Init();
	}

	public static void Init()
	{
		D3DContext.Init(2);
		DefaultCommandList.Name = "Default List";
		DefaultCommandList.Open();

		AddStep(new SkinningStep());
	}

	public static void RenderFrame()
	{
		// Run scene render steps.
		DefaultCommandList.BeginEvent("Update scenes");
		foreach (Scene scene in Scene.All)
		{
			// Execute per-scene render steps.
			foreach (var step in sceneSteps)
			{
				step.Scene = scene;
				
				DefaultCommandList.BeginEvent($"{step.GetType().Name} (scene)");
				step.Run(DefaultCommandList);
				DefaultCommandList.EndEvent();
			}
		}

		// We don't want other threads submitting uploads while the list is closed.
		lock (DefaultCommandList)
		{
			// Execute default command list and wait for it on the GPU.
			DefaultCommandList.EndEvent();
			DefaultCommandList.Close();
			DefaultCommandList.Execute();

			// Render to each viewport.
			foreach (var viewport in Viewport.All)
			{
				RenderCamera<StandardRenderPipeline>(viewport.Camera, viewport.Swapchain);
			}

			// Wait for completion.
			D3DContext.WaitFrame();

			// Reopen default command list
			DefaultCommandList.Open();
		}
	}

	public static void RenderCamera<T>(CameraNode camera, Swapchain swapchain) where T : RenderPipeline<T>, new()
	{
		RenderCamera<T>(camera, swapchain.RT, (o) => o.RequestState(swapchain.RT, ResourceStates.Present));
		swapchain.Present();
	}
	
	public static void RenderCamera<T>(CameraNode camera, Texture texture) where T : RenderPipeline<T>, new()  => RenderCamera<T>(camera, texture, null);
	private static void RenderCamera<T>(CameraNode camera, Texture texture, Action<CommandList> beforeExecute) where T : RenderPipeline<T>, new()
	{
		// Grab an RP instance and open it's command list
		var rp = RenderPipeline<T>.Get(texture);

		lock (rp)
		{
			// Execute the render pipeline
			rp.List.Open();
			rp.List.BeginEvent($"{typeof(T).Name} for {camera.Name}");
			rp.Render(texture, camera);

			// Setup gizmos context
			var context = new GizmosContext(rp.List, camera, rp.ViewMatrix, rp.ProjectionMatrix, rp.ViewCB);
			rp.List.SetRenderTarget(texture);

			// Draw axis lines
			context.DrawLine(new Vector3(0), new Vector3(1, 0, 0), Color.FromHex(0xfa3652));
			context.DrawLine(new Vector3(0), new Vector3(0, 1, 0), Color.FromHex(0x6fa21c));
			context.DrawLine(new Vector3(0), new Vector3(0, 0, 1), Color.FromHex(0x317cd1));

			// Close/execute the command list
			beforeExecute?.Invoke(rp.List);
			rp.List.EndEvent();
			rp.List.Close();
			rp.List.Execute();
		}
	}

	public static void Cleanup()
	{
		D3DContext.Flush();
	}
}
