using NFM.Graphics;

namespace NFM.Resources;

/// <summary>
/// A 3D model, composed of one or multiple parts and optionally a skeleton.
/// </summary>
[Icon("view_in_ar", IconColor = "#7ec97e")]
public sealed class Model : GameResource
{
    /// <summary>
    /// Enumerates all meshes owned by the model.
    /// </summary>
	public IEnumerable<Mesh> Meshes => MeshGroups.SelectMany(group => group.Meshes);

    /// <summary>
    /// Collection of all mesh groups in the model.
    /// </summary>
    public IReadOnlyCollection<MeshGroup> MeshGroups => meshGroups;
    private readonly List<MeshGroup> meshGroups = [];

    /// <summary>
    /// Skeleton this model's skinned meshes are posed by, if it has one.
    /// </summary>
    public Skeleton? Skeleton { get; private set; }

    protected override void PostLoad()
    {
        foreach (var mesh in Meshes)
        {
		    if (mesh.Bounds == Box3D.Infinity)
		    {
			    mesh.PopulateBounds();
		    }

		    mesh.RenderData = new RenderMesh(mesh);
        }

        base.PostLoad();
    }

    public void AddMeshGroup(Mesh mesh, string groupName) => AddMeshGroup(new MeshGroup()
    {
        Name = groupName,
        Meshes = [mesh]
    });

    public void AddMeshGroup(MeshGroup meshGroup)
	{
        Guard.Require(!IsFullyLoaded, "Cannot modify an already-loaded model");
		meshGroups.Add(meshGroup);
	}

	public void SetSkeleton(Skeleton skeleton)
	{
		Guard.Require(!IsFullyLoaded, "Cannot modify an already-loaded model");
		Skeleton = skeleton;
	}

	public override void Dispose()
	{
		foreach (var mesh in Meshes)
		{
			mesh.Dispose();
		}

		base.Dispose();
	}
}

/// <summary>
/// A model's bone hierarchy. Bones are ordered so a parent always precedes its children.
/// </summary>
public sealed class Skeleton
{
	public required Bone[] Bones { get; init; }

	public int IndexOf(string name) => Array.FindIndex(Bones, bone => bone.Name == name);
}

public readonly struct Bone
{
	public required string Name { get; init; }

	/// <summary>
	/// Index of this bone's parent, or -1 if it's a root.
	/// </summary>
	public required int ParentIndex { get; init; }

	/// <summary>
	/// Rest transform, relative to the parent bone.
	/// </summary>
	public required Vector3 Position { get; init; }
	public required Vector3 Rotation { get; init; }
	public required Vector3 Scale { get; init; }

	/// <summary>
	/// Brings a vertex from model space into this bone's space, at the bind pose.
	/// </summary>
	public required Matrix4 InverseBind { get; init; }
}

public sealed class MeshGroup : IDisposable
{
	/// <summary>
	/// The name of this group, displayed in the editor.
	/// </summary>
	public required string Name { get; init; }

    /// <summary>
    /// Collection of meshes to include in this group
    /// </summary>
    public required Mesh[] Meshes { get; init; }

	/// <summary>
	/// Should this group be visible by default?
	/// </summary>
	public bool IsVisible { get; init; } = true;

    public void Dispose() => Meshes.ForEach(o => o.Dispose());
}

public sealed class Mesh : IDisposable
{
    /// <summary>
    /// Triangle indices to be used by the renderer.
    /// </summary>
	public required uint[]? Indices { get; init; } = null;

    /// <summary>
    /// Vertex information to be used by the renderer.
    /// </summary>
	public required Vertex[]? Vertices { get; init; } = null;

    /// <summary>
    /// Material to be used by default (can be overriden in editor).
    /// </summary>
	public required Material? Material { get; init; } = null;

    /// <summary>
    /// Bone influences, parallel to <see cref="Vertices"/>. Null on unskinned meshes.
    /// </summary>
	public VertexWeights[]? Weights { get; init; } = null;

	public bool IsSkinned => Weights is not null;

    /// <summary>
    /// Mask of LOD levels at which this mesh should be visible.
    /// </summary>
    public LODLevel LODMask { get; init; } = LODLevel.LOD0;

    /// <summary>
    /// This mesh's bounding box as displayed in the editor.
    /// </summary>
	public Box3D Bounds { get; set; } = Box3D.Infinity;

	internal RenderMesh? RenderData = null;

	public void Dispose()
	{
		RenderData?.Dispose();
	}

	/// <summary>
	/// Automatically generates 3D bounds
	/// </summary>
	internal void PopulateBounds()
	{
        if (Vertices is null)
        {
            return;
        }

		Vector3 min = Vector3.PositiveInfinity;
		Vector3 max = Vector3.NegativeInfinity;

		// Nothing fancy, just loop over every vert and
		// compare to the current min/max values.
		for (int i = 0; i < Vertices.Length; i++)
		{
			var vert = Vertices[i];

			// Update minimums.
			if (vert.Position.X < min.X)
			{
				min.X = vert.Position.X;
			}
			if (vert.Position.Y < min.Y)
			{
				min.Y = vert.Position.Y;
			}
			if (vert.Position.Z < min.Z)
			{
				min.Z = vert.Position.Z;
			}

			// Update maximums.
			if (vert.Position.X > max.X)
			{
				max.X = vert.Position.X;
			}
			if (vert.Position.Y > max.Y)
			{
				max.Y = vert.Position.Y;
			}
			if (vert.Position.Z > max.Z)
			{
				max.Z = vert.Position.Z;
			}
		}

		Bounds = new Box3D(min, max);
	}
}

[Flags]
public enum LODLevel
{
    LOD0 = 1 << 0,
    LOD1 = 1 << 1,
    LOD2 = 1 << 2,
    LOD3 = 1 << 3,
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Vertex
{
	public Vector3 Position;
	public Vector3 Normal;
	public Vector4 Tangent;
	public Vector2 UV0;
	public Vector2 UV1;
}

/// <summary>
/// Bone influences for a single vertex. Weights are unsigned-normalized and sum to one.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct VertexWeights
{
	public const int MaxInfluences = 4;

	public fixed ushort Indices[MaxInfluences];
	public fixed ushort Weights[MaxInfluences];

	/// <summary>
	/// Keeps the heaviest influences, normalizes them, and packs the result.
	/// </summary>
	public static VertexWeights FromInfluences(List<(int Bone, float Weight)> influences)
	{
		influences.Sort((a, c) => c.Weight.CompareTo(a.Weight));

		int count = Math.Min(influences.Count, MaxInfluences);
		float total = 0;

		for (int i = 0; i < count; i++)
		{
			total += influences[i].Weight;
		}

		VertexWeights result = new();
		if (total <= 0)
		{
			return result;
		}

		// Distributed against a running total, so the packed weights always sum to exactly one.
		ushort assigned = 0;
		float accumulated = 0;

		for (int i = 0; i < count; i++)
		{
			accumulated += influences[i].Weight / total;

			ushort packed = (ushort)Math.Clamp(MathF.Round(accumulated * ushort.MaxValue), 0, ushort.MaxValue);

			result.Indices[i] = (ushort)influences[i].Bone;
			result.Weights[i] = (ushort)(packed - assigned);

			assigned = packed;
		}

		return result;
	}
}