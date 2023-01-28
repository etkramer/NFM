#include "src/meshoptimizer.h"

#define EXPORT extern "C" __declspec(dllexport)

EXPORT size_t buildMeshletsBound(size_t index_count, size_t max_vertices, size_t max_triangles)
{
	return meshopt_buildMeshletsBound(index_count, max_vertices, max_triangles);
}

EXPORT size_t buildMeshletsScan(meshopt_Meshlet* meshlets, unsigned int* meshlet_vertices, unsigned char* meshlet_triangles, const unsigned int* indices, size_t index_count, size_t vertex_count, size_t max_vertices, size_t max_triangles)
{
	return meshopt_buildMeshletsScan(meshlets, meshlet_vertices, meshlet_triangles, indices, index_count, vertex_count, max_vertices, max_triangles);
}