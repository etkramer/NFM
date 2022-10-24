using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.GPU
{
	public class CommandList : IDisposable
	{
		private ID3D12CommandAllocator[] commandAllocators;
		internal ID3D12GraphicsCommandList6 list;

		internal PipelineState CurrentPSO { get; set; } = null;
		public bool IsOpen { get; private set; } = false;

		public string Name
		{
			get => list.Name;
			set => list.Name = value;
		}

		private static PipelineState MipGenPSO = new PipelineState()
			.UseIncludes(typeof(CommandList).Assembly)
			.SetComputeShader(Embed.GetString("Content/MipGenCS.hlsl", typeof(Graphics).Assembly), "MipGenCS")
			.AsRootConstant(0, 2)
			.Compile().Result;

		public CommandList()
		{
			// Create command allocators.
			commandAllocators = new ID3D12CommandAllocator[Graphics.RenderLatency];
			for (int i = 0; i < Graphics.RenderLatency; i++)
			{
				commandAllocators[i] = Graphics.Device.CreateCommandAllocator(CommandListType.Direct);
			}

			list = Graphics.Device.CreateCommandList<ID3D12GraphicsCommandList6>(CommandListType.Direct, commandAllocators[Graphics.FrameIndex]);
			list.Close();
		}

		public void Dispose()
		{
			list.Dispose();
			for (int i = 0; i < commandAllocators.Length; i++)
			{
				commandAllocators[i].Dispose();
			}
		}

		public void DispatchMesh(int threadGroupCountX, int threadGroupCountY = 1, int threadGroupCountZ = 1)
		{
			list.DispatchMesh(threadGroupCountX, threadGroupCountY, threadGroupCountZ);
		}

		public void DispatchMeshThreads(int threadCountX, int groupSizeX, int threadCountY = 1, int groupSizeY = 1, int threadCountZ = 1, int groupSizeZ = 1)
		{
			int groupsX = MathHelper.IntCeiling(threadCountX / (float)groupSizeX);
			int groupsY = MathHelper.IntCeiling(threadCountY / (float)groupSizeY);
			int groupsZ = MathHelper.IntCeiling(threadCountZ / (float)groupSizeZ);

			DispatchMesh(groupsX, groupsY, groupsZ);
		}

		public void Dispatch(int threadGroupCountX, int threadGroupCountY = 1, int threadGroupCountZ = 1)
		{
			list.Dispatch(threadGroupCountX, threadGroupCountY, threadGroupCountZ);
		}

		public void DispatchThreads(int threadCountX, int groupSizeX, int threadCountY = 1, int groupSizeY = 1, int threadCountZ = 1, int groupSizeZ = 1)
		{
			int groupsX = MathHelper.IntCeiling(threadCountX / (float)groupSizeX);
			int groupsY = MathHelper.IntCeiling(threadCountY / (float)groupSizeY);
			int groupsZ = MathHelper.IntCeiling(threadCountZ / (float)groupSizeZ);
			Dispatch(groupsX, groupsY, groupsZ);
		}

		public void BarrierUAV(params GraphicsBuffer[] buffers)
		{
			ResourceBarrier[] barriers = new ResourceBarrier[buffers.Length];
			for (int i = 0; i < buffers.Length; i++)
			{
				barriers[i] = new ResourceBarrier(new ResourceUnorderedAccessViewBarrier(buffers[i].D3DResource));
			}

			list.ResourceBarrier(barriers);
		}

		public void BarrierUAV(params Texture[] textures)
		{
			ResourceBarrier[] barriers = new ResourceBarrier[textures.Length];
			for (int i = 0; i < textures.Length; i++)
			{
				barriers[i] = new ResourceBarrier(new ResourceUnorderedAccessViewBarrier(textures[i].D3DResource));
			}

			list.ResourceBarrier(barriers);
		}

		public void ExecuteIndirect(CommandSignature signature, GraphicsBuffer commandBuffer, int maxCommandCount, int commandStart = 0)
		{
			RequestState(commandBuffer, ResourceStates.IndirectArgument);

			ulong commandOffset = (ulong)commandStart * (ulong)signature.Stride;
			list.ExecuteIndirect(signature.Handle, maxCommandCount, commandBuffer.D3DResource, commandOffset, commandBuffer.HasCounter ? commandBuffer : null, (ulong)commandBuffer.CounterOffset);
		}

		public void ExecuteIndirect(CommandSignature signature, GraphicsBuffer commandBuffer, GraphicsBuffer countBuffer, long countOffset, int maxCommandCount, int commandStart = 0)
		{
			RequestState(commandBuffer, ResourceStates.IndirectArgument);
			RequestState(countBuffer, ResourceStates.IndirectArgument);

			ulong commandOffset = (ulong)commandStart * (ulong)signature.Stride;
			list.ExecuteIndirect(signature.Handle, maxCommandCount, commandBuffer.D3DResource, commandOffset, countBuffer, (ulong)countOffset);
		}

		public unsafe void SetPipelineConstants(BindPoint point, int start, params int[] constants)
		{
			if (!CurrentPSO.cRegisterMapping.TryGetValue(point, out int parameterIndex))
			{
				return;
			}

			if (CurrentPSO.IsGraphics)
			{
				fixed (int* constantsPtr = constants)
				{
					list.SetGraphicsRoot32BitConstants(parameterIndex, constants.Length, constantsPtr, start);
				}
			}
			if (CurrentPSO.IsCompute)
			{
				fixed (int* constantsPtr = constants)
				{
					list.SetComputeRoot32BitConstants(parameterIndex, constants.Length, constantsPtr, start);
				}
			}
		}

		public void SetPipelineCBV(int slot, int space, GraphicsBuffer target)
		{
			RequestState(target, ResourceStates.VertexAndConstantBuffer);

			if (!CurrentPSO.cRegisterMapping.TryGetValue(new(slot, space), out int parameterIndex))
			{
				return;
			}

			if (CurrentPSO.IsGraphics)
			{
				list.SetGraphicsRootDescriptorTable(parameterIndex, target.GetCBV().Handle);
			}
			if (CurrentPSO.IsCompute)
			{
				list.SetComputeRootDescriptorTable(parameterIndex, target.GetCBV().Handle);
			}
		}

		public void SetPipelineUAV(int slot, int space, Texture target, int mipLevel = 0)
		{
			Debug.Assert(target.Samples <= 1, "Can't use a multisampled texture as a UAV");
			RequestState(target, ResourceStates.UnorderedAccess);

			if (!CurrentPSO.uRegisterMapping.TryGetValue(new(slot, space), out int parameterIndex))
			{
				return;
			}

			if (CurrentPSO.IsGraphics)
			{
				list.SetGraphicsRootDescriptorTable(parameterIndex, target.GetUAV(mipLevel).Handle);
			}
			if (CurrentPSO.IsCompute)
			{
				list.SetComputeRootDescriptorTable(parameterIndex, target.GetUAV(mipLevel).Handle);
			}
		}

		public void SetPipelineUAV(int slot, int space, GraphicsBuffer target)
		{
			RequestState(target, ResourceStates.UnorderedAccess);

			if (!CurrentPSO.uRegisterMapping.TryGetValue(new(slot, space), out int parameterIndex))
			{
				return;
			}

			if (CurrentPSO.IsGraphics)
			{
				list.SetGraphicsRootDescriptorTable(parameterIndex, target.GetUAV().Handle);
			}
			if (CurrentPSO.IsCompute)
			{
				list.SetComputeRootDescriptorTable(parameterIndex, target.GetUAV().Handle);
			}
		}

		public void SetPipelineSRV(int slot, int space, Texture target)
		{
			RequestState(target, ResourceStates.AllShaderResource);

			if (!CurrentPSO.tRegisterMapping.TryGetValue(new(slot, space), out int parameterIndex))
			{
				return;
			}

			if (CurrentPSO.IsGraphics)
			{
				list.SetGraphicsRootDescriptorTable(parameterIndex, target.GetSRV().Handle);
			}
			if (CurrentPSO.IsCompute)
			{
				list.SetComputeRootDescriptorTable(parameterIndex, target.GetSRV().Handle);
			}
		}

		public void SetPipelineSRV(int slot, int space, GraphicsBuffer target)
		{
			RequestState(target, ResourceStates.AllShaderResource);

			if (!CurrentPSO.tRegisterMapping.TryGetValue(new(slot, space), out int parameterIndex))
			{
				return;
			}

			if (CurrentPSO.IsGraphics)
			{
				list.SetGraphicsRootDescriptorTable(parameterIndex, target.GetSRV().Handle);
			}
			if (CurrentPSO.IsCompute)
			{
				list.SetComputeRootDescriptorTable(parameterIndex, target.GetSRV().Handle);
			}
		}

		public void SetPipelineState(PipelineState pso)
		{
			// Don't switch PSOs unnecessarily.
			if (CurrentPSO == pso)
			{
				return;
			}

			list.SetPipelineState(pso.PSO);
			CurrentPSO = pso;

			if (pso.IsGraphics)
			{
				list.SetGraphicsRootSignature(pso.RootSignature);
			}
			if (pso.IsCompute)
			{
				list.SetComputeRootSignature(pso.RootSignature);
			}
		}

		public void CustomCommand(Action<ID3D12GraphicsCommandList> buildDelegate)
		{
			buildDelegate.Invoke(list);
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

		public void UploadBuffer<T>(BufferAllocation<T> handle, T data) where T : unmanaged
		{
			UploadBuffer(handle.Buffer, data, handle.Start);
		}

		public unsafe void UploadBuffer<T>(GraphicsBuffer buffer, T data, long start = 0) where T : unmanaged
		{
			UploadBuffer(buffer, &data, sizeof(T), start * sizeof(T));
		}

		public void UploadBuffer<T>(BufferAllocation<T> handle, ReadOnlySpan<T> data) where T : unmanaged
		{
			UploadBuffer(handle.Buffer, data, handle.Start);
		}

		public unsafe void UploadBuffer<T>(GraphicsBuffer buffer, ReadOnlySpan<T> data, long start = 0) where T : unmanaged
		{
			fixed (T* dataPtr = data)
			{
				UploadBuffer(buffer, dataPtr, data.Length * sizeof(T), start * sizeof(T));
			}
		}

		public unsafe void UploadBuffer(GraphicsBuffer buffer, void* data, int dataSize, long offset = 0)
		{
			Debug.Assert(buffer.IsAlive);

			lock (UploadHelper.Lock)
			{
				int uploadRing = UploadHelper.Ring;
				int uploadOffset = UploadHelper.UploadOffset;
				long destOffset = offset;

				// Copy data to upload buffer.
				Unsafe.CopyBlock((byte*)UploadHelper.MappedRings[uploadRing] + uploadOffset, data, (uint)dataSize);
				UploadHelper.UploadOffset += dataSize;

				// Copy from upload to dest buffer.
				RequestState(buffer, ResourceStates.CopyDest);
				list.CopyBufferRegion(buffer.D3DResource, (ulong)destOffset, UploadHelper.Rings[uploadRing], (ulong)uploadOffset, (ulong)dataSize);
			}
		}

		public unsafe void UploadTexture(Texture texture, ReadOnlySpan<byte> data, int mipLevel = 0)
		{
			fixed (byte* dataPtr = data)
			{
				UploadTexture(texture, dataPtr, data.Length * sizeof(byte), mipLevel);
			}
		}

		public unsafe void UploadTexture(Texture texture, void* data, int dataSize, int mipLevel = 0)
		{
			Debug.Assert(texture.IsAlive);

			lock (UploadHelper.Lock)
			{
				int uploadRing = UploadHelper.Ring;
				int uploadOffset = UploadHelper.UploadOffset;

				// Realign upload offset for 512b alignment requirement.
				UploadHelper.UploadOffset = uploadOffset = (int)MathHelper.Align(UploadHelper.UploadOffset, D3D12.TextureDataPlacementAlignment);

				// Copy data to upload buffer.
				Unsafe.CopyBlockUnaligned((byte*)UploadHelper.MappedRings[uploadRing] + uploadOffset, data, (uint)dataSize);
				UploadHelper.UploadOffset += dataSize;

				// Calculate subresource info.
				var footprints = new PlacedSubresourceFootPrint[1];
				Graphics.Device.GetCopyableFootprints(texture.Description, mipLevel, 1, (ulong)uploadOffset, footprints, stackalloc int[1], stackalloc ulong[1], out _);

				TextureCopyLocation sourceLocation = new TextureCopyLocation(UploadHelper.Rings[uploadRing], footprints[0]);
				TextureCopyLocation destLocation = new TextureCopyLocation(texture, mipLevel);
				RequestState(texture, ResourceStates.CopyDest);
				list.CopyTextureRegion(destLocation, 0, 0, 0, sourceLocation);
			}
		}

		public void CopyBuffer(GraphicsBuffer source, GraphicsBuffer dest, long startOffset = 0, long destOffset = 0, long numBytes = -1)
		{
			RequestState(source, ResourceStates.CopySource);
			RequestState(dest, ResourceStates.CopyDest);

			if (numBytes == -1)
			{
				numBytes = Math.Min(source.Capacity * source.Stride, dest.Capacity * dest.Stride);
			}

			list.CopyBufferRegion(dest, (ulong)destOffset, source, (ulong)startOffset, (ulong)numBytes);
		}

		private static GraphicsBuffer intermediateCopyBuffer = new GraphicsBuffer(8 * 1024 * 1024, 1); // ~8MB
		public void CopyBuffer(GraphicsBuffer buffer, long startOffset, long destOffset, long numBytes)
		{
			CopyBuffer(buffer, intermediateCopyBuffer, startOffset, 0, numBytes);
			CopyBuffer(intermediateCopyBuffer, buffer, 0, destOffset, numBytes);
		}

		public void CopyTexture(Texture source, Texture dest)
		{
			RequestState(source, ResourceStates.CopySource);
			RequestState(dest, ResourceStates.CopyDest);

			list.CopyTextureRegion(new TextureCopyLocation(dest.D3DResource), 0, 0, 0, new TextureCopyLocation(source.D3DResource));
		}

		public void ResolveTexture(Texture source, Texture dest)
		{
			// If neither texture is multisampled, perform a regular copy instead.
			if (source.Samples <= 1 && dest.Samples <= 1)
			{
				CopyTexture(source, dest);
				return;
			}

			Debug.Assert(source.Samples > 1 && dest.Samples <= 1, "Cannot resolve a non-multisampled texture or to a multisampled texture");

			RequestState(source, ResourceStates.ResolveSource);
			RequestState(dest, ResourceStates.ResolveDest);

			list.ResolveSubresource(dest, 0, source, 0, dest.Format);
		}

		public void CopyResource(Resource source, Resource dest)
		{
			RequestState(source, ResourceStates.CopySource);
			RequestState(dest, ResourceStates.CopyDest);

			list.CopyResource(dest.D3DResource, source.D3DResource);
		}

		public void SetRenderTarget(Texture renderTarget, Texture depthStencil = null)
		{
			RequestState(renderTarget, ResourceStates.RenderTarget);
			RequestState(depthStencil, ResourceStates.DepthWrite);

			if (renderTarget == null)
			{
				list.OMSetRenderTargets(0, new CpuDescriptorHandle[0], depthStencil.GetDSV().Handle);
				list.RSSetViewport(0, 0, depthStencil.Width, depthStencil.Height);
				list.RSSetScissorRect(depthStencil.Width, depthStencil.Height);
			}
			else
			{
				list.OMSetRenderTargets(renderTarget.GetRTV().Handle, depthStencil?.GetDSV().Handle);
				list.RSSetViewport(0, 0, renderTarget.Width, renderTarget.Height);
				list.RSSetScissorRect(renderTarget.Width, renderTarget.Height);
			}
		}

		public void SetRenderTargets(Texture depthStencil, params Texture[] renderTargets)
		{
			RequestState(depthStencil, ResourceStates.DepthWrite);
			renderTargets.ForEach(o => RequestState(o, ResourceStates.RenderTarget));

			list.OMSetRenderTargets(renderTargets.Length, renderTargets.Select(o => o.GetRTV().Handle.CPUHandle).ToArray(), depthStencil?.GetDSV().Handle);
			list.RSSetViewport(0, 0, renderTargets.First().Width, renderTargets.First().Height);
			list.RSSetScissorRect(renderTargets.First().Width, renderTargets.First().Height);
		}

		public void ClearRenderTarget(Texture target, Color color = default(Color))
		{
			RequestState(target, ResourceStates.RenderTarget);

			ClearValue value;
			if (color == default(Color))
			{
				value = target.ClearValue.Value;
			}
			else
			{
				value = new ClearValue(target.Format, new Vortice.Mathematics.Color(color.R, color.G, color.B, color.A));
			}

			list.ClearRenderTargetView(target.GetRTV().Handle, value.Color);
		}

		public void ClearDepth(Texture target)
		{
			RequestState(target, ResourceStates.DepthWrite);

			list.ClearDepthStencilView(target.GetDSV().Handle, ClearFlags.Depth, target.ClearValue.Value.DepthStencil.Depth, target.ClearValue.Value.DepthStencil.Stencil);
		}

		public void GenerateMips(Texture texture)
		{
			if (texture.MipmapCount <= 1)
			{
				return;
			}
			else
			{
				SetPipelineState(MipGenPSO);

				for (int i = 1; i < texture.MipmapCount; i++)
				{
					uint dstWidth = (uint)Math.Max(texture.Width >> i, 1);
					uint dstHeight = (uint)Math.Max(texture.Height >> i, 1);

					unsafe
					{
						Vector2 texelSize = new(1.0f / dstWidth, 1.0f / dstHeight);
						SetPipelineConstants(0, 0, *(int*)&texelSize.X, *(int*)&texelSize.Y);
					}

					RequestState(texture, ResourceStates.AllShaderResource);

					list.SetComputeRootDescriptorTable(MipGenPSO.tRegisterMapping[new BindPoint(0, 0)], texture.GetSRV(i - 1).Handle);
					list.SetComputeRootDescriptorTable(MipGenPSO.uRegisterMapping[new BindPoint(0, 0)], texture.GetUAV(i).Handle);
					
					Dispatch((int)Math.Max(dstWidth / 8, 1), (int)Math.Max(dstHeight / 8, 1));
					BarrierUAV(texture);
				}
			}
		}

		public void RequestState(Resource resource, ResourceStates state)
		{
			if (resource == null)
			{
				return;
			}

			if ((resource.State & state) == 0)
			{
				list.ResourceBarrierTransition(resource, resource.State, state);
				resource.State = state;
			}
		}

		public void BeginEvent(string name)
		{
			list.BeginEvent(name);
		}

		public void EndEvent()
		{
			list.EndEvent();
		}

		public void Open()
		{
			commandAllocators[Graphics.FrameIndex].Reset();
			list.Reset(commandAllocators[Graphics.FrameIndex]);

			// Setup common state.
			list.SetDescriptorHeaps(1, new[]
			{
				ShaderResourceView.Heap.handle,
			});
		}

		public void Close()
		{
			list.Close();

			// Reset state.
			CurrentPSO = null;
		}

		public void Execute()
		{
			// Execute D3D command list.
			Graphics.GraphicsQueue.ExecuteCommandList(list);
		}
	}
}
