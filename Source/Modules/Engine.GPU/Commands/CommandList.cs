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

		public void Build()
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
			// Execute command list.
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
