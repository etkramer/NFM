using System;
using System.Linq;
using Engine.GPU;
using Engine.Rendering;

namespace Engine.Resources
{
	/// <summary>
	/// A 3D model, composed of one or multiple parts and optionally a skeleton.
	/// </summary>
	public partial class Model : Resource
	{
		public ModelPart[] Parts { get; set; }

		public bool IsCommitted => Parts?.Any(o => o.Meshes.Any(o2 => o2.IsCommitted)) ?? false;
		public bool IsDeformable => Parts?.Any(o => o.Meshes.Any(o2 => o2.IsDeformable)) ?? false;

		public Model(params Mesh[] meshes)
		{
			Parts = new[]
			{
				new ModelPart()
				{
					Meshes = meshes
				}
			};
		}

		public Model(params ModelPart[] parts)
		{
			Parts = parts;
		}

		public override void Dispose()
		{
			foreach (var part in Parts)
			{
				part.Dispose();
			}

			base.Dispose();
		}
	}

	/// <summary>
	/// A group of one or multiple meshes. Can be toggled on/off within the editor, comparable to a bodygroup in SFM.
	/// </summary>
	public partial class ModelPart : IDisposable
	{
		public Mesh[] Meshes { get; set; }

		public ModelPart(params Mesh[] meshes)
		{
			Meshes = meshes;
		}

		public void Dispose()
		{
			foreach (var mesh in Meshes)
			{
				mesh.Dispose();
			}
		}
	}
}