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
		private const int maxCommandCount = 100;
		private static GraphicsBuffer commandBuffer = new GraphicsBuffer(16 * maxCommandCount, 16);
		private static GraphicsBuffer<uint> commandCountBuffer = new GraphicsBuffer<uint>(1);

		private CommandSignature cullSignature;
		private ShaderProgram visProgram;
		private ShaderProgram cullProgram;

		public override void Init()
		{
			cullProgram = new ShaderProgram()
				.UseIncludes(typeof(Embed).Assembly)
				.SetComputeShader(Embed.GetString("Shaders/CullShader.hlsl"))
				.Compile().Result;

			visProgram = new ShaderProgram()
				.UseIncludes(typeof(Embed).Assembly)
				.SetMeshShader(Embed.GetString("Shaders/MeshShader.hlsl"))
				.SetPixelShader(Embed.GetString("Shaders/VisShader.hlsl"))
				.SetRTFormat(Format.R32G32_UInt)
				.SetDepthMode(DepthMode.GreaterEqual)
				.SetCullMode(CullMode.CCW)
				.AsConstant(0, 1)
				.Compile().Result;

			cullSignature = new CommandSignature()
				.WithConstantArg(0, visProgram)
				.WithDispatchMeshArg()
				.Compile();
		}

		public override void Run()
		{
			// Generate indirect draw commands.
			BuildCommands();

			// Build visibility buffer for opaque geometry.
			DrawVisibility();
		}

		private void DrawVisibility()
		{
			// Switch to visbuffer program (graphics).
			List.SetProgram(visProgram);

			// Set render targets.
			List.SetRenderTarget(Viewport.VisBuffer, Viewport.DepthBuffer);

			// Bind program inputs.
			List.SetProgramSRV(252, ModelActor.InstanceBuffer);
			List.SetProgramSRV(253, Mesh.MeshBuffer);
			List.SetProgramSRV(254, Mesh.MeshletBuffer);
			List.SetProgramSRV(255, Mesh.PrimBuffer);
			List.SetProgramSRV(256, Mesh.VertBuffer);

			// Update view data.
			Viewport.UpdateView();
			List.SetProgramCBV(1, Viewport.ViewConstantsBuffer);

			// Dispatch draw commands.
			List.BarrierUAV(commandBuffer, commandCountBuffer);
			List.DrawIndirect(cullSignature, maxCommandCount, commandBuffer, commandCountBuffer);
		}

		private void BuildCommands()
		{
			// Reset command count.
			commandCountBuffer.SetData(0, 0);

			// Switch to culling program (compute).
			List.SetProgram(cullProgram);

			// Set SRV inputs.
			List.SetProgramSRV(252, ModelActor.InstanceBuffer);
			List.SetProgramSRV(253, Mesh.MeshBuffer);

			// Set UAV outputs.
			List.SetProgramUAV(0, commandBuffer);
			List.SetProgramUAV(1, commandCountBuffer);

			// Dispatch compute shader.
			if (ModelActor.InstanceCount > 0)
			{
				List.DispatchGroups(ModelActor.InstanceCount);
			}
		}
	}
}