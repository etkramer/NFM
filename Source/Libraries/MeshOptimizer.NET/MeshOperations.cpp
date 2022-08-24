#include "MeshOperations.h"
#include <vector>
#include <iostream>
#include <string>

namespace MeshOptimizer
{
	void MeshOperations::BuildMeshlets(unsigned* indices, int numIndices, void* verts, int numVerts, int vertStride,
		[Out] array<unsigned char>^% outPrimIndices, [Out] array<unsigned int>^% outVertIndices, [Out] array<Meshlet>^% outMeshlets)
	{
		const float coneWeight = 0.0f;
		const size_t maxVerts = 64;
		const size_t maxTris = 124;

		// Decide how many meshlets we need to allow for
		size_t maxMeshlets = meshopt_buildMeshletsBound(numIndices, maxVerts, maxTris);
		std::vector<meshopt_Meshlet> meshlets(maxMeshlets);
		std::vector<unsigned int> meshletVerts(maxMeshlets * maxVerts);
		std::vector<unsigned char> meshletTris(maxMeshlets * maxTris * 3);

		// Build meshlet data
		size_t meshletCount = meshopt_buildMeshlets(meshlets.data(), meshletVerts.data(), meshletTris.data(), indices, numIndices, (float*)verts, numVerts, vertStride, maxVerts, maxTris, coneWeight);
		
		// Trim arrays
		const meshopt_Meshlet& last = meshlets[meshletCount - 1];
		size_t meshletSize = meshletCount;
		size_t meshletVertsSize = last.vertex_offset + last.vertex_count;
		size_t meshletTrisSize = last.triangle_offset + ((last.triangle_count * 3 + 3) & ~3);

		// Create output arrays.
		outMeshlets = gcnew array<Meshlet>(meshletSize);
		outVertIndices = gcnew array<unsigned int>(meshletVertsSize);
		outPrimIndices = gcnew array<unsigned char>(meshletTrisSize);
		pin_ptr<Meshlet> outMeshletsPin = &outMeshlets[0];
		pin_ptr<unsigned int> outVertsPin = &outVertIndices[0];
		pin_ptr<unsigned char> outPrimsPin = &outPrimIndices[0];

		// Write to output arrays.
		std::memcpy(outMeshletsPin, &meshlets[0], meshletSize * sizeof(Meshlet));
		std::memcpy(outVertsPin, &meshletVerts[0], meshletVertsSize * sizeof(unsigned int));
		std::memcpy(outPrimsPin, &meshletTris[0], meshletTrisSize * sizeof(unsigned char));
	}
}