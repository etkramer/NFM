using NFM.GPU;
using NFM.World;

namespace NFM.Graphics;

/// <summary>
/// Immediate-mode overlay drawing for one camera's frame. Calls are batched and flushed as two draws -
/// one for lines, one for solid geometry - so a gizmo costs the same whether it's one line or a thousand.
/// </summary>
public class Gizmos
{
	/// <summary>Line width in pixels.</summary>
	public const float DefaultWidth = 2f;

	public static readonly Color[] AxisColors = [Color.FromHex(0xfa3652), Color.FromHex(0x6fa21c), Color.FromHex(0x317cd1)];
	public static readonly Vector3[] AxisDirections = [Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ];

	/// <summary>Replaces an axis colour while its handle is hovered or dragged.</summary>
	public static readonly Color Highlight = Color.FromHex(0xffbe2e);

	public static event EventHandler<Gizmos> OnDrawGizmos = delegate {};

	public CameraNode Camera { get; }
	public GizmoView View { get; }

	[StructLayout(LayoutKind.Sequential)]
	private struct LineData
	{
		public Vector3 P0;
		public float Width;
		public Vector3 P1;
		public float Padding;
		public Color Color;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct VertexData
	{
		public Vector3 Position;
		public float Padding;
		public Color Color;
	}

	private const int MaxLines = 8192;
	private const int MaxVertices = 16384;
	private const int MaxIndices = MaxVertices * 3;

	private static readonly PipelineState linePSO;
	private static readonly PipelineState geometryPSO;

	private static readonly TypedBuffer<LineData> lineBuffer = new(MaxLines);
	private static readonly TypedBuffer<VertexData> vertexBuffer = new(MaxVertices);
	private static readonly TypedBuffer<uint> indexBuffer = new(MaxIndices);

	static Gizmos()
	{
		// Gizmos are an overlay drawn after the pipeline resolves, so they never test or write depth.
		linePSO = new PipelineState()
			.SetMeshShader(new ShaderModule(Embed.GetString("Shaders/Common/Gizmos/LineMS.hlsl"), ShaderStage.Mesh))
			.SetPixelShader(new ShaderModule(Embed.GetString("Shaders/Common/Gizmos/GizmosPS.hlsl"), ShaderStage.Pixel))
			.AsRootConstant(0, 2)
			.SetDepthMode(DepthMode.None, false, false)
			.SetEnableBlend(true)
			.Compile().Result;

		geometryPSO = new PipelineState()
			.SetMeshShader(new ShaderModule(Embed.GetString("Shaders/Common/Gizmos/GeomMS.hlsl"), ShaderStage.Mesh))
			.SetPixelShader(new ShaderModule(Embed.GetString("Shaders/Common/Gizmos/GizmosPS.hlsl"), ShaderStage.Pixel))
			.SetDepthMode(DepthMode.None, false, false)
			.SetEnableBlend(true)
			.Compile().Result;
	}

	private readonly CommandList renderList;
	private readonly TypedBuffer<ViewConstants> viewConstants;

	private readonly List<LineData> lines = [];
	private readonly List<VertexData> vertices = [];
	private readonly List<uint> indices = [];

	public Gizmos(CommandList list, CameraNode camera, GizmoView view, TypedBuffer<ViewConstants> constants)
	{
		renderList = list;
		viewConstants = constants;

		Camera = camera;
		View = view;
	}

	public void DrawLine(Vector3 p0, Vector3 p1, Color color = default, float width = DefaultWidth)
	{
		lines.Add(new LineData()
		{
			P0 = p0,
			P1 = p1,
			Width = width,
			Color = color == default ? Color.White : color,
		});
	}

