using System;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class PrepassStep : RenderStep
	{
		public GraphicsBuffer CommandBuffer;
		public CommandSignature DepthCommandSignature;

		private ShaderProgram cullProgram;
		private ShaderProgram depthProgram;

		public override void Init()
		{
			// Compile indirect compute program.
			cullProgram = new ShaderProgram()
				.UseIncludes(typeof(Game).Assembly)
				.SetComputeShader(Embed.GetString("Content/Shaders/Prepass/CullCS.hlsl", typeof(Game).Assembly), "CullCS")
				.AsRootConstant(0, 1)
				.Compile().Result;

			// Compile depth prepass program.
			depthProgram = new ShaderProgram()
				.UseIncludes(typeof(Game).Assembly)
				.SetMeshShader(Embed.GetString("Content/Shaders/BaseMS.hlsl", typeof(Game).Assembly))
				.SetPixelShader(Embed.GetString("Content/Shaders/Prepass/DepthPS.hlsl", typeof(Game).Assembly))
				.SetDepthMode(DepthMode.GreaterEqual, true, true)
				.SetCullMode(CullMode.CCW)
				.AsRootConstant(0, 1)
				.Compile().Result;

			// Indirect command signature for depth pass.
			DepthCommandSignature = new CommandSignature()
				.AddConstantArg(0, depthProgram)
				.AddDispatchMeshArg()
				.Compile();

			CommandBuffer = new GraphicsBuffer(DepthCommandSignature.Stride * Scene.MaxInstanceCount, DepthCommandSignature.Stride, hasCounter: true);
		}

		public override void Run()
		{
			// Generate indirect draw commands and sort front-back.
			Cull();

			// Build depth buffer for opaque geometry.
			DrawDepth();
		}

		private void Cull()
		{
			// Reset command count.
			List.ResetCounter(CommandBuffer);

			// Switch to culling program (compute).
			List.SetProgram(cullProgram);

			// Set SRV inputs.
			List.SetProgramSRV(3, 1, Mesh.MeshBuffer);
			List.SetProgramSRV(5, 1, Scene.InstanceBuffer);

			// Set UAV outputs.
			List.SetProgramUAV(0, 0, CommandBuffer);

			// Build for all shaders
			List.SetProgramConstants(0, 0, -1);

			// Dispatch compute shader.
			if (Scene.InstanceCount > 0)
			{
				List.DispatchGroups(Scene.InstanceCount);
			}

			List.BarrierUAV(CommandBuffer);
		}

		private void DrawDepth()
		{
			// Switch to material program.
			List.SetProgram(depthProgram);

			// Set render targets.
			List.SetRenderTarget(null, RT.DepthBuffer);

			// Bind program SRVs.
			List.SetProgramSRV(0, 1, Mesh.VertBuffer);
			List.SetProgramSRV(1, 1, Mesh.PrimBuffer);
			List.SetProgramSRV(2, 1, Mesh.MeshletBuffer);
			List.SetProgramSRV(3, 1, Mesh.MeshBuffer);
			List.SetProgramSRV(4, 1, Scene.TransformBuffer);
			List.SetProgramSRV(5, 1, Scene.InstanceBuffer);

			// Bind program CBVs.
			List.SetProgramCBV(0, 1, RT.ViewCB);

			// Dispatch draw commands.
			List.ExecuteIndirect(DepthCommandSignature, CommandBuffer, Scene.MaxInstanceCount);
		}
	}
}