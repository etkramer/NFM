#pragma once
#include "meshoptimizer.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace MeshOptimizer
{
	public value struct Meshlet
	{
		unsigned int vertexOffset;
		unsigned int triangleOffset;

		unsigned int vertexCount;
		unsigned int triangleCount;
	};

	public ref class MeshOperations sealed abstract
	{
	public:
		static void BuildMeshlets(unsigned* indices, int numIndices, void* verts, int numVerts, int vertStride,
			[Out] array<unsigned char>^% outPrimIndices, [Out] array<unsigned int>^% outVertIndices, [Out] array<Meshlet>^% outMeshlets);
	};
}