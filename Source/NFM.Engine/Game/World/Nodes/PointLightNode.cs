using NFM.Graphics;

namespace NFM.World;

[Icon("emoji_objects")]
public class PointLightNode : LightNode
{
	/// <summary>
	/// The light's intensity in lumens
	/// </summary>
	[Inspect]
	public float Intensity { get; set; } = 1000;

	/// <summary>
	/// The light's radius in meters
	/// </summary>
	[Inspect]
	public float Radius { get; set; } = 0.2f;

	public PointLightNode(Scene? scene) : base(scene)
	{
		Name = "Point Light";

		this.SubscribeFast(nameof(Intensity), nameof(Radius), MarkDirty);
	}

	protected override GPULight Pack()
	{
		// Lumens to candela, for a point emitting over the full sphere.
		float candela = Intensity / (4 * MathF.PI);

		return new GPULight()
		{
			Type = GPULight.Point,
			Position = WorldTransform.ExtractTranslation(),
			Color = Color * candela,
			Radius = Radius,
		};
	}
}
