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
			var context = new GizmosContext(List, Viewport);

			// Set render target.
			List.SetRenderTarget(Viewport.ColorTarget, Viewport.DepthBuffer);

			// Draw gizmos for each actor.
			foreach (var actor in Scene.Actors)
			{
				actor.OnDrawGizmos(context);
			}
		}
	}

	public class GizmosContext
	{
		private CommandList renderList;
		private Viewport renderViewport;

		public CameraActor Camera => renderViewport.Camera;

		private static ShaderProgram polylineProgram = null;

		unsafe static GizmosContext()
		{
			polylineProgram = new ShaderProgram()
				.UseIncludes(typeof(Game).Assembly)
				.SetMeshShader(Embed.GetString("Content/Shaders/Gizmos/PolylineMS.hlsl", typeof(Game).Assembly), "PolylineMS")
				.SetPixelShader(Embed.GetString("Content/Shaders/Gizmos/GizmosPS.hlsl", typeof(Game).Assembly), "GizmosPS")
				.AsRootConstant(0, 4 + 4 + 4 + 1, 0)
				.SetDepthMode(DepthMode.GreaterEqual, true, true)
				.SetTopologyType(TopologyType.Line)
				.Compile().Result;
		}

		public GizmosContext(CommandList list, Viewport viewport)
		{
			renderList = list;
			renderViewport = viewport;
		}

		public void DrawLine(Vector3 p0, Vector3 p1, Color color = default, int width = 1)
		{
			if (color == default)
			{
				color = Color.FromHex(0xFF9F2C);
			}
			
			// Use the right shader program.
			renderList.SetProgram(polylineProgram);

			// Bind program constants (keeping in mind cbuffer packing requirements).
			renderList.SetProgramCBV(0, 1, renderViewport.ViewCB);
			renderList.SetProgramConstants(0, 0, 0, AsInt(p0));
			renderList.SetProgramConstants(0, 0, 4, AsInt(p1));
			renderList.SetProgramConstants(0, 0, 8, AsInt((Vector3)color));
			renderList.SetProgramConstants(0, 0, 12, width);

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
