#pragma once
#include "meshoptimizer.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace MeshOptimizer
{


	public ref class MeshOperations sealed abstract
	{
	public:
		static void BuildMeshlets(unsigned* indices, int numIndices, int numVerts,
			[Out] array<unsigned char>^% outPrimIndices, [Out] array<unsigned int>^% outVertIndices, [Out] array<Meshlet>^% outMeshlets);
	};
}