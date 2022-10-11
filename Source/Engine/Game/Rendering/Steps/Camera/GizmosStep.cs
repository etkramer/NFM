using System;
using System.Collections.Generic;
using Engine.GPU;
using Engine.World;

namespace Engine.Rendering
{
	public class GizmosStep : RenderStep
	{
		public override void Run()
		{
			var context = new GizmosContext(List, RT, Camera);

			// Set render target.
			List.SetRenderTarget(RT.ColorTarget, RT.DepthBuffer);

			// Draw gizmos for each actor.
			foreach (var actor in Scene.Nodes)
			{
				actor.OnDrawGizmos(context);
			}
			
			context.DrawLine(new Vector3(0), new Vector3(1, 0, 0), Color.FromHex(0xfa3652));
			context.DrawLine(new Vector3(0), new Vector3(0, 1, 0), Color.FromHex(0x6fa21c));
			context.DrawLine(new Vector3(0), new Vector3(0, 0, 1), Color.FromHex(0x317cd1));
		}
	}

	public class GizmosContext
	{
		private CommandList renderList;
		private RenderTarget renderTarget;
		private CameraNode camera;

		public CameraNode Camera => camera;

		private static ShaderProgram polylineProgram = null;

		unsafe static GizmosContext()
		{
			polylineProgram = new ShaderProgram()
				.UseIncludes(typeof(Game).Assembly)
				.SetMeshShader(Embed.GetString("Content/Shaders/Gizmos/LineMS.hlsl", typeof(Game).Assembly), "LineMS")
				.SetPixelShader(Embed.GetString("Content/Shaders/Gizmos/GizmosPS.hlsl", typeof(Game).Assembly), "GizmosPS")
				.AsRootConstant(0, 4 + 4 + 4, 0)
				.SetDepthMode(DepthMode.GreaterEqual, true, true)
				.SetTopologyType(TopologyType.Line)
				.Compile().Result;
		}

		public GizmosContext(CommandList list, RenderTarget rt, CameraNode camera)
		{
			renderList = list;
			renderTarget = rt;
			this.camera = camera;
		}

		public void DrawLine(Vector3 p0, Vector3 p1, Color color = default)
		{
			if (color == default)
			{
				color = Color.White;
			}
			
			// Use the right shader program.
			renderList.SetProgram(polylineProgram);

			// Bind program constants (keeping in mind cbuffer packing requirements).
			renderList.SetProgramCBV(0, 1, renderTarget.ViewCB);
			renderList.SetProgramConstants(0, 0, AsInt(p0));
			renderList.SetProgramConstants(0, 4, AsInt(p1));
			renderList.SetProgramConstants(0, 8, AsInt((Vector3)color));

			// Dispatch draw command.
			renderList.DispatchMeshGroups(1);
		}

		private unsafe int AsInt(float value)
		{
			return *(int*)&value;
		}

		private unsafe int[] AsInt(Vector3 value)
		{
			return new int[] { AsInt(value.X), AsInt(value.Y), AsInt(value.Z) };
		}

		private unsafe int[] AsInt(Vector4 value)
		{
			return new int[] { AsInt(value.X), AsInt(value.Y), AsInt(value.Z), AsInt(value.W) };
		}
	}
}
