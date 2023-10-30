using NFM.Graphics;
using NFM.Threading;

namespace NFM.Resources;

/// <summary>
/// A 3D model, composed of one or multiple parts and optionally a skeleton.
/// </summary>
public sealed class Model : GameResource
{
	public IReadOnlyCollection<Mesh> Meshes => meshes;
	private List<Mesh> meshes = new();

    protected override void PostLoad()
    {
        foreach (var mesh in meshes)
        {
		    if (mesh.Bounds == Box3D.Infinity)
		    {
			    mesh.PopulateBounds();
		    }

		    mesh.RenderData = new RenderMesh(mesh);
        }

        base.PostLoad();
    }

    public bool AddMesh(Mesh mesh)
	{
        Guard.Require(!IsFullyLoaded, "Cannot modify an already-loaded model");

		if (mesh is not null)
		{
			meshes.Add(mesh);
		}

		return true;
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

public sealed class Mesh : IDisposable
{
	/// <summary>
	/// The name of this mesh, displayed when choosing mesh groups.
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// Should this mesh be visible by default?
	/// </summary>
	public bool IsVisible { get; init; } = true;

    /// <summary>
    /// This mesh's bounding box as displayed in the editor.
    /// </summary>
	public Box3D Bounds { get; set; } = Box3D.Infinity;

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

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Vertex
{
	public Vector3 Position;
	public Vector3 Normal;
	public Vector4 Tangent;
	public Vector2 UV0;
	public Vector2 UV1;
}