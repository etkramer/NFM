namespace NFM.World;

[Icon("photo_camera")]
public class CameraNode : Node
{
	// The pixel PickingStep should sample this frame, or null to skip picking entirely.
	internal Vector2i? PickCoords { get; set; }

	// Instance buffer offset under PickCoords, or -1 for empty space. Trails PickCoords by a frame or two.
	internal int HoveredInstance { get; set; } = -1;

	[Inspect]
	public uint FocalLength { get; set; } = 35;

	[Inspect]
	public uint SensorSize { get; set; } = 36;

	[Inspect]
	public float Exposure { get; set; } = 1;

	public float FOV => (2 * (float)Math.Atan(SensorSize / 2f / FocalLength)).ToDegrees();

	public CameraNode(Scene? scene) : base(scene)
	{
		Name = "Camera";
	}
}