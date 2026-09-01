namespace NFM.Graphics;

/// <summary>
/// A frozen view transform, enough to move between world space and viewport pixels. Cheap to copy and
/// free of any GPU state, so interaction code can hold onto last frame's copy and hit-test against it.
/// </summary>
public readonly struct GizmoView
{
	/// <summary>Viewport size in pixels. Screen coordinates run from (0, 0) at the top left.</summary>
	public Vector2i Size { get; }

	public Vector3 EyePosition { get; }

	private readonly Matrix4 worldToClip;
	private readonly Matrix4 clipToWorld;

	// World units spanned by one pixel, per unit of clip-space W.
	private readonly float pixelScale;

	public GizmoView(Matrix4 view, Matrix4 projection, Vector3 eyePosition, Vector2i size, float verticalFov)
	{
		worldToClip = view * projection;
		clipToWorld = worldToClip.Inverse();

		EyePosition = eyePosition;
		Size = size;
		pixelScale = 2 * MathF.Tan(verticalFov.ToRadians() * 0.5f) / size.Y;
	}

	/// <summary>
	/// Projects a world point to viewport pixels. Fails for anything at or behind the eye.
	/// </summary>
	public bool TryToScreen(Vector3 world, out Vector2 screen)
	{
		Vector4 clip = new Vector4(world, 1) * worldToClip;
		if (clip.W < 1e-4f)
		{
			screen = default;
			return false;
		}

		Vector2 ndc = clip.Xy / clip.W;
		screen = new Vector2((ndc.X + 1) * 0.5f * Size.X, (1 - ndc.Y) * 0.5f * Size.Y);

		return true;
	}

	/// <summary>
	/// World units covered by one pixel at the given point - the factor that keeps a gizmo the same
	/// size on screen however far away it is.
	/// </summary>
	public float PixelSize(Vector3 world)
	{
		Vector4 clip = new Vector4(world, 1) * worldToClip;
		return MathF.Max(clip.W, 1e-4f) * pixelScale;
	}

	/// <summary>
	/// The world-space direction the view points in.
	/// </summary>
	public Vector3 Forward => ScreenRay(new Vector2(Size.X, Size.Y) * 0.5f).Direction;

	/// <summary>
	/// The world-space directions a pixel step right and a pixel step down point in.
	/// </summary>
	public void ScreenBasis(out Vector3 right, out Vector3 down)
	{
		Vector2 center = new Vector2(Size.X, Size.Y) * 0.5f;
		Vector3 forward = Forward;

		right = (ScreenRay(center + Vector2.UnitX).Direction - forward).Normalized();
		down = (ScreenRay(center + Vector2.UnitY).Direction - forward).Normalized();
	}

	/// <summary>
	/// Builds the world-space ray through a viewport pixel.
	/// </summary>
	public Ray ScreenRay(Vector2 screen)
	{
		Vector2 ndc = new((screen.X / Size.X * 2) - 1, 1 - (screen.Y / Size.Y * 2));

		Vector4 target = new Vector4(ndc.X, ndc.Y, 0.5f, 1) * clipToWorld;
		return new Ray(EyePosition, (target.Xyz / target.W) - EyePosition);
	}
}
