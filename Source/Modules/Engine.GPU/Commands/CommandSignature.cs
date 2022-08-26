using System;
using System.Runtime.InteropServices;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public sealed class CommandSignature
	{
		public int Stride { get; private set; } = 0;

		private List<IndirectArgumentDescription> arguments = new();
		internal ID3D12CommandSignature Handle;

		private ShaderProgram program = null;

		public CommandSignature AddDispatchMeshArg()
		{
			arguments.Add(new IndirectArgumentDescription
			{
				Type = IndirectArgumentType.DispatchMesh,
			});

			unsafe
			{
				Stride += sizeof(DispatchMeshArguments);
			}

			return this;
		}
		public CommandSignature AddConstantArg(int register, ShaderProgram program)
		{
			if (!program.cRegisterMapping.TryGetValue(new(register, 0), out var rootParam))
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

			Stride += 4;
			return this;
		}

		public CommandSignature Compile()
		{		
			CommandSignatureDescription desc = new()
			{
				ByteStride = Stride,
				IndirectArguments = arguments.ToArray(),
			};

			GPUContext.Device.CreateCommandSignature(desc, program?.RootSignature, out Handle);
			
			return this;
		}
	}
}