	public void DrawBox(Box3D box, Color color = default, float width = DefaultWidth)
	{
		// Vertical lines
		DrawLine(box.BottomLeftNear, box.TopLeftNear, color, width);
		DrawLine(box.BottomRightNear, box.TopRightNear, color, width);
		DrawLine(box.BottomLeftFar, box.TopLeftFar, color, width);
		DrawLine(box.BottomRightFar, box.TopRightFar, color, width);

		// Bottom lines
		DrawLine(box.BottomLeftNear, box.BottomLeftFar, color, width);
		DrawLine(box.BottomLeftFar, box.BottomRightFar, color, width);
		DrawLine(box.BottomRightFar, box.BottomRightNear, color, width);
		DrawLine(box.BottomRightNear, box.BottomLeftNear, color, width);

		// Top lines
		DrawLine(box.TopLeftNear, box.TopLeftFar, color, width);
		DrawLine(box.TopLeftFar, box.TopRightFar, color, width);
		DrawLine(box.TopRightFar, box.TopRightNear, color, width);
		DrawLine(box.TopRightNear, box.TopLeftNear, color, width);
	}

	public void DrawArrow(Vector3 p0, Vector3 p1, float radius, Color color = default, float width = DefaultWidth)
	{
		float headLength = radius * 3;
		Vector3 direction = (p1 - p0).Normalized();

		DrawLine(p0, p1 - (headLength * direction), color, width);
		DrawCone(p1 - (headLength * direction), p1, radius, color);
	}

	public void DrawCone(Vector3 p0, Vector3 p1, float radius, Color color = default, int segments = 16)
	{
		Vector3 axis = (p1 - p0).Normalized();
		Basis(axis, out Vector3 right, out Vector3 up);

		Span<Vector3> verts = stackalloc Vector3[segments + 2];
		Span<uint> tris = stackalloc uint[segments * 6];

		verts[segments] = p1;
		verts[segments + 1] = p0;

		for (int i = 0; i < segments; i++)
		{
			float angle = i / (float)segments * MathF.Tau;
			verts[i] = p0 + (((right * MathF.Cos(angle)) + (up * MathF.Sin(angle))) * radius);
		}

		for (int i = 0; i < segments; i++)
		{
			uint current = (uint)i;
			uint next = (uint)((i + 1) % segments);

			// Side, then the cap that closes the base off.
			tris[(i * 6) + 0] = current;
			tris[(i * 6) + 1] = next;
			tris[(i * 6) + 2] = (uint)segments;
			tris[(i * 6) + 3] = next;
			tris[(i * 6) + 4] = current;
			tris[(i * 6) + 5] = (uint)segments + 1;
		}

		DrawTriangles(verts, tris, color);
	}

	/// <summary>
	/// Draws a circle, optionally keeping only the half of it facing <paramref name="clipFrom"/>.
	/// </summary>
	public void DrawCircle(Vector3 center, Vector3 normal, float radius, Color color = default, float width = DefaultWidth, int segments = 64, Vector3? clipFrom = null)
	{
		Basis(normal.Normalized(), out Vector3 right, out Vector3 up);

		for (int i = 0; i < segments; i++)
		{
			Vector3 from = OnCircle(center, right, up, radius, i / (float)segments);
			Vector3 to = OnCircle(center, right, up, radius, (i + 1) / (float)segments);

			if (clipFrom is Vector3 eye && (FacesAway(from, center, eye) || FacesAway(to, center, eye)))
			{
				continue;
			}

			DrawLine(from, to, color, width);
		}
	}

	/// <summary>
	/// Draws a filled circle, facing along its normal.
	/// </summary>
	public void DrawDisc(Vector3 center, Vector3 normal, float radius, Color color = default, int segments = 64)
	{
		Basis(normal.Normalized(), out Vector3 right, out Vector3 up);

		Span<Vector3> verts = stackalloc Vector3[segments + 1];
		Span<uint> tris = stackalloc uint[segments * 3];

		verts[segments] = center;

		for (int i = 0; i < segments; i++)
		{
			verts[i] = OnCircle(center, right, up, radius, i / (float)segments);

			tris[(i * 3) + 0] = (uint)i;
			tris[(i * 3) + 1] = (uint)((i + 1) % segments);
			tris[(i * 3) + 2] = (uint)segments;
		}

		DrawTriangles(verts, tris, color);
	}

	/// <summary>
	/// Whether a point on a sphere sits on the hemisphere pointing away from the given eye.
	/// </summary>
	public static bool FacesAway(Vector3 point, Vector3 center, Vector3 eye) => Vector3.Dot(point - center, point - eye) >= 0;

	/// <summary>
	/// Draws a flat quad from its center, spanning the two given half-extent vectors.
	/// </summary>
	public void DrawQuad(Vector3 center, Vector3 extentU, Vector3 extentV, Color color = default)
	{
		Span<Vector3> verts =
		[
			center - extentU - extentV,
			center + extentU - extentV,
			center - extentU + extentV,
			center + extentU + extentV,
		];

		DrawTriangles(verts, [0, 1, 2, 2, 1, 3], color);
	}

	public void DrawTriangles(ReadOnlySpan<Vector3> positions, ReadOnlySpan<uint> triangles, Color color = default)
	{
		Guard.Require(triangles.Length % 3 == 0, "Indices passed to DrawTriangles() must be triangles.");

		uint offset = (uint)vertices.Count;
		if (color == default)
		{
			color = Color.White;
		}

		foreach (Vector3 position in positions)
		{
			vertices.Add(new VertexData() { Position = position, Color = color });
		}

		foreach (uint index in triangles)
		{
			indices.Add(offset + index);
		}
	}

	/// <summary>
	/// Uploads everything queued this frame and issues the two batched draws.
	/// </summary>
	internal void Flush()
	{
		Guard.Require(lines.Count <= MaxLines, "Too many gizmo lines queued in one frame.");
		Guard.Require(vertices.Count <= MaxVertices, "Too many gizmo vertices queued in one frame.");

		if (lines.Count > 0)
		{
			renderList.UploadBuffer(lineBuffer, CollectionsMarshal.AsSpan(lines));

			renderList.SetPipelineState(linePSO);
			renderList.SetPipelineCBV(0, 1, viewConstants);
			renderList.SetPipelineSRV(0, 0, lineBuffer);
			renderList.SetPipelineConstants(0, 0, AsInt((float)View.Size.X), AsInt((float)View.Size.Y));
			renderList.DispatchMesh(lines.Count);
		}

		if (indices.Count > 0)
		{
			renderList.UploadBuffer(vertexBuffer, CollectionsMarshal.AsSpan(vertices));
			renderList.UploadBuffer(indexBuffer, CollectionsMarshal.AsSpan(indices));

			renderList.SetPipelineState(geometryPSO);
			renderList.SetPipelineCBV(0, 1, viewConstants);
			renderList.SetPipelineSRV(0, 0, vertexBuffer);
			renderList.SetPipelineSRV(1, 0, indexBuffer);
			renderList.DispatchMesh(indices.Count / 3);
		}

		lines.Clear();
		vertices.Clear();
		indices.Clear();
	}

	internal void FireGizmosEvent()
	{
		OnDrawGizmos?.Invoke(null, this);
	}

	private static Vector3 OnCircle(Vector3 center, Vector3 right, Vector3 up, float radius, float turns)
	{
		float angle = turns * MathF.Tau;
		return center + (((right * MathF.Cos(angle)) + (up * MathF.Sin(angle))) * radius);
	}

	/// <summary>
	/// Completes a unit vector into an orthonormal basis. Which way the pair falls is arbitrary.
	/// </summary>
	public static void Basis(Vector3 normal, out Vector3 right, out Vector3 up)
	{
		Vector3 reference = MathF.Abs(normal.Z) < 0.9f ? Vector3.UnitZ : Vector3.UnitX;

		right = Vector3.Cross(normal, reference).Normalized();
		up = Vector3.Cross(normal, right);
	}

	private static unsafe int AsInt(float value) => *(int*)&value;
}
