using System;
using System.Linq;

namespace Engine.Resources
{
	/// <summary>
	/// A 3D model, composed of one or multiple parts and optionally a skeleton.
	/// </summary>
	public partial class Model : Resource
	{
		public ModelPart[] Parts { get; set; }
	}

	/// <summary>
	/// A part of a model that contains geometry. Can be toggled on/off within the editor, comparable to a bodygroup in SFM.
	/// </summary>
	public partial class ModelPart
	{
		public Submesh[] Submeshes { get; set; }
	}

	public partial class Submesh
	{
		public Material Material { get; set; }

		public uint[] Triangles { get; set; }
		public Vector3[] Vertices { get; set; }
		public Vector3[] Normals { get; set; }
	}
}