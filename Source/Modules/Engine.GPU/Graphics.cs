using System;
using Vortice.Direct3D12;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Engine.GPU
{
	public static class Graphics
	{
		/// <summary>
		/// The total number of frames that have been rendered
		/// </summary>
		public static ulong FrameCount => GPUContext.FrameCount;

		/// <summary>
		/// The time spent rendering the last frame, in seconds
		/// </summary>
		public static double FrameTime { get; private set; }

		public static event Action OnFrameStart = delegate {};

		private static CommandList commandList;
		private static CommandList GetCommandList()
		{
			if (commandList == null)
			{
				commandList = new();
			}

			return commandList;
		}

		public static void Dispatch(int threadGroupCountX, int threadGroupCountY = 1, int threadGroupCountZ = 1)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.Dispatch(threadGroupCountX, threadGroupCountY, threadGroupCountZ);
			};

			GetCommandList().AddCommand(buildDelegate, null);
		}

		public static void UAVBarrier(GraphicsBuffer buffer)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ResourceBarrier barrier = new ResourceBarrier(new ResourceUnorderedAccessViewBarrier(buffer.Resource));
				list.ResourceBarrier(barrier);
			};

			GetCommandList().AddCommand(buildDelegate, null);
		}

		public static void BarrierUAV(params GraphicsBuffer[] buffers)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ResourceBarrier[] barriers = new ResourceBarrier[buffers.Length];
				for (int i = 0; i < buffers.Length; i++)
				{
					barriers[i] = new ResourceBarrier(new ResourceUnorderedAccessViewBarrier(buffers[i].Resource));
				}

				list.ResourceBarrier(barriers);
			};

			GetCommandList().AddCommand(buildDelegate, null);
		}

		public static void DrawIndirect(CommandSignature signature, int maxCommandCount, GraphicsBuffer commandBuffer, GraphicsBuffer countBuffer)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.ExecuteIndirect(signature.Handle, maxCommandCount, commandBuffer.Resource, 0, countBuffer?.Resource, 0);
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(commandBuffer, ResourceStates.IndirectArgument),
				new CommandInput(countBuffer, ResourceStates.IndirectArgument)
			};

			GetCommandList().AddCommand(buildDelegate, () => inputs);
		}

		public static void SetProgramCBV<T>(int slot, GraphicsBuffer<T> target) where T : unmanaged
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = GetCommandList().CurrentProgram;
				if (!program.cRegisterMapping.TryGetValue(slot, out int parameterIndex))
				{
					return;
				}

				if (program.IsMeshPixel)
				{
					list.SetGraphicsRootDescriptorTable(parameterIndex, target.CBV.Handle);
				}
				if (program.IsCompute)
				{
					list.SetComputeRootDescriptorTable(parameterIndex, target.CBV.Handle);
				}
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(target, ResourceStates.VertexAndConstantBuffer)
			};

			GetCommandList().AddCommand(buildDelegate, () => inputs);
		}

		public static void SetProgramUAV(int slot, GraphicsBuffer target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = GetCommandList().CurrentProgram;
				if (!program.uRegisterMapping.TryGetValue(slot, out int parameterIndex))
				{
					return;
				}

				if (program.IsMeshPixel)
				{
					list.SetGraphicsRootDescriptorTable(parameterIndex, target.UAV.Handle);
				}
				if (program.IsCompute)
				{
					list.SetComputeRootDescriptorTable(parameterIndex, target.UAV.Handle);
				}
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(target, ResourceStates.UnorderedAccess)
			};

			GetCommandList().AddCommand(buildDelegate, () => inputs);
		}

		public static void SetProgramSRV<T>(int slot, GraphicsBuffer<T> target) where T : unmanaged
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = GetCommandList().CurrentProgram;
				if (!program.tRegisterMapping.TryGetValue(slot, out int parameterIndex))
				{
					return;
				}

				if (program.IsMeshPixel)
				{
					list.SetGraphicsRootDescriptorTable(parameterIndex, target.SRV.Handle);
				}
				if (program.IsCompute)
				{
					list.SetComputeRootDescriptorTable(parameterIndex, target.SRV.Handle);
				}
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(target, ResourceStates.AllShaderResource)
			};

			GetCommandList().AddCommand(buildDelegate, () => inputs);
		}

		public static void SetProgram(ShaderProgram program)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.SetPipelineState(program.PSO);
				GetCommandList().CurrentProgram = program;

				if (program.IsMeshPixel)
				{
					list.SetGraphicsRootSignature(program.RootSignature);
				}
				if (program.IsCompute)
				{
					list.SetComputeRootSignature(program.RootSignature);
				}
			};

			GetCommandList().AddCommand(buildDelegate, null);
		}

		public static void DrawMesh(int instanceCount)
		{
			Action<ID3D12GraphicsCommandList6> buildDelegate = (list) =>
			{
				list.DispatchMesh(instanceCount, 1, 1);
			};

			GetCommandList().AddCommand(buildDelegate, null);
		}

		public static void CustomCommand(Action<ID3D12GraphicsCommandList> buildDelegate, CommandInput[] inputs)
		{
			GetCommandList().AddCommand(buildDelegate, () => inputs);
		}

		public static void CopyTexture(Texture source, Texture dest)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.CopyTextureRegion(new TextureCopyLocation(dest.Resource), 0, 0, 0, new TextureCopyLocation(source.Resource));
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(source, ResourceStates.CopySource),
				new CommandInput(dest, ResourceStates.CopyDest),
			};
			
			GetCommandList().AddCommand(buildDelegate, () => inputs);
		}

		public static void CopyResource(Resource source, Resource dest)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.CopyResource(dest.GetBaseResource(), source.GetBaseResource());
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(source, ResourceStates.CopySource),
				new CommandInput(dest, ResourceStates.CopyDest),
			};
			
			GetCommandList().AddCommand(buildDelegate, () => inputs);
		}

		public static void SetRenderTarget(Texture color, Texture depth = null)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.OMSetRenderTargets(color.RTV.Handle, depth?.DSV.Handle);
				list.RSSetViewport(0, 0, color.Width, color.Height);
				list.RSSetScissorRect((int)color.Width, (int)color.Height);
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(color, ResourceStates.RenderTarget),
				new CommandInput(depth, ResourceStates.DepthWrite),
			};

			GetCommandList().AddCommand(buildDelegate, () => inputs);
		}

		public static void ClearRenderTarget(Texture target, Color color = default(Color))
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ClearValue value;

				if (color == default(Color))
				{
					value = target.ClearValue;
				}
				else
				{
					value = new ClearValue(target.Format, new Vortice.Mathematics.Color(color.R, color.G, color.B, color.A));
				}

				list.ClearRenderTargetView(target.RTV.Handle, value.Color);
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(target, ResourceStates.RenderTarget)
			};

			GetCommandList().AddCommand(buildDelegate, () => inputs);
		}

		public static void ClearDepth(Texture target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.ClearDepthStencilView(target.DSV.Handle, ClearFlags.Depth, target.ClearValue.DepthStencil.Depth, target.ClearValue.DepthStencil.Stencil);
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(target, ResourceStates.DepthWrite)
			};

			GetCommandList().AddCommand(buildDelegate, () => inputs);
		}

		public static void RequestState(Resource resource, ResourceStates state)
		{
			CommandInput[] inputs = new[]
			{
				new CommandInput(resource, state)
			};

			GetCommandList().AddCommand(delegate{}, () => inputs);
		}

		private static Stopwatch frameTimer = new();

		public static void SubmitAndWait()
		{
			// Execute command list.
			commandList.Build();
			commandList.Execute();

			// Update frame timer.
			frameTimer.Stop();
			FrameTime = frameTimer.Elapsed.TotalSeconds;
			frameTimer.Restart();
			
			// Wait for completion.
			GPUContext.WaitFrame();

			// Reopen command list.
			commandList.Reset();

			// Raise new frame event.
			OnFrameStart();
		}

		public static void Flush()
		{
			GPUContext.WaitIdle();
		}
	}
}
