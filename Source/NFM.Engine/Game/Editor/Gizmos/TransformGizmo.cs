using NFM.Graphics;
using NFM.World;

namespace NFM;

public enum GizmoMode
{
	Translate,
	Rotate,
}

/// <summary>
/// One grabbable part of a transform gizmo. Axis handles move or spin along a world axis, plane handles
/// move within one, Screen is the view-aligned handle, and Center is the trackball inside it.
/// </summary>
public enum GizmoHandle
{
	None,
	AxisX,
	AxisY,
	AxisZ,
	PlaneX,
	PlaneY,
	PlaneZ,
	Screen,
	Center,
}

/// <summary>
/// The move/rotate gizmo for one viewport's selection. Laid out in screen pixels so it stays the same
/// size however far away the selection is, and hit-tested on the CPU so a grab lands the frame it happens.
/// </summary>
public class TransformGizmo
{
	// Screen-space layout, in pixels.
	private const float ShaftStart = 24;
	private const float ShaftEnd = 117;
	private const float HeadRadius = 8.25f;
	private const float PlaneOffset = 51;
	private const float PlaneRadius = 8;
	private const float RingRadius = 111;
	private const float ScreenRingRadius = 138;
	private const float CenterRadius = 18;
	private const float PickPadding = 12;

	private const float TrackballAlpha = 0.15f;

	private const float LineWidth = 2;
	private const float RingWidth = 3;
	private const int RingSegments = 96;

	public GizmoMode Mode { get; set; } = GizmoMode.Translate;

	public GizmoHandle Hovered { get; private set; }
	public GizmoHandle Dragging { get; private set; }
	public bool IsDragging => Dragging != GizmoHandle.None;

	private readonly Viewport viewport;

	private GizmoView view;
	private bool hasView;

	private Vector3 pivot;
	private bool hasPivot;

	// Captured at the moment of the grab, so a drag stays anchored to where it started.
	private Vector3 dragPivot;
	private Vector3 dragAxis;
	private Vector3 grabPoint;
	private Vector3 grabSphere;
	private float grabAngle;

	private readonly record struct Target(Node Node, Matrix4 World, Matrix4 ParentInverse);
	private readonly List<Target> targets = [];

	// Held open for the length of a drag, so the whole thing undoes in one step.
	private Transaction? edit;

	public TransformGizmo(Viewport viewport)
	{
		this.viewport = viewport;
	}

	private static bool IsAxis(GizmoHandle handle) => handle is >= GizmoHandle.AxisX and <= GizmoHandle.AxisZ;

	private static int AxisIndex(GizmoHandle handle) =>
		handle >= GizmoHandle.PlaneX ? handle - GizmoHandle.PlaneX : handle - GizmoHandle.AxisX;

	#region Drawing

	/// <summary>
	/// Refreshes the cached view, updates hover, and queues this frame's geometry.
	/// </summary>
	public void Draw(Gizmos context)
	{
		view = context.View;
		hasView = true;

		hasPivot = TryGetPivot(out pivot);
		if (!hasPivot)
		{
			Hovered = GizmoHandle.None;
			return;
		}

		if (!IsDragging)
		{
			Hovered = viewport.CursorPosition is Vector2i cursor ? Pick(new Vector2(cursor.X, cursor.Y)) : GizmoHandle.None;
		}

		if (Mode == GizmoMode.Translate)
		{
			DrawTranslate(context);
		}
		else
		{
			DrawRotate(context);
		}
	}

	private void DrawTranslate(Gizmos context)
	{
		float unit = view.PixelSize(pivot);

		for (int i = 0; i < 3; i++)
		{
			Vector3 axis = Gizmos.AxisDirections[i];
			context.DrawArrow(pivot + (axis * ShaftStart * unit), pivot + (axis * ShaftEnd * unit),
				HeadRadius * unit, ColorOf(GizmoHandle.AxisX + i, i), LineWidth);

			Vector3 center = PlaneCenter(i, unit);
			Vector3 u = Gizmos.AxisDirections[(i + 1) % 3] * PlaneRadius * unit;
			Vector3 v = Gizmos.AxisDirections[(i + 2) % 3] * PlaneRadius * unit;
			Color color = ColorOf(GizmoHandle.PlaneX + i, i);

			context.DrawQuad(center, u, v, new Color(color.R, color.G, color.B, 0.5f));
			context.DrawLine(center - u - v, center + u - v, color, LineWidth);
			context.DrawLine(center + u - v, center + u + v, color, LineWidth);
			context.DrawLine(center + u + v, center - u + v, color, LineWidth);
			context.DrawLine(center - u + v, center - u - v, color, LineWidth);
		}

		bool active = Hovered == GizmoHandle.Screen || Dragging == GizmoHandle.Screen;
		context.DrawCircle(pivot, view.Forward, CenterRadius * unit, active ? Gizmos.Highlight : Color.White, RingWidth, segments: 48);
	}

	private void DrawRotate(Gizmos context)
	{
		float unit = view.PixelSize(pivot);

		if (Hovered == GizmoHandle.Center || Dragging == GizmoHandle.Center)
		{
			context.DrawDisc(pivot, view.Forward, RingRadius * unit, new Color(1, 1, 1, TrackballAlpha));
		}

		// Only the arcs on the near side of the sphere are drawn.
		for (int i = 0; i < 3; i++)
		{
			context.DrawCircle(pivot, Gizmos.AxisDirections[i], RingRadius * unit,
				ColorOf(GizmoHandle.AxisX + i, i), RingWidth, segments: RingSegments, clipFrom: view.EyePosition);
		}

		context.DrawCircle(pivot, view.Forward, ScreenRingRadius * unit,
			Hovered == GizmoHandle.Screen || Dragging == GizmoHandle.Screen ? Gizmos.Highlight : Color.White,
			RingWidth, segments: RingSegments);
	}

	private Color ColorOf(GizmoHandle handle, int axisIndex)
	{
		bool active = IsDragging ? Dragging == handle : Hovered == handle;
		return active ? Gizmos.Highlight : Gizmos.AxisColors[axisIndex];
	}

	#endregion

	#region Picking

	/// <summary>
	/// Finds the handle under a viewport pixel, innermost first so the small handles stay reachable
	/// where they sit on top of the arrows.
	/// </summary>
	private GizmoHandle Pick(Vector2 cursor)
	{
		if (!view.TryToScreen(pivot, out Vector2 center))
		{
			return GizmoHandle.None;
		}

		float unit = view.PixelSize(pivot);

		return Mode == GizmoMode.Translate
			? PickTranslate(cursor, center, unit)
			: PickRotate(cursor, center, unit);
	}

	private GizmoHandle PickTranslate(Vector2 cursor, Vector2 center, float unit)
	{
		if ((cursor - center).Length <= CenterRadius)
		{
			return GizmoHandle.Screen;
		}

		for (int i = 0; i < 3; i++)
		{
			if (view.TryToScreen(PlaneCenter(i, unit), out Vector2 handle) && (cursor - handle).Length <= MathF.Max(PlaneRadius, PickPadding))
			{
				return GizmoHandle.PlaneX + i;
			}
		}

		GizmoHandle best = GizmoHandle.None;
		float bestDistance = PickPadding;

		for (int i = 0; i < 3; i++)
		{
			Vector3 axis = Gizmos.AxisDirections[i];

			if (!view.TryToScreen(pivot + (axis * ShaftStart * unit), out Vector2 from) ||
				!view.TryToScreen(pivot + (axis * ShaftEnd * unit), out Vector2 to))
			{
				continue;
			}

			float distance = DistanceToSegment(cursor, from, to);
			if (distance < bestDistance)
			{
				bestDistance = distance;
				best = GizmoHandle.AxisX + i;
			}
		}

		return best;
	}

	private GizmoHandle PickRotate(Vector2 cursor, Vector2 center, float unit)
	{
		GizmoHandle best = GizmoHandle.None;
		float bestDistance = PickPadding;

		for (int i = 0; i < 3; i++)
		{
			float distance = DistanceToRing(cursor, Gizmos.AxisDirections[i], RingRadius * unit, true);
			if (distance < bestDistance)
			{
				bestDistance = distance;
				best = GizmoHandle.AxisX + i;
			}
		}

		if (DistanceToRing(cursor, view.Forward, ScreenRingRadius * unit, false) < bestDistance)
		{
			return GizmoHandle.Screen;
		}

		if (best != GizmoHandle.None)
		{
			return best;
		}

		return (cursor - center).Length <= RingRadius ? GizmoHandle.Center : GizmoHandle.None;
	}

	/// <summary>
	/// Closest approach between the cursor and a world-space circle, measured along its screen projection.
	/// </summary>
	private float DistanceToRing(Vector2 cursor, Vector3 normal, float radius, bool clip)
	{
		Gizmos.Basis(normal.Normalized(), out Vector3 right, out Vector3 up);

		float best = float.MaxValue;
		Vector2 previous = default;
		bool hasPrevious = false;

		for (int i = 0; i <= RingSegments; i++)
		{
			float angle = i / (float)RingSegments * MathF.Tau;
			Vector3 point = pivot + (((right * MathF.Cos(angle)) + (up * MathF.Sin(angle))) * radius);

			if (clip && Gizmos.FacesAway(point, pivot, view.EyePosition))
			{
				hasPrevious = false;
				continue;
			}

			if (!view.TryToScreen(point, out Vector2 screen))
			{
				hasPrevious = false;
				continue;
			}

			if (hasPrevious)
			{
				best = MathF.Min(best, DistanceToSegment(cursor, previous, screen));
			}

			previous = screen;
			hasPrevious = true;
		}

		return best;
	}

	private static float DistanceToSegment(Vector2 point, Vector2 a, Vector2 b)
	{
		Vector2 offset = b - a;
		float lengthSquared = offset.LengthSquared;
		float along = lengthSquared < 1e-6f ? 0 : Math.Clamp(Vector2.Dot(point - a, offset) / lengthSquared, 0, 1);

		return (point - (a + (offset * along))).Length;
	}

	#endregion

	#region Dragging

	/// <summary>
	/// Grabs whatever the cursor is over. Returns true when the click belongs to the gizmo, so the
	/// viewport knows not to treat it as a selection.
	/// </summary>
	public bool TryBeginDrag()
	{
		if (!hasView || !hasPivot || viewport.CursorPosition is not Vector2i cursorPosition)
		{
			return false;
		}

		Vector2 cursor = new(cursorPosition.X, cursorPosition.Y);

		GizmoHandle handle = Pick(cursor);
		Hovered = handle;

		if (handle == GizmoHandle.None)
		{
			return false;
		}

		Ray ray = view.ScreenRay(cursor);

		dragPivot = pivot;
		dragAxis = handle is GizmoHandle.Screen or GizmoHandle.Center ? view.Forward : Gizmos.AxisDirections[AxisIndex(handle)];

		if (Mode == GizmoMode.Translate)
		{
			bool grabbed = IsAxis(handle)
				? ray.ClosestPointOnLine(dragPivot, dragAxis, out grabPoint)
				: ray.IntersectPlane(dragPivot, dragAxis, out grabPoint);

			if (!grabbed)
			{
				return false;
			}
		}
		else if (handle == GizmoHandle.Center)
		{
			grabSphere = SphereVector(cursor);
		}
		else if (!TryRingAngle(ray, dragAxis, out grabAngle))
		{
			return false;
		}

		targets.Clear();
		foreach (ISelectable item in Selection.Selected)
		{
			if (item is Node node)
			{
				targets.Add(new Target(node, node.WorldTransform,
					node.Parent is null ? Matrix4.Identity : node.Parent.WorldTransform.Inverse()));
			}
		}

		if (targets.Count == 0)
		{
			return false;
		}

		edit = History.Begin(Mode == GizmoMode.Translate ? "Move" : "Rotate");

		foreach (Target target in targets)
		{
			History.Track(target.Node);
		}

		Dragging = handle;
		return true;
	}

	/// <summary>
	/// Re-solves the drag against the current cursor. Absolute rather than incremental, so the grab
	/// point stays glued to the cursor no matter how the frames land.
	/// </summary>
	public void Update()
	{
		if (!IsDragging || viewport.CursorPosition is not Vector2i cursorPosition)
		{
			return;
		}

		Vector2 cursor = new(cursorPosition.X, cursorPosition.Y);
		Ray ray = view.ScreenRay(cursor);

		if (Mode == GizmoMode.Translate)
		{
			UpdateTranslate(ray);
		}
		else if (Dragging == GizmoHandle.Center)
		{
			UpdateTrackball(cursor);
		}
		else if (TryRingAngle(ray, dragAxis, out float angle))
		{
			Apply(RotationAbout(dragAxis, angle - grabAngle), true);
		}
	}

	public void EndDrag()
	{
		edit?.Dispose();
		edit = null;

		Dragging = GizmoHandle.None;
		targets.Clear();
	}

	private void UpdateTranslate(Ray ray)
	{
		Vector3 point;

		if (IsAxis(Dragging))
		{
			if (!ray.ClosestPointOnLine(dragPivot, dragAxis, out point))
			{
				return;
			}
		}
		else if (!ray.IntersectPlane(dragPivot, dragAxis, out point))
		{
			return;
		}

		Apply(Matrix4.CreateTranslation(point - grabPoint), false);
	}

	private void UpdateTrackball(Vector2 cursor)
	{
		Vector3 current = SphereVector(cursor);
		Vector3 axis = Vector3.Cross(grabSphere, current);

		if (axis.Length < 1e-5f)
		{
			return;
		}

		float angle = MathF.Atan2(axis.Length, Vector3.Dot(grabSphere, current));
		Apply(RotationAbout(axis.Normalized(), angle), true);
	}

	/// <summary>
	/// Where the cursor lands on a virtual sphere centered on the pivot. Past the sphere's edge it
	/// continues onto a hyperbolic sheet, so dragging away keeps turning instead of saturating.
	/// </summary>
	private Vector3 SphereVector(Vector2 cursor)
	{
		if (!view.TryToScreen(dragPivot, out Vector2 center))
		{
			return Vector3.UnitZ;
		}

		view.ScreenBasis(out Vector3 right, out Vector3 down);

		Vector2 offset = (cursor - center) / RingRadius;
		float lengthSquared = offset.LengthSquared;
		float depth = lengthSquared <= 0.5f ? MathF.Sqrt(1 - lengthSquared) : 0.5f / MathF.Sqrt(lengthSquared);

		return ((right * offset.X) + (down * offset.Y) - (view.Forward * depth)).Normalized();
	}

	private bool TryRingAngle(Ray ray, Vector3 normal, out float angle)
	{
		angle = 0;
		if (!ray.IntersectPlane(dragPivot, normal, out Vector3 hit))
		{
			return false;
		}

		Gizmos.Basis(normal, out Vector3 right, out Vector3 up);
		Vector3 offset = hit - dragPivot;
		angle = MathF.Atan2(Vector3.Dot(offset, up), Vector3.Dot(offset, right));

		return true;
	}

	private Matrix4 RotationAbout(Vector3 axis, float angle) =>
		Matrix4.CreateTranslation(-dragPivot) * Matrix4.CreateFromAxisAngle(axis, angle) * Matrix4.CreateTranslation(dragPivot);

	/// <summary>
	/// Rebuilds each target from the transform it had when the drag began, so repeated updates can't
	/// accumulate error.
	/// </summary>
	private void Apply(Matrix4 delta, bool rotates)
	{
		foreach (Target target in targets)
		{
			Matrix4 local = target.World * delta * target.ParentInverse;
			target.Node.Position = local.ExtractTranslation();

			if (rotates)
			{
				Vector3 euler = local.ExtractEulerAngles();
				target.Node.Rotation = new Vector3(euler.X.ToDegrees(), euler.Y.ToDegrees(), euler.Z.ToDegrees());
			}
		}
	}

	#endregion

	/// <summary>
	/// The median of the selection's world origins.
	/// </summary>
	private static bool TryGetPivot(out Vector3 result)
	{
		result = Vector3.Zero;
		int count = 0;

		foreach (ISelectable item in Selection.Selected)
		{
			if (item is Node node)
			{
				result += node.WorldTransform.ExtractTranslation();
				count++;
			}
		}

		if (count == 0)
		{
			return false;
		}

		result /= count;
		return true;
	}

	private Vector3 PlaneCenter(int axis, float unit) =>
		pivot + ((Gizmos.AxisDirections[(axis + 1) % 3] + Gizmos.AxisDirections[(axis + 2) % 3]) * PlaneOffset * unit);
}
