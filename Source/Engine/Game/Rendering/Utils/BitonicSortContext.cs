using System;
using Engine.GPU;

namespace Engine.Rendering
{
	public class BitonicSortContext
	{
		private static ShaderProgram indirectProgram;
		private static ShaderProgram preSortProgram;
		private static ShaderProgram innerSortProgram;
		private static ShaderProgram outerSortProgram;

		static BitonicSortContext()
		{
			indirectProgram = new ShaderProgram()
				.UseIncludes(typeof(Game).Assembly)
				.SetComputeShader(Embed.GetString("Content/Shaders/BitonicSort/BitonicIndirectCS.hlsl", typeof(Game).Assembly), "main")
				.AsRootConstant(0, 2)
				.AsRootConstant(1, 2)
				.Compile().Result;

			innerSortProgram = new ShaderProgram()
				.UseIncludes(typeof(Game).Assembly)
				.SetComputeShader(Embed.GetString("Content/Shaders/BitonicSort/BitonicInnerSortCS.hlsl", typeof(Game).Assembly), "main")
				.AsRootConstant(0, 2)
				.AsRootConstant(1, 2)
				.Compile().Result;

			outerSortProgram = new ShaderProgram()
				.UseIncludes(typeof(Game).Assembly)
				.SetComputeShader(Embed.GetString("Content/Shaders/BitonicSort/BitonicOuterSortCS.hlsl", typeof(Game).Assembly), "main")
				.AsRootConstant(0, 2)
				.AsRootConstant(1, 2)
				.Compile().Result;

			preSortProgram = new ShaderProgram()
				.UseIncludes(typeof(Game).Assembly)
				.SetComputeShader(Embed.GetString("Content/Shaders/BitonicSort/BitonicPreSortCS.hlsl", typeof(Game).Assembly), "main")
				.AsRootConstant(0, 2)
				.AsRootConstant(1, 2)
				.Compile().Result;
		}
		
		/// <summary>
		/// The table that the sort algorithm operates on.
		/// Every element is a uint2, where component 0 (X) is the index and component 1 (Y) the sort key.
		/// </summary>
		public GraphicsBuffer TableBuffer { get; } = null;

		private GraphicsBuffer dispatchArgsBuffer = null;
		private int maxElements = 0;

		public BitonicSortContext(int maxElements)
		{
			this.maxElements = maxElements;
			TableBuffer = new GraphicsBuffer(8 * maxElements, 8);
			dispatchArgsBuffer = new GraphicsBuffer(22*23/2, 12);
		}

		public void Sort(CommandList list, GraphicsBuffer subject, GraphicsBuffer counter, int counterOffset = 0)
		{
			list.PushEvent("Bitonic sort");

			const bool sortAscending = true;

			int alignedMaxElements = (int)MathHelper.AlignPowerOfTwo(maxElements);
			int maxIterations = (int)Math.Log2(Math.Max(2048u, alignedMaxElements)) - 10;

			// Build indirect args.
			list.SetProgram(indirectProgram);
			list.SetProgramConstants(0, 0, maxIterations);
			list.SetProgramConstants(1, 0, counterOffset, unchecked((int)(sortAscending ? 0xffffffff : 0)));
			list.SetProgramSRV(0, 0, counter);
			list.SetProgramUAV(0, 0, dispatchArgsBuffer);
			list.DispatchGroups(1);

			// Pre-Sort the buffer up to k = 2048.
			// This also pads the list with invalid indices that will drift to the end of the sorted list.
			list.SetProgram(preSortProgram);
			list.BarrierUAV(TableBuffer);
			list.SetProgramUAV(0, 0, TableBuffer);
			list.DispatchIndirect(dispatchArgsBuffer);

			// We have already pre-sorted up through k = 2048 when first writing our list, so we continue sorting
			// with k = 4096.  For unnecessarily large values of k, these indirect dispatches will be skipped over with thread counts of 0.
			int indirectArgsOffset = 12;
			for (int k = 4096; k <= alignedMaxElements; k *= 2)
			{
				list.SetProgram(outerSortProgram);

				for (int j = k / 2; j >= 2048; j /= 2)
				{
					list.SetProgramConstants(0, 0, k, j);
					list.DispatchIndirect(dispatchArgsBuffer, indirectArgsOffset);
					list.BarrierUAV(TableBuffer);
					indirectArgsOffset += 12;
				}

				list.SetProgram(innerSortProgram);
				list.DispatchIndirect(TableBuffer, indirectArgsOffset);
				list.BarrierUAV(TableBuffer);
				indirectArgsOffset += 12;
			}

			list.PopEvent();
		}
	}
}
