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
		public ShaderProgram MaterialProgram;
		public CommandSignature MaterialCommandSignature;

		public override void Init()
		{
			MaterialProgram = new ShaderProgram()
				.UseIncludes(typeof(Embed).Assembly)
				.SetMeshShader(Embed.GetString("HLSL/BaseMS.hlsl"))
				.SetPixelShader(Embed.GetString("HLSL/Material/BaseMaterialPS.hlsl"), "MaterialPS")
				.SetDepthMode(DepthMode.Equal, true, false)
				.SetCullMode(CullMode.CCW)
				.SetRTCount(2)
				.AsRootConstant(0, 1)
				.Compile().Result;

			MaterialCommandSignature = new CommandSignature()
				.AddConstantArg(0, MaterialProgram)
				.AddDispatchMeshArg()
				.Compile();
		}

		public override void Run()
		{
			// Switch to material program.
			List.SetProgram(MaterialProgram);

			// Set and reset render targets.
			List.ClearRenderTarget(Viewport.MatBuffer0);
			List.ClearRenderTarget(Viewport.MatBuffer1);
			List.SetRenderTargets(Viewport.DepthBuffer, Viewport.MatBuffer0, Viewport.MatBuffer1);

			// Bind program inputs.
			List.SetProgramSRV(251, Actor.TransformBuffer);
			List.SetProgramSRV(252, ModelActor.InstanceBuffer);
			List.SetProgramSRV(253, Mesh.MeshBuffer);
			List.SetProgramSRV(254, Mesh.MeshletBuffer);
			List.SetProgramSRV(255, Mesh.PrimBuffer);
			List.SetProgramSRV(256, Mesh.VertBuffer);
			List.SetProgramCBV(1, Viewport.ViewCB);

			// Dispatch draw commands.
			var prepass = Renderer.GetStep<PrepassStep>();
			List.ExecuteIndirect(prepass.DepthCommandSignature, prepass.CommandBuffer, ModelActor.MaxInstanceCount);
		}
	}
}