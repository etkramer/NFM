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
		public Face[] Faces { get; set; }
		public Vector3[] Positions { get; set; }
		public Vector3[] Normals { get; set; }
	}

	public struct Face
	{
		public uint A;
		public uint B;
		public uint C;
		public Material Material;
	}
}