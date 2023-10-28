using NFM.GPU;
using NFM.World;

namespace NFM.Graphics;

public class Gizmos
{
	private CommandList renderList;
	public CameraNode Camera { get; private set; }

	private static readonly PipelineState linePSO;
	private static readonly PipelineState geometryPSO;

	private static readonly TypedBuffer<uint> gizmosIndexBuffer = new TypedBuffer<uint>(2048 * 3); // Support up to 2048 tris per DrawGeometry() call
	private static readonly TypedBuffer<Vector3> gizmosVertexBuffer = new TypedBuffer<Vector3>(2048); // Support up to 2048 verts per DrawGeometry() call

	public static event EventHandler<Gizmos> OnDrawGizmos = delegate {};

	unsafe static Gizmos()
	{
		linePSO = new PipelineState()
			.SetMeshShader(new ShaderModule(Embed.GetString("Shaders/Common/Gizmos/LineMS.hlsl"), ShaderStage.Mesh))
			.SetPixelShader(new ShaderModule(Embed.GetString("Shaders/Common/Gizmos/GizmosPS.hlsl"), ShaderStage.Pixel))
			.AsRootConstant(0, 4 + 4 + 4)
			.SetDepthMode(DepthMode.GreaterEqual, true, true)
			.SetTopologyType(TopologyType.Line)
			.Compile().Result;

		geometryPSO = new PipelineState()
			.SetMeshShader(new ShaderModule(Embed.GetString("Shaders/Common/Gizmos/GeomMS.hlsl"), ShaderStage.Mesh))
			.SetPixelShader(new ShaderModule(Embed.GetString("Shaders/Common/Gizmos/GizmosPS.hlsl"), ShaderStage.Pixel))
			.AsRootConstant(0, 4)
			.SetDepthMode(DepthMode.Always, false, true)
			.Compile().Result;
	}

	private Matrix4 viewMatrix;
	private Matrix4 projectionMatrix;
	private TypedBuffer<ViewConstants> viewConstants;

	public Gizmos(CommandList list, CameraNode camera, Matrix4 view, Matrix4 projection, TypedBuffer<ViewConstants> constants)
	{
		renderList = list;
		Camera = camera;

		viewMatrix = view;
		projectionMatrix = projection;
		viewConstants = constants;
	}

	public void DrawBox(Box3D box, Color borderColor = default, Color fillColor = default)
	{
		if (borderColor == default)
		{
			borderColor = Color.White;
		}
		if (fillColor == default)
		{
			fillColor = new Color(borderColor.R / 2, borderColor.G / 2, borderColor.B / 2, borderColor.A);
		}

		// Draw fill, if needed
		if (fillColor.A != 0)
		{
			// Draw fill
			Vector4[] vertices = new Vector4[]
			{
				ToClipSpace(box.BottomLeftNear), // 0
				ToClipSpace(box.BottomRightNear), // 1
				ToClipSpace(box.TopLeftNear), // 2
				ToClipSpace(box.TopRightNear), // 3
				ToClipSpace(box.BottomLeftFar), // 4
				ToClipSpace(box.BottomRightFar), // 5
				ToClipSpace(box.TopLeftFar), // 6
				ToClipSpace(box.TopRightFar), // 7
			};

			uint[] indices = new uint[]
			{
				// Front
				0, 2, 1,
				2, 3, 1,
				// Back
				4, 6, 5,
				6, 7, 5,
				// Left
				4, 6, 0,
				6, 2, 0,
				// Right
				5, 7, 1,
				7, 3, 1,
				// Top
				2, 6, 3,
				6, 7, 3,
				// Bottom
				0, 4, 1,
				4, 5, 1,
			};

			DrawGeometry(vertices, indices, fillColor);
		}

		// Draw border, if needed.
		if (borderColor != fillColor && borderColor.A != 0)
		{
			// Vertical lines
			DrawLine(box.BottomLeftNear, box.TopLeftNear, borderColor);
			DrawLine(box.BottomRightNear, box.TopRightNear, borderColor);
			DrawLine(box.BottomLeftFar, box.TopLeftFar, borderColor);
			DrawLine(box.BottomRightFar, box.TopRightFar, borderColor);

			// Bottom lines
			DrawLine(box.BottomLeftNear, box.BottomLeftFar, borderColor);
			DrawLine(box.BottomLeftFar, box.BottomRightFar, borderColor);
			DrawLine(box.BottomRightFar, box.BottomRightNear, borderColor);
			DrawLine(box.BottomRightNear, box.BottomLeftNear, borderColor);

			// Top lines
			DrawLine(box.TopLeftNear, box.TopLeftFar, borderColor);
			DrawLine(box.TopLeftFar, box.TopRightFar, borderColor);
			DrawLine(box.TopRightFar, box.TopRightNear, borderColor);
			DrawLine(box.TopRightNear, box.TopLeftNear, borderColor);
		}
	}

	public void DrawLine(Vector3 p0, Vector3 p1, Color color = default)
	{
		if (color == default)
		{
			color = Color.White;
		}
		
		// Use the right shader program.
		renderList.SetPipelineState(linePSO);

		// Bind program constants (keeping in mind cbuffer packing requirements).
		renderList.SetPipelineCBV(0, 1, viewConstants);
		renderList.SetPipelineConstants(0, 0, AsInt(p0));
		renderList.SetPipelineConstants(0, 4, AsInt(p1));
		renderList.SetPipelineConstants(0, 8, AsInt(color));

		// Dispatch draw command.
		renderList.DispatchMesh(1);
	}

	public void DrawArrow(Vector3 p0, Vector3 p1, float radius, Color color = default)
	{
		float headLength = radius * 5;

		Vector3 direction = (p1 - p0).Normalized();
		DrawLine(p0, p1 - (headLength * direction), color);
		DrawCone(p1 - (headLength * direction), p1, radius, color);
	}

	public void DrawCone(Vector3 p0, Vector3 p1, float radius, Color color = default)
	{
		const int numSegments = 12;
		const float aMax = (MathF.PI * 2.0f) * (numSegments - 1.0f) / numSegments;

		Span<Vector4> vertices = stackalloc Vector4[numSegments + 2];
		Span<uint> indices = stackalloc uint[(numSegments + 1) * 3];

		// Populate vertices
		vertices[numSegments + 1] = ToClipSpace(p1);
		for (int i = 0; i <= numSegments; i++)
		{
			// Calculate vertex coords
			float aMin = 0f;
			float a = aMin + (i / (float)numSegments) * (aMax - aMin);
			var vertex = new Vector3(0 + MathF.Cos(a) * radius, 0 + MathF.Sin(a) * radius, 0);

			// Rotate vertex
			Vector3 fromDirection = new Vector3(0, 0, -1);
			Vector3 toDirection = (p1 - p0).Normalized();
			float angleRadians = MathF.Acos(Vector3.Dot(fromDirection, toDirection));
			Vector3 axis = Vector3.Cross(fromDirection, toDirection);

			vertex = p0 + Vector3.Transform(vertex, Rotation.FromAxisAngle(axis, angleRadians));
			vertices[i] = ToClipSpace(vertex);
		}
		
		// Populate indices
		for (int i = 0; i <= numSegments; i++)
		{
			indices[(i * 3) + 0] = (uint)i;
			indices[(i * 3) + 1] = (i == numSegments) ? 0 : (uint)i + 1;
			indices[(i * 3) + 2] = numSegments + 1;
		}

		DrawGeometry(vertices, indices, color);
	}

	private Vector4 ToClipSpace(Vector3 world)
	{
		var clip = new Vector4(world, 1);
		clip *= viewMatrix;
		clip *= projectionMatrix;

		return clip;
	}

	private void DrawGeometry(ReadOnlySpan<Vector4> vertices, ReadOnlySpan<uint> indices, Color color)
	{
		Guard.Require(indices.Length % 3 == 0, "Indices passed to DrawGeometry() must be triangles.");

		renderList.UploadBuffer(gizmosIndexBuffer, indices);
		renderList.UploadBuffer(gizmosVertexBuffer, vertices);
		
		// Use the right shader program.
		renderList.SetPipelineState(geometryPSO);

		// Bind program constants
		renderList.SetPipelineSRV(0, 0, gizmosVertexBuffer);
		renderList.SetPipelineSRV(1, 0, gizmosIndexBuffer);
		renderList.SetPipelineConstants(0, 0, AsInt(color));

		// Dispatch draw command.
		renderList.DispatchMesh(indices.Length / 3);
	}

	internal void FireGizmosEvent()
	{
		OnDrawGizmos?.Invoke(null, this);
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
