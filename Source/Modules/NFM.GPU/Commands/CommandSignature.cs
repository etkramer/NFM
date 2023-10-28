using System;
using System.Runtime.InteropServices;
using Vortice.Direct3D12;

namespace NFM.GPU;

public sealed class CommandSignature : IDisposable
{
	public int Stride { get; private set; } = 0;

	private List<IndirectArgumentDescription> arguments = new();
	internal ID3D12CommandSignature? Handle;

	private PipelineState? program = null;

	public CommandSignature AddDrawIndexedArg()
	{
		arguments.Add(new IndirectArgumentDescription
		{
			Type = IndirectArgumentType.DrawIndexed,
		});

		unsafe
		{
			Stride += sizeof(DrawIndexedArguments);
		}

		return this;
	}

	public CommandSignature AddDispatchArg()
	{
		arguments.Add(new IndirectArgumentDescription
		{
			Type = IndirectArgumentType.Dispatch,
		});

		unsafe
		{
			Stride += sizeof(DispatchArguments);
		}

		return this;
	}

	public CommandSignature AddConstantArg(int register, PipelineState program)
	{
		if (!program.cRegisterMapping.TryGetValue(new(register, 0), out var rootParam))
		{
			Log.Warn($"Program does not contain cbuffer at register b{register}");
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

		Guard.NotNull(D3DContext.Device).CreateCommandSignature(desc, program?.RootSignature, out Handle);
		
		return this;
	}

	public void Dispose()
	{
		Handle?.Dispose();
	}
}
