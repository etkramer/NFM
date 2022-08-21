using System;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class OpaqueStep : RenderStep
	{
		private const int maxCommandCount = 100;
		private static GraphicsBuffer commandBuffer = new GraphicsBuffer(16 * maxCommandCount);
		private static GraphicsBuffer commandCountBuffer = new GraphicsBuffer(sizeof(uint));

		private CommandSignature commandSignature;
		private ShaderProgram visProgram;
		private ShaderProgram cullProgram;
		private ShaderProgram basicMaterialProgram;

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
				.SetDepthMode(DepthMode.GreaterEqual)
				.SetCullMode(CullMode.CCW)
				.AsConstant(0, 1)
				.Compile().Result;

			/*basicMaterialProgram = new ShaderProgram()
				.UseIncludes(typeof(Embed).Assembly)
				.SetComputeShader(Embed.GetString("Shaders/MaterialShader.hlsl"))
				.Compile().Result;*/

			commandSignature = new CommandSignature()
				.WithConstantArg(0, visProgram)
				.WithDispatchMeshArg()
				.Compile();
		}

		public override void Run()
		{
			// Set render targets.
			Graphics.SetRenderTarget(Viewport.ColorTarget, Viewport.DepthBuffer);

			// Generate indirect draw commands.
			BuildCommands();

			// Build visibility buffer for opaque geometry.
			DrawVisibility();
			DrawMaterials();
		}

		private void DrawMaterials()
		{
			//Graphics.SetProgram(basicMaterialProgram);
		}

		private void DrawVisibility()
		{
			// Switch to visbuffer program (graphics).
			Graphics.SetProgram(visProgram);

			// Bind program inputs.
			Graphics.SetProgramSRV(252, ModelActor.InstanceBuffer);
			Graphics.SetProgramSRV(253, Submesh.MeshBuffer);
			Graphics.SetProgramSRV(254, Submesh.MeshletBuffer);
			Graphics.SetProgramSRV(255, Submesh.PrimBuffer);
			Graphics.SetProgramSRV(256, Submesh.VertBuffer);

			// Update view data.
			Viewport.UpdateView();
			Graphics.SetProgramConstant(1, Viewport.ViewConstantsBuffer);

			// Dispatch draw commands.
			Graphics.BarrierUAV(commandBuffer, commandCountBuffer);
			Graphics.DrawIndirect(commandSignature, maxCommandCount, commandBuffer, commandCountBuffer);
		}

		private void BuildCommands()
		{
			// Reset command count.
			commandCountBuffer.SetData(0, 0);

			// Switch to culling program (compute).
			Graphics.SetProgram(cullProgram);

			// Set SRV inputs.
			Graphics.SetProgramSRV(252, ModelActor.InstanceBuffer);
			Graphics.SetProgramSRV(253, Submesh.MeshBuffer);

			// Set UAV outputs.
			Graphics.SetProgramUAV(0, commandBuffer);
			Graphics.SetProgramUAV(1, commandCountBuffer);

			// Dispatch compute shader.
			if (ModelActor.InstanceCount > 0)
			{
				Graphics.Dispatch(ModelActor.InstanceCount);
			}
		}
	}
}