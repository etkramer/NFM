using System;
using Engine.Content;
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
				.UseIncludes(typeof(Embed).Assembly)
				.SetComputeShader(Embed.GetString("HLSL/Prepass/CullCS.hlsl"), "CullCS")
				.Compile().Result;

			// Compile depth prepass program.
			depthProgram = new ShaderProgram()
				.UseIncludes(typeof(Embed).Assembly)
				.SetMeshShader(Embed.GetString("HLSL/BaseMS.hlsl"))
				.SetPixelShader(Embed.GetString("HLSL/Prepass/DepthPS.hlsl"))
				.SetDepthMode(DepthMode.GreaterEqual, true, true)
				.SetCullMode(CullMode.CCW)
				.AsRootConstant(0, 1)
				.Compile().Result;

			// Indirect command signature for depth pass.
			DepthCommandSignature = new CommandSignature()
				.AddConstantArg(0, depthProgram)
				.AddDispatchMeshArg()
				.Compile();

			CommandBuffer = new GraphicsBuffer(DepthCommandSignature.Stride * ModelActor.MaxInstanceCount, DepthCommandSignature.Stride, hasCounter: true);
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
			CommandBuffer.ResetCounter();

			// Switch to culling program (compute).
			List.SetProgram(cullProgram);

			// Set SRV inputs.
			List.SetProgramSRV(3, 1, Mesh.MeshBuffer);
			List.SetProgramSRV(5, 1, ModelActor.InstanceBuffer);

			// Set UAV outputs.
			List.SetProgramUAV(0, 0, CommandBuffer);

			// Dispatch compute shader.
			if (ModelActor.InstanceCount > 0)
			{
				List.DispatchGroups(ModelActor.InstanceCount);
			}
		}

		private void DrawDepth()
		{
			// Switch to material program.
			List.SetProgram(depthProgram);

			// Set render targets.
			List.SetRenderTarget(null, Viewport.DepthBuffer);

			// Bind program SRVs.
			List.SetProgramSRV(0, 1, Mesh.VertBuffer);
			List.SetProgramSRV(1, 1, Mesh.PrimBuffer);
			List.SetProgramSRV(2, 1, Mesh.MeshletBuffer);
			List.SetProgramSRV(3, 1, Mesh.MeshBuffer);
			List.SetProgramSRV(4, 1, Actor.TransformBuffer);
			List.SetProgramSRV(5, 1, ModelActor.InstanceBuffer);

			// Bind program CBVs.
			List.SetProgramCBV(0, 1, Viewport.ViewCB);

			// Dispatch draw commands.
			List.ExecuteIndirect(DepthCommandSignature, CommandBuffer, ModelActor.MaxInstanceCount);
		}
	}
}