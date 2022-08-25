using System;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;
using Vortice.DXGI;

namespace Engine.Rendering
{
	public class PrepassStep : RenderStep
	{
		public const int MaxCommandCount = 100;

		public GraphicsBuffer CommandBuffer;
		public GraphicsBuffer<uint> CommandCountBuffer;
		public CommandSignature CommandSignature;

		private ShaderProgram cullProgram;
		private ShaderProgram prepassProgram;

		public override void Init()
		{
			cullProgram = new ShaderProgram()
				.UseIncludes(typeof(Embed).Assembly)
				.SetComputeShader(Embed.GetString("Shaders/Prepass/CullCS.hlsl"))
				.Compile().Result;

			prepassProgram = new ShaderProgram()
				.UseIncludes(typeof(Embed).Assembly)
				.SetMeshShader(Embed.GetString("Shaders/BaseMS.hlsl"))
				.SetPixelShader(Embed.GetString("Shaders/Prepass/DepthPS.hlsl"))
				.SetDepthMode(DepthMode.GreaterEqual, true, true)
				.SetCullMode(CullMode.CCW)
				.AsConstant(0, 1)
				.Compile().Result;

			CommandSignature = new CommandSignature()
				.WithConstantArg(0, prepassProgram)
				.WithDispatchMeshArg()
				.Compile();

			CommandBuffer = new GraphicsBuffer(CommandSignature.Stride * MaxCommandCount, CommandSignature.Stride);
			CommandCountBuffer = new GraphicsBuffer<uint>(1);
		}

		public override void Run()
		{
			// Generate indirect draw commands.
			BuildCommands();

			// Build depth buffer for opaque geometry.
			DrawDepth();
		}

		private void BuildCommands()
		{
			// Reset command count.
			CommandCountBuffer.SetData(0, 0);

			// Switch to culling program (compute).
			List.SetProgram(cullProgram);

			// Set SRV inputs.
			List.SetProgramSRV(252, ModelActor.InstanceBuffer);
			List.SetProgramSRV(253, Mesh.MeshBuffer);

			// Set UAV outputs.
			List.SetProgramUAV(0, CommandBuffer);
			List.SetProgramUAV(1, CommandCountBuffer);

			// Dispatch compute shader.
			if (ModelActor.InstanceCount > 0)
			{
				List.DispatchGroups(ModelActor.InstanceCount);
			}
		}

		private void DrawDepth()
		{
			// Switch to material program.
			List.SetProgram(prepassProgram);

			// Set render targets.
			List.SetRenderTarget(null, Viewport.DepthBuffer);

			// Bind program inputs.
			List.SetProgramSRV(252, ModelActor.InstanceBuffer);
			List.SetProgramSRV(253, Mesh.MeshBuffer);
			List.SetProgramSRV(254, Mesh.MeshletBuffer);
			List.SetProgramSRV(255, Mesh.PrimBuffer);
			List.SetProgramSRV(256, Mesh.VertBuffer);
			List.SetProgramCBV(1, Viewport.ViewConstantsBuffer);

			// Dispatch draw commands.
			List.BarrierUAV(CommandBuffer, CommandCountBuffer);
			List.DrawIndirect(CommandSignature, MaxCommandCount, CommandBuffer, CommandCountBuffer);
		}
	}
}