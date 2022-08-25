using System;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;
using Vortice.DXGI;

namespace Engine.Rendering
{
	public class MaterialStep : RenderStep
	{
		private ShaderProgram materialProgram;

		public override void Init()
		{
			materialProgram = new ShaderProgram()
				.UseIncludes(typeof(Embed).Assembly)
				.SetMeshShader(Embed.GetString("Shaders/BaseMS.hlsl"))
				.SetPixelShader(Embed.GetString("Shaders/Material/TempMaterialPS.hlsl"))
				.SetDepthMode(DepthMode.GreaterEqual, true, false)
				.SetCullMode(CullMode.CCW)
				.AsConstant(0, 1)
				.Compile().Result;
		}

		public override void Run()
		{
			// Switch to material program.
			List.SetProgram(materialProgram);

			// Set render targets.
			List.SetRenderTarget(Viewport.ColorTarget, Viewport.DepthBuffer);

			// Bind program inputs.
			List.SetProgramSRV(252, ModelActor.InstanceBuffer);
			List.SetProgramSRV(253, Mesh.MeshBuffer);
			List.SetProgramSRV(254, Mesh.MeshletBuffer);
			List.SetProgramSRV(255, Mesh.PrimBuffer);
			List.SetProgramSRV(256, Mesh.VertBuffer);
			List.SetProgramCBV(1, Viewport.ViewConstantsBuffer);

			// Dispatch draw commands.
			var prepass = Renderer.GetStep<PrepassStep>();
			List.BarrierUAV(prepass.CommandBuffer, prepass.CommandCountBuffer);
			List.DrawIndirect(prepass.CommandSignature, PrepassStep.MaxCommandCount, prepass.CommandBuffer, prepass.CommandCountBuffer);
		}
	}
}