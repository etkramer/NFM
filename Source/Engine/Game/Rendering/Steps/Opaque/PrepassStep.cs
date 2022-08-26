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
			cullProgram = new ShaderProgram()
				.UseIncludes(typeof(Embed).Assembly)
				.SetComputeShader(Embed.GetString("HLSL/Prepass/CullCS.hlsl"), "CullCS")
				.Compile().Result;

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
			SortCommands();

			// Build depth buffer for opaque geometry.
			DrawDepth();
		}

		private void SortCommands()
		{

		}

		private void Cull()
		{
			// Reset command count.
			CommandBuffer.ResetCounter();

			// Switch to culling program (compute).
			List.SetProgram(cullProgram);

			// Set SRV inputs.
			List.SetProgramSRV(252, ModelActor.InstanceBuffer);
			List.SetProgramSRV(253, Mesh.MeshBuffer);

			// Set UAV outputs.
			List.SetProgramUAV(0, CommandBuffer);

			// Dispatch compute shader.
			if (ModelActor.InstanceBufferCount > 0)
			{
				List.DispatchGroups(ModelActor.InstanceBufferCount);
			}
		}

		private void DrawDepth()
		{
			// Switch to material program.
			List.SetProgram(depthProgram);

			// Set render targets.
			List.SetRenderTarget(null, Viewport.DepthBuffer);

			// Bind program inputs.
			List.SetProgramSRV(251, Actor.TransformBuffer);
			List.SetProgramSRV(252, ModelActor.InstanceBuffer);
			List.SetProgramSRV(253, Mesh.MeshBuffer);
			List.SetProgramSRV(254, Mesh.MeshletBuffer);
			List.SetProgramSRV(255, Mesh.PrimBuffer);
			List.SetProgramSRV(256, Mesh.VertBuffer);
			List.SetProgramCBV(1, Viewport.ViewCB);

			// Dispatch draw commands.
			List.DrawIndirect(DepthCommandSignature, ModelActor.MaxInstanceCount, CommandBuffer);
		}
	}
}