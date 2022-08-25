using System;
using System.Collections.Concurrent;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public class CommandList : IDisposable
	{
		private readonly List<Command> commands = new();
		private readonly CommandAllocator allocator;
		internal ID3D12GraphicsCommandList6 list;

		internal ShaderProgram CurrentProgram { get; set; } = null;

		struct Command
		{
			public Action<ID3D12GraphicsCommandList6> BuildAction;
			public Func<CommandInput[]> FetchInputsAction;

			public Command(Action<ID3D12GraphicsCommandList6> buildAction, Func<CommandInput[]> fetchInputsAction)
			{
				BuildAction = buildAction;
				FetchInputsAction = fetchInputsAction;
			}
		}

		public CommandList()
		{
			allocator = new CommandAllocator(CommandListType.Direct);
			list = GPUContext.Device.CreateCommandList<ID3D12GraphicsCommandList6>(CommandListType.Direct, allocator.commandAllocators[GPUContext.FrameIndex]);
			list.Close();

			Reset();
		}

		public void Dispose()
		{
			list.Dispose();
			allocator.Dispose();
		}

		public void AddCommand(Action<ID3D12GraphicsCommandList6> buildAction, Func<CommandInput[]> fetchInputsAction)
		{
			lock (commands)
			{
				commands.Add(new Command(buildAction, fetchInputsAction));
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

		public void DrawIndirect(CommandSignature signature, int maxCommandCount, GraphicsBuffer commandBuffer, GraphicsBuffer countBuffer, int commandStart = 0, int countStart = 0)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ulong commandOffset = (ulong)commandStart * (ulong)signature.Stride;
				ulong countOffset = (ulong)countStart * sizeof(int);
				list.ExecuteIndirect(signature.Handle, maxCommandCount, commandBuffer.Resource, commandOffset, countBuffer?.Resource, countOffset);
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(commandBuffer, ResourceStates.IndirectArgument),
				new CommandInput(countBuffer, ResourceStates.IndirectArgument)
			};

			AddCommand(buildDelegate, () => inputs);
		}

		public void SetProgramCBV<T>(Register binding, GraphicsBuffer<T> target) where T : unmanaged
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

			AddCommand(buildDelegate, () => inputs);
		}

		public void SetProgramUAV(int slot, Texture target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = CurrentProgram;
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

			AddCommand(buildDelegate, () => inputs);
		}

		public void SetProgramUAV(int slot, GraphicsBuffer target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = CurrentProgram;
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

			AddCommand(buildDelegate, () => inputs);
		}

		public void SetProgramSRV(int slot, Texture target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = CurrentProgram;
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

			AddCommand(buildDelegate, () => inputs);
		}

		public void SetProgramSRV(int slot, GraphicsBuffer target)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				ShaderProgram program = CurrentProgram;
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

			AddCommand(buildDelegate, () => inputs);
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
			AddCommand(buildDelegate, () => inputs);
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
			
			AddCommand(buildDelegate, () => inputs);
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
			
			AddCommand(buildDelegate, () => inputs);
		}

		public void SetRenderTarget(Texture color, Texture depth = null)
		{
			Action<ID3D12GraphicsCommandList> buildDelegate = (list) =>
			{
				if (color == null)
				{
					list.OMSetRenderTargets(0, new CpuDescriptorHandle[0], depth.DSV.Handle);
					list.RSSetViewport(0, 0, depth.Width, depth.Height);
					list.RSSetScissorRect(depth.Width, depth.Height);
				}
				else
				{
					list.OMSetRenderTargets(color.RTV.Handle, depth?.DSV.Handle);
					list.RSSetViewport(0, 0, color.Width, color.Height);
					list.RSSetScissorRect(color.Width, color.Height);
				}
			};

			CommandInput[] inputs = new[]
			{
				new CommandInput(color, ResourceStates.RenderTarget),
				new CommandInput(depth, ResourceStates.DepthWrite),
			};

			AddCommand(buildDelegate, () => inputs);
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

			AddCommand(buildDelegate, () => inputs);
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

			AddCommand(buildDelegate, () => inputs);
		}

		public void RequestState(Resource resource, ResourceStates state)
		{
			CommandInput[] inputs = new[]
			{
				new CommandInput(resource, state)
			};

			AddCommand(null, () => inputs);
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
					CommandInput[] currentInputs = commands[i].FetchInputsAction?.Invoke();
					transitions.Clear();

					// Look for additional transitions to batch with.
					for (int j = i; j < commands.Count; j++)
					{
						CommandInput[] inputs = commands[j].FetchInputsAction?.Invoke();

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

		public void Execute()
		{
			// Build and execute command list.
			Build();
			GPUContext.GraphicsQueue.ExecuteCommandList(list);
		}

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
