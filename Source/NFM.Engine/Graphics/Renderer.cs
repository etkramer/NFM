using NFM.GPU;
using NFM.World;
using Vortice.Direct3D12;

namespace NFM.Graphics;

static class Renderer
{
	/// <summary>
	/// "Shared" command list, guaranteed to be executed just before any frames are rendered.
	/// </summary>
	public static CommandList DefaultCommandList { get; private set; } = new CommandList();

	private static readonly List<ScenePass> scenePasses = new();

	// One pipeline per camera, so passes can safely keep per-view history between frames.
	private static readonly Dictionary<CameraNode, RenderPipeline> pipelines = new();

	public static void AddPass(ScenePass pass)
	{
		scenePasses.Add(pass);
		pass.Init();
	}

	public static void Init()
	{
		D3DContext.Init(2);
		DefaultCommandList.Name = "Default List";
		DefaultCommandList.Open();

		AddPass(new SkinningStep());
	}

	public static void RenderFrame()
	{
		// Run scene render passes.
		DefaultCommandList.BeginEvent("Update scenes");
		foreach (Scene scene in Scene.All)
		{
			// Flush pending node changes to the GPU.
			scene.RenderData.Sync(DefaultCommandList);

			ScenePassContext ctx = new()
			{
				List = DefaultCommandList,
				Scene = scene
			};

			// Execute per-scene render passes.
			foreach (var pass in scenePasses)
			{
				DefaultCommandList.BeginEvent($"{pass.GetType().Name} (scene)");
				pass.Run(ctx);
				DefaultCommandList.EndEvent();
			}
		}

		// Execute default command list and wait for it on the GPU.
		DefaultCommandList.EndEvent();
		DefaultCommandList.Close();
		DefaultCommandList.Execute();

		try
		{
			// Render to each viewport.
			foreach (var viewport in Viewport.All)
			{
				RenderCamera<StandardRenderPipeline>(viewport.Camera, viewport.Swapchain);
			}
		}
		finally
		{
			// Wait for completion.
			D3DContext.WaitFrame();

			// Reopen default command list
			DefaultCommandList.Open();
		}
	}

	public static void RenderCamera<T>(CameraNode camera, Swapchain swapchain) where T : RenderPipeline, new()
	{
		RenderCamera<T>(camera, swapchain.RT, (o) => o.RequestState(swapchain.RT, ResourceStates.Present));
		swapchain.Present();
	}

	public static void RenderCamera<T>(CameraNode camera, Texture texture) where T : RenderPipeline, new() => RenderCamera<T>(camera, texture, null);
	private static void RenderCamera<T>(CameraNode camera, Texture texture, Action<CommandList>? beforeExecute) where T : RenderPipeline, new()
	{
		// Grab this camera's RP and open it's command list
		var rp = GetPipeline<T>(camera, texture.Size);

		// Execute the render pipeline
		rp.List.Open();
		rp.List.BeginEvent($"{typeof(T).Name} for {camera.Name}");
		rp.Render(texture, camera);

		// Setup gizmos context
		var context = new Gizmos(rp.List, camera, rp.ViewMatrix, rp.ProjectionMatrix, rp.ViewCB);
		rp.List.SetRenderTarget(texture);

		// Draw gizmos for any subscribers
		context.FireGizmosEvent();

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

	private static T GetPipeline<T>(CameraNode camera, Vector2i size) where T : RenderPipeline, new()
	{
		if (pipelines.TryGetValue(camera, out var existing))
		{
			if (existing is T match && existing.Size == size)
			{
				return match;
			}

			// Wrong type or stale size - rebuild from scratch.
			existing.Dispose();
			pipelines.Remove(camera);
		}

		T pipeline = new();
		pipeline.Build(size);
		pipelines[camera] = pipeline;

		return pipeline;
	}

	/// <summary>
	/// Drops the render pipeline belonging to a camera that's going away.
	/// </summary>
	public static void ReleasePipeline(CameraNode camera)
	{
		if (pipelines.Remove(camera, out var pipeline))
		{
			pipeline.Dispose();
		}
	}

	public static void Cleanup()
	{
		D3DContext.Flush();
	}
}
