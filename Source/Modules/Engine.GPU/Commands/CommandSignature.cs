using System;
using System.Runtime.InteropServices;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public sealed class CommandSignature
	{
		private List<IndirectArgumentDescription> arguments = new();
		private int stride = 0;
		internal ID3D12CommandSignature Handle;

		private ShaderProgram program = null;

		public CommandSignature WithDispatchMeshArg()
		{
			arguments.Add(new IndirectArgumentDescription
			{
				Type = IndirectArgumentType.DispatchMesh,
			});

			unsafe
			{
				stride += sizeof(DispatchMeshArguments);
			}

			return this;
		}
		public CommandSignature WithConstantArg(int register, ShaderProgram program)
		{
			if (!program.cRegisterMapping.TryGetValue(register, out var rootParam))
			{
				Debug.LogWarning($"Program does not contain cbuffer at register b{register}");
				return this;
			}

			this.program = program;
			arguments.Add(new IndirectArgumentDescription
			{
				Type = IndirectArgumentType.Constant,
				Constant = new()
				{
					DestOffsetIn32BitValues = 0,
					Num32BitValuesToSet = 1,
					RootParameterIndex = rootParam
				}
			});

			stride += 4;
			return this;
		}

		public CommandSignature Compile()
		{		
			CommandSignatureDescription desc = new()
			{
				ByteStride = stride,
				IndirectArguments = arguments.ToArray(),
			};

			GPUContext.Device.CreateCommandSignature(desc, program?.RootSignature, out Handle);
			
			return this;
		}
	}
}
