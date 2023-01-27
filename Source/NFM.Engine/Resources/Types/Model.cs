﻿using System;
using System.Collections;
using MeshOptimizer;

namespace NFM.Resources;

/// <summary>
/// A 3D model, composed of one or multiple parts and optionally a skeleton.
/// </summary>
public class Model : GameResource
{
	public IEnumerable<MeshGroup> MeshGroups => meshGroups;
	private List<MeshGroup> meshGroups = new();

	public IEnumerable<Mesh> Meshes => meshGroups.SelectMany(o => o.Meshes).Where(o => o != null);
	public bool IsCommitted => Meshes.All(o => o.IsCommitted);

	public bool AddMeshGroup(string groupName, IEnumerable<Mesh> meshes, Mesh defaultSelection = null)
	{
		foreach (var mesh in meshes)
		{
			mesh?.Commit();
		}

		MeshGroup group = new MeshGroup(groupName, meshes, defaultSelection);
		meshGroups.Add(group);

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

public class MeshGroup
{
	/// <summary>
	/// The name of this group, displayed when selecting group members.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// The group member (mesh) that should be selected by default.
	/// </summary>
	public Mesh DefaultSelection { get; }

	public IEnumerable<Mesh> Meshes => meshes;
	private Mesh[] meshes;

	internal MeshGroup(string name, IEnumerable<Mesh> meshes, Mesh defaultSelection = null)
	{
		Debug.Assert(meshes != null && meshes?.Count() > 0, "Tried to assign a null or empty collection to a MeshGroup");
		Debug.Assert(meshes.Contains(defaultSelection), "Tried to set DefaultSelection to a mesh that does not exist in the MeshGroup");

		Name = name;
		DefaultSelection = defaultSelection;
		this.meshes = meshes.ToArray();
	}
}

public partial class Mesh : IDisposable
{
	/// <summary>
	/// The name of this mesh, displayed when choosing mesh groups.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Does this mesh need to be reuploaded?
	/// </summary>
	public bool IsCommitted { get; private set; } = false;

	public Box3D Bounds { get; set; } = Box3D.Infinity;
	public uint[] Indices { get; private set; }
	public Vertex[] Vertices { get; private set; }
	public Material Material { get; private set; }

	public void SetIndices(uint[] indices)
	{
		Indices = indices;
		IsCommitted = false;
	}

	public void SetVertices(Vertex[] vertices)
	{
		Vertices = vertices;
		IsCommitted = false;
	}

	public void SetMaterial(Material material)
	{
		Material = material;
		IsCommitted = false;
	}

	public Mesh(string name)
	{
		Name = name;
	}
}