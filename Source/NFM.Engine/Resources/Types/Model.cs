using System;
using System.Collections;
using Avalonia.Controls;
using NFM.Graphics;

namespace NFM.Resources;

/// <summary>
/// A 3D model, composed of one or multiple parts and optionally a skeleton.
/// </summary>
public sealed class Model : GameResource
{
	public IEnumerable<Mesh> Meshes => meshes;
	private List<Mesh> meshes = new();

	public bool IsCommitted => meshes.All(o => o.IsCommitted);

	public bool AddMesh(Mesh mesh)
	{
		if (mesh != null)
		{
			meshes.Add(mesh);
			mesh.Commit();
		}

		return true;
	}

	public override void Dispose()
	{
		foreach (var  mesh in Meshes)
		{
			mesh.Dispose();
		}

		base.Dispose();
	}
}

public sealed class Mesh
{
	/// <summary>
	/// The name of this mesh, displayed when choosing mesh groups.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Does this mesh need to be reuploaded?
	/// </summary>
	public bool IsCommitted { get; private set; } = false;

	/// <summary>
	/// Should this mesh be visible by default?
	/// </summary>
	public bool IsVisible { get; private set; } = true;

	public int NumLODs { get; private set; } = 0;

	public Box3D Bounds { get; set; } = Box3D.Infinity;
	public uint[][] Indices { get; private set; } = new uint[0][];
	public Vertex[][] Vertices { get; private set; } = new Vertex[0][];
	public Material[] Materials { get; private set; } = new Material[0];

	internal RenderMesh RenderData = null;

	public Mesh(string name)
	{
		Name = name;
	}

	public void SetIndices(uint[] indices, int lod = 0)
	{
		CheckLOD(lod);
		Indices[lod] = indices;
		IsCommitted = false;
	}

	public void SetVertices(Vertex[] vertices, int lod = 0)
	{
		CheckLOD(lod);
		Vertices[lod] = vertices;
		IsCommitted = false;
	}

	public void SetMaterial(Material material, int lod = 0)
	{
		CheckLOD(lod);
		Materials[lod] = material;
		IsCommitted = false;
	}

	private void CheckLOD(int lod)
	{
		Debug.Assert(lod <= 4, "Meshes may not contain more than five LOD levels (up to LOD4)");

		if (lod >= NumLODs)
		{
			NumLODs = lod + 1;
			
			var vertices = Vertices;
			var indices = Indices;
			var material = Materials;
			Array.Resize(ref vertices, NumLODs);
			Array.Resize(ref indices, NumLODs);
			Array.Resize(ref material, NumLODs);
			Vertices = vertices;
			Indices = indices;
			Materials = material;
		}
	}

	public void Commit()
	{
		if (IsCommitted)
		{
			return;
		}

		if (RenderData != null)
		{
			RenderData.Dispose();
		}

		if (Bounds == Box3D.Infinity)
		{
			PopulateBounds();
		}

		RenderData = new RenderMesh(this);
		IsCommitted = true;
	}

	public void Dispose()
	{
		RenderData.Dispose();
	}

	/// <summary>
	/// Automatically generates 3D bounds
	/// </summary>
	void PopulateBounds()
	{
		Vector3 min = Vector3.PositiveInfinity;
		Vector3 max = Vector3.NegativeInfinity;

		// Nothing fancy, just loop over every vert and
		// compare to the current min/max values.
		for (int i = 0; i < Vertices.Length; i++)
		{
			var vert = Vertices[0][i];

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