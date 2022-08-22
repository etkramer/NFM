#include "MeshOperations.h"
#include <vector>

namespace MeshOptimizer
{
	void MeshOperations::BuildMeshlets(int numIndices, unsigned* indices, int numVerts, void* verts, int vertSize,
		[Out] array<unsigned char>^% outTriIndices, [Out] array<unsigned int>^% outVertIndices, [Out] array<Meshlet>^% outMeshlets)
	{
		const float coneWeight = 0.0f;
		const size_t maxVerts = 64;
		const size_t maxTris = 124;

		// Decide how many meshlets we need to allow for
		size_t maxMeshlets = meshopt_buildMeshletsBound(numIndices, maxVerts, maxTris);

		const int posStart = 0;
		std::vector<meshopt_Meshlet> meshlets(maxMeshlets);
		std::vector<unsigned int> meshletVerts(maxMeshlets * maxVerts);
		std::vector<unsigned char> meshletTris(maxMeshlets * maxTris * 3);

		// Build meshlet data
		size_t meshletCount = meshopt_buildMeshlets(meshlets.data(), meshletVerts.data(), meshletTris.data(), indices, numIndices, (float*)verts + posStart, numVerts, vertSize, maxVerts, maxTris, coneWeight);
		
		// Trim arrays
		const meshopt_Meshlet& last = meshlets[meshletCount - 1];
		meshlets.resize(meshletCount);
		meshletVerts.resize(last.vertex_offset + last.vertex_count);
		meshletTris.resize(last.triangle_offset + ((last.triangle_count * 3 + 3) & ~3));

		// Write to output arrays.
		outMeshlets = gcnew array<Meshlet>(meshlets.size());
		outVertIndices = gcnew array<unsigned int>(meshletVerts.size());
		outTriIndices = gcnew array<unsigned char>(meshletTris.size());
		pin_ptr<Meshlet> outMeshletsPin = &outMeshlets[0];
		pin_ptr<unsigned int> outVertsPin = &outVertIndices[0];
		pin_ptr<unsigned char> outTrisPin = &outTriIndices[0];
		std::memcpy(outMeshletsPin, &meshlets[0], meshlets.size() * sizeof(Meshlet));
		std::memcpy(outVertsPin, &meshletVerts[0], meshletVerts.size() * sizeof(unsigned int));
		std::memcpy(outTrisPin, &meshletTris[0], meshletTris.size() * sizeof(unsigned char));
	}
}