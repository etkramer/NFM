namespace NFM.World;

/// <summary>
/// One bone of a <see cref="ModelNode"/>'s skeleton. Shouldn't be spawned directly.
/// </summary>
[Icon("polyline")]
public class BoneNode : Node
{
	/// <summary>
	/// Index of this bone into its model's skeleton.
	/// </summary>
	public int Index { get; internal set; } = -1;

	public BoneNode(Scene? scene) : base(scene)
	{
		Name = "Bone";
	}
}
