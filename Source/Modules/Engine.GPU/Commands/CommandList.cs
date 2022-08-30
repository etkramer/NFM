using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public class CommandList : IDisposable
	{
		private readonly List<Command> commands = new();
		private readonly CommandAllocator allocator;
		internal ID3D12GraphicsCommandList6 list;

		internal ShaderProgram CurrentProgram { get; set; } = null;

		private ID3D12Fence waitFence;
		private ulong waitValue = 0;

		struct Command
		{
			public Action<ID3D12GraphicsCommandList6> BuildAction;
			public CommandInput[] Inputs;

			public Command(Action<ID3D12GraphicsCommandList6> buildAction, CommandInput[] inputs)
			{
				BuildAction = buildAction;
				Inputs = inputs;
			}
		}

		public CommandList()
		{
			allocator = new CommandAllocator(CommandListType.Direct);
			list = GPUContext.Device.CreateCommandList<ID3D12GraphicsCommandList6>(CommandListType.Direct, allocator.commandAllocators[GPUContext.FrameIndex]);
			list.Close();

			waitFence = GPUContext.Device.CreateFence(0);

			Reset();
		}

		public void Dispose()
		{
			list.Dispose();
			allocator.Dispose();
		}

		public void AddCommand(Action<ID3D12GraphicsCommandList6> buildAction, CommandInput[] inputs)
		{
			lock (commands)
			{
				commands.Add(new Command(buildAction, inputs));
			}
		}

		private struct PendingTransition
		{
			public Resource Resource;
			public ResourceStates BeforeState;
			public ResourceStates AfterState;
		}

		public void DispatchGroups(int threadGroupCountX, int threadGroupCountY = 1, int threadGroupCountZ = 1)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.Dispatch(threadGroupCountX, threadGroupCountY, threadGroupCountZ);
			};

			AddCommand(buildDelegate, null);
		}

		public void DispatchThreads(int threadCountX, int groupSizeX, int threadCountY = 1, int groupSizeY = 1, int threadCountZ = 1, int groupSizeZ = 1)
		{
			int groupsX = MathHelper.IntCeiling(threadCountX / (float)groupSizeX);
			int groupsY = MathHelper.IntCeiling(threadCountY / (float)groupSizeY);
			int groupsZ = MathHelper.IntCeiling(threadCountZ / (float)groupSizeZ);
			DispatchGroups(groupsX, groupsY, groupsZ);
		}

		public void BarrierUAV(params GraphicsBuffer[] buffers)
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

			AddCommand(buildDelegate, null);
		}

		public void BarrierUAV(params Texture[] textures)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ResourceBarrier[] barriers = new ResourceBarrier[textures.Length];
				for (int i = 0; i < textures.Length; i++)
				{
					barriers[i] = new ResourceBarrier(new ResourceUnorderedAccessViewBarrier(textures[i].Resource));
				}

				list.ResourceBarrier(barriers);
			};

			AddCommand(buildDelegate, null);
		}

		public void ExecuteIndirect(CommandSignature signature, GraphicsBuffer commandBuffer, int maxCommandCount, int commandStart = 0)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ulong commandOffset = (ulong)commandStart * (ulong)signature.Stride;
				list.ExecuteIndirect(signature.Handle, maxCommandCount, commandBuffer.Resource, commandOffset, commandBuffer.HasCounter ? commandBuffer : null, (ulong)commandBuffer.CounterOffset);
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(commandBuffer, ResourceStates.IndirectArgument)
			};

			AddCommand(buildDelegate, inputs);
		}

		public void ExecuteIndirect(CommandSignature signature, GraphicsBuffer commandBuffer, GraphicsBuffer countBuffer, long countOffset, int maxCommandCount, int commandStart = 0)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ulong commandOffset = (ulong)commandStart * (ulong)signature.Stride;
				list.ExecuteIndirect(signature.Handle, maxCommandCount, commandBuffer.Resource, commandOffset, countBuffer, (ulong)countOffset);
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(commandBuffer, ResourceStates.IndirectArgument),
				new CommandInput(countBuffer, ResourceStates.IndirectArgument)
			};

			AddCommand(buildDelegate, inputs);
		}

		private static CommandSignature dispatchSignature = new CommandSignature()
			.AddDispatchArg()
			.Compile();

		public void DispatchIndirect(GraphicsBuffer commandBuffer, int commandOffset = 0)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.ExecuteIndirect(dispatchSignature.Handle, 1, commandBuffer, (ulong)commandOffset, null, 0);
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(commandBuffer, ResourceStates.IndirectArgument)
			};

			AddCommand(buildDelegate, inputs);
		}

		public void SetProgramConstants(Register binding, params int[] constants)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = CurrentProgram;
				if (!program.cRegisterMapping.TryGetValue(binding, out int parameterIndex))
				{
					return;
				}

				if (program.IsMeshPixel)
				{
					unsafe
					{
						fixed (int* constantsPtr = constants)
						{
							list.SetGraphicsRoot32BitConstants(parameterIndex, constants.Length, constantsPtr, 0);
						}
					}
				}
				if (program.IsCompute)
				{
					unsafe
					{
						fixed (int* constantsPtr = constants)
						{
							list.SetComputeRoot32BitConstants(parameterIndex, constants.Length, constantsPtr, 0);
						}
					}
				}
			};

			AddCommand(buildDelegate, null);
		}

		public void SetProgramCBV(int slot, int space, GraphicsBuffer target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = CurrentProgram;
				if (!program.cRegisterMapping.TryGetValue(new(slot, space), out int parameterIndex))
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

			AddCommand(buildDelegate, inputs);
		}

		public void SetProgramUAV(int slot, int space, Texture target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = CurrentProgram;
				if (!program.uRegisterMapping.TryGetValue(new(slot, space), out int parameterIndex))
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

			AddCommand(buildDelegate, inputs);
		}

		public void SetProgramUAV(int slot, int space, GraphicsBuffer target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = CurrentProgram;
				if (!program.uRegisterMapping.TryGetValue(new(slot, space), out int parameterIndex))
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

			AddCommand(buildDelegate, inputs);
		}

		public void SetProgramSRV(int slot, int space, Texture target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = CurrentProgram;
				if (!program.tRegisterMapping.TryGetValue(new(slot, space), out int parameterIndex))
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

			AddCommand(buildDelegate, inputs);
		}

		public void SetProgramSRV(int slot, int space, GraphicsBuffer target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = CurrentProgram;
				if (!program.tRegisterMapping.TryGetValue(new(slot, space), out int parameterIndex))
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

			AddCommand(buildDelegate, inputs);
		}

		public void SetProgram(ShaderProgram program)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.SetPipelineState(program.PSO);
				CurrentProgram = program;

				if (program.IsMeshPixel)
				{
					list.SetGraphicsRootSignature(program.RootSignature);
				}
				if (program.IsCompute)
				{
					list.SetComputeRootSignature(program.RootSignature);
				}
			};

			AddCommand(buildDelegate, null);
		}

		public void DrawMesh(int instanceCount)
		{
			Action<ID3D12GraphicsCommandList6> buildDelegate = (list) =>
			{
				list.DispatchMesh(instanceCount, 1, 1);
			};

			AddCommand(buildDelegate, null);
		}

		public void CustomCommand(Action<ID3D12GraphicsCommandList> buildDelegate, CommandInput[] inputs)
		{
			AddCommand(buildDelegate, inputs);
		}

		public void ResetCounter(GraphicsBuffer buffer)
		{
			if (buffer.HasCounter)
			{
				unsafe
				{
					int zeroInt = 0;
					UploadBuffer(buffer, &zeroInt, 4, buffer.CounterOffset);
				}
			}
		}

		public unsafe void UploadBuffer<T>(BufferAllocation<T> handle, T data) where T : unmanaged
		{
			UploadBuffer(handle.Buffer, data, handle.Start);
		}

		public unsafe void UploadBuffer<T>(BufferAllocation<T> handle, Span<T> data) where T : unmanaged
		{
			UploadBuffer(handle.Buffer, data, handle.Start);
		}

		public unsafe void UploadBuffer<T>(GraphicsBuffer buffer, T data, long start = 0) where T : unmanaged
		{
			UploadBuffer(buffer, &data, sizeof(T), start * sizeof(T));
		}

		public unsafe void UploadBuffer<T>(GraphicsBuffer buffer, Span<T> data, long start = 0) where T : unmanaged
		{
			fixed (T* dataPtr = data)
			{
				UploadBuffer(buffer, dataPtr, data.Length * sizeof(T), start * sizeof(T));
			}
		}

		public unsafe void UploadBuffer(GraphicsBuffer buffer, void* data, int dataSize, long offset = 0)
		{
			lock (UploadHelper.Lock)
			{
				int uploadRing = UploadHelper.Ring;
				int uploadOffset = UploadHelper.UploadOffset;
				long destOffset = offset;

				// Copy data to upload buffer.
				Unsafe.CopyBlockUnaligned((byte*)UploadHelper.MappedRings[uploadRing] + uploadOffset, data, (uint)dataSize);
				UploadHelper.UploadOffset += dataSize;

				// Copy from upload to dest buffer.
				CustomCommand((o) =>
				{
					o.CopyBufferRegion(buffer.Resource, (ulong)destOffset, UploadHelper.Rings[uploadRing], (ulong)uploadOffset, (ulong)dataSize);
				},
				new[] { new CommandInput(buffer, ResourceStates.CopyDest) });
			}
		}

		public void CompactBuffer<T>(GraphicsBuffer<T> buffer) where T : unmanaged
		{
			buffer.Compact(this);
		}

		public void CopyBuffer(GraphicsBuffer source, GraphicsBuffer dest, long startOffset = 0, long destOffset = 0, long numBytes = -1)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				if (numBytes == -1)
				{
					numBytes = Math.Min(source.Capacity * source.Stride, dest.Capacity * dest.Stride);
				}

				list.CopyBufferRegion(dest, (ulong)destOffset, source, (ulong)startOffset, (ulong)numBytes);
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(source, ResourceStates.CopySource),
				new CommandInput(dest, ResourceStates.CopyDest),
			};
			
			AddCommand(buildDelegate, inputs);
		}

		private static GraphicsBuffer intermediateCopyBuffer = new GraphicsBuffer(8 * 1024 * 1024, 1); // ~8MB
		public void CopyBuffer(GraphicsBuffer buffer, long startOffset, long destOffset, long numBytes)
		{
			CopyBuffer(buffer, intermediateCopyBuffer, startOffset, 0, numBytes);
			CopyBuffer(intermediateCopyBuffer, buffer, 0, destOffset, numBytes);
		}

		public void CopyTexture(Texture source, Texture dest)
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
			
			AddCommand(buildDelegate, inputs);
		}

		public void ResolveTexture(Texture source, Texture dest)
		{
			// If neither texture is multisampled, perform a regular copy instead.
			if (source.Samples <= 1 && dest.Samples <= 1)
			{
				CopyTexture(source, dest);
				return;
			}

			Debug.Log($"{source.Samples}, {dest.Samples}");

			Debug.Assert(source.Samples > 1 && dest.Samples <= 1, "Cannot resolve a non-multisampled texture or to a multisampled texture");

			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.ResolveSubresource(dest, 0, source, 0, dest.Format);
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(source, ResourceStates.ResolveSource),
				new CommandInput(dest, ResourceStates.ResolveDest),
			};
			
			AddCommand(buildDelegate, inputs);
		}

		public void CopyResource(Resource source, Resource dest)
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
			
			AddCommand(buildDelegate, inputs);
		}

		public void SetRenderTarget(Texture renderTarget, Texture depthStencil = null)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				if (renderTarget == null)
				{
					list.OMSetRenderTargets(0, new CpuDescriptorHandle[0], depthStencil.DSV.Handle);
					list.RSSetViewport(0, 0, depthStencil.Width, depthStencil.Height);
					list.RSSetScissorRect(depthStencil.Width, depthStencil.Height);
				}
				else
				{
					list.OMSetRenderTargets(renderTarget.RTV.Handle, depthStencil?.DSV.Handle);
					list.RSSetViewport(0, 0, renderTarget.Width, renderTarget.Height);
					list.RSSetScissorRect(renderTarget.Width, renderTarget.Height);
				}
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(renderTarget, ResourceStates.RenderTarget),
				new CommandInput(depthStencil, ResourceStates.DepthWrite),
			};

			AddCommand(buildDelegate, inputs);
		}

		public void SetRenderTargets(Texture depthStencil, params Texture[] renderTargets)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.OMSetRenderTargets(renderTargets.Length, renderTargets.Select(o => o.RTV.Handle.CPUHandle).ToArray(), depthStencil?.DSV.Handle);
				list.RSSetViewport(0, 0, renderTargets.First().Width, renderTargets.First().Height);
				list.RSSetScissorRect(renderTargets.First().Width, renderTargets.First().Height);
			};

			List<CommandInput> inputs = new();
			if (depthStencil != null)
			{
				inputs.Add(new CommandInput(depthStencil, ResourceStates.DepthWrite));
			}
			
			foreach (var target in renderTargets)
			{
				inputs.Add(new CommandInput(target, ResourceStates.RenderTarget));
			}

			AddCommand(buildDelegate, inputs.ToArray());
		}

		public void ClearRenderTarget(Texture target, Color color = default(Color))
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

			AddCommand(buildDelegate, inputs);
		}

		public void ClearDepth(Texture target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.ClearDepthStencilView(target.DSV.Handle, ClearFlags.Depth, target.ClearValue.DepthStencil.Depth, target.ClearValue.DepthStencil.Stencil);
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(target, ResourceStates.DepthWrite)
			};

			AddCommand(buildDelegate, inputs);
		}

		public void RequestState(Resource resource, ResourceStates state)
		{
			CommandInput[] inputs = new[]
			{
				new CommandInput(resource, state)
			};

			AddCommand(null, inputs);
		}

		public void PushEvent(string name)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.BeginEvent(name);
			};

			AddCommand(buildDelegate, null);
		}

		public void PopEvent()
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				list.EndEvent();
			};

			AddCommand(buildDelegate, null);
		}

		private void Build()
		{
			lock (commands)
			{
				List<PendingTransition> transitions = new();

				// Build.
				for (int i = 0; i < commands.Count; i++)
				{
					CommandInput[] currentInputs = commands[i].Inputs;
					transitions.Clear();

					// Look for additional transitions to batch with.
					for (int j = i; j < commands.Count; j++)
					{
						CommandInput[] inputs = commands[j].Inputs;

						if (inputs == null)
						{
							continue;
						}

						// NOTE: Batching is completely and utterly broken. Rather than try to fix it, I'll just disable it for now.
						if (j > i)
						{
							break;
						}

						// Look for resource transitons.
						foreach (CommandInput input in inputs)
						{
							if (input.Resource == null)
							{
								continue;
							}

							if (input.Resource.State != input.State)
							{
								// Already transitioning this resource.
								if (transitions.Any((o) => o.Resource == input.Resource))
								{
									continue;
								}
								// Are we looking for batching candicates?
								else if (j > i)
								{
									// Is this resource being used by the current command?
									if (currentInputs?.Any(o => o.Resource == input.Resource) ?? false)
									{
										// Skip it, because the previous check doesn't notice resources that don't need a transition because they're already in the right state.
										continue;
									}
								}

								// Input requires transition. Add it to the batch.
								transitions.Add(new PendingTransition()
								{
									Resource = input.Resource,
									BeforeState = input.Resource.State,
									AfterState = input.State,
								});

								input.Resource.State = input.State;
							}
						}
					}
				
					if (transitions.Count > 0)
					{
						list.ResourceBarrier(transitions.Select(o => new ResourceBarrier(new ResourceTransitionBarrier(o.Resource.GetBaseResource(), o.BeforeState, o.AfterState))).ToArray());
					}

					commands[i].BuildAction?.Invoke(list);
				}
			}

			// Close command list.
			list.Close();
		}

		/// <summary>
		/// Executes all commands.
		/// </summary>
		/// <param name="wait">Perform a GPU-side wait (ensure all commands are complete before continuing?)</param>
		public void Execute(bool wait = false)
		{
			// Build and execute command list.
			Build();
			GPUContext.GraphicsQueue.ExecuteCommandList(list);

			if (wait)
			{
				GPUContext.GraphicsQueue.Signal(waitFence, ++waitValue);
				GPUContext.GraphicsQueue.Wait(waitFence, waitValue);
			}
		}

		/// <summary>
		/// Resets command list and prepares it to be (re)built.
		/// </summary>
		public void Reset()
		{
			lock (commands)
			{
				// Reset virtual list.
				commands.Clear();

				CurrentProgram = null;

				// Reset D3D list.
				allocator.Reset();
				list.Reset(allocator.commandAllocators[GPUContext.FrameIndex]);

				// Setup common state.
				list.SetDescriptorHeaps(1, new[]
				{
					ShaderResourceView.Heap.handle,
				});
			}
		}

		public IntPtr GetPointer()
		{
			return list.NativePointer;
		}
	}
}
