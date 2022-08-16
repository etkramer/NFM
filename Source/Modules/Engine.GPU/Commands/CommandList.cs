using System;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	internal class CommandList : IDisposable
	{
		private List<Command> Commands = new();
		private CommandAllocator allocator;
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
			Commands.Add(new Command(buildAction, fetchInputsAction));
		}

		private struct PendingTransition
		{
			public Resource Resource;
			public ResourceStates BeforeState;
			public ResourceStates AfterState;
		}

		public void Build()
		{
			List<PendingTransition> transitions = new();

			// Build.
			for (int i = 0; i < Commands.Count; i++)
			{
				CommandInput[] currentInputs = Commands[i].FetchInputsAction?.Invoke();
				transitions.Clear();

				// Look for additional transitions to batch with.
				for (int j = i; j < Commands.Count; j++)
				{
					CommandInput[] inputs = Commands[j].FetchInputsAction?.Invoke();

					if (inputs == null)
					{
						continue;
					}

					// Look for resource transitons.
					foreach (CommandInput input in inputs)
					{
						if (input.Resource == null)
						{
							continue;
						}

						// NOTE: The "are we already transitioning this" test fails if we didn't need to insert a transition because we were already in the right state.
						// This could cause resources to be transitioned to the wrong state, when they were already in the right one.
						// Added solution:
						if ((currentInputs?.Any(o => o.Resource == input.Resource) ?? false) && j != i)
						{
							continue;
						}

						if (input.State != input.Resource.State)
						{
							// Already transitioning this resource.
							if (transitions.Any((o) => o.Resource == input.Resource))
							{
								continue;
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

				Commands[i].BuildAction.Invoke(list);
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
			// Reset virtual list.
			Commands.Clear();
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

		public IntPtr GetPointer()
		{
			return list.NativePointer;
		}
	}
}
