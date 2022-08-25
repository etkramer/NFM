using System;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;

namespace Engine.Rendering
{
	public class MaterialInstance
	{
		private Material baseMaterial;

		public MaterialInstance(Material baseMaterial)
		{
			this.baseMaterial = baseMaterial;

			// Basic outline:
			// 1. Loop through all shader stacks "used" in the scene (doesn't matter if culled)
			// 2. Generate draw commands for all meshes using that shader stack, then dispatch the shader.
			// 3. Shaders use instance ID to look themselves up in the material buffer, which contains all material parameters (descriptor indexes and constants).

			// 1. Cull shader counts the number of objects using this shader, then uses it in a prefix sum to figure out the shader's range in the command buffer. Can also
			// consult http://filmicworlds.com/blog/visibility-buffer-rendering-with-material-graphs/ for a (non-exact) reference.
			// 2. May need a "Material Start/Count" buffer to feed to DrawIndirect().

			// To do this, we first need to:
			// 1. Maintain a collection of all used shader combinations.
			// 2. Generate an entry into the material buffer for each material instance.
		}
	}
}
