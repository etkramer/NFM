using System;
using Vortice.DXGI;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public unsafe class Texture : Resource, IDisposable
	{
		public bool IsRT => rtv != null;
		public bool IsDS => dsv != null;

		private UnorderedAccessView[] uavs;
		private ShaderResourceView[] srvs;
		private RenderTargetView rtv;
		private DepthStencilView dsv;

		internal ID3D12Resource Resource;

		public readonly Format Format = Format.R8G8B8A8_UNorm;
		public readonly Format DSFormat = default;
		public readonly Format SRFormat = default;

		public int Width { get; private set; }
		public int Height { get; private set; }
		public Vector2i Size => new(Width, Height);
		public byte MipmapCount { get; private set; }
		public byte Samples;

		internal ClearValue ClearValue { get; private set; }

		public string Name
		{
			get => Resource.Name;
			set => Resource.Name = value;
		}

		/// <summary>
		/// Constructs a Texture object
		/// </summary>
		public Texture(int width, int height, byte mipmapCount = 1, Format format = Format.R8G8B8A8_UNorm, Color clearColor = default, Format dsFormat = default, Format srFormat = default, byte samples = 1)
		{
			// Clamp mipmap count to avoid overly small sizes.
			int maxMipmaps = Math.Max(1, (int)Math.Floor(width / Math.Pow(2, mipmapCount)));

			// Mipmap generation requires a UAV-supported format.
			if (!format.SupportsUAV())
			{
				maxMipmaps = 1;
			}

			Format = format;
			Width = width;
			Height = height;
			MipmapCount = (byte)Math.Min(mipmapCount, maxMipmaps);
			Samples = samples;

			DSFormat = dsFormat;
			SRFormat = srFormat;

			// Calculate clear value for depth/stencil.
			if (format.IsDepthStencil() || format.IsTypeless())
			{
				ClearValue = new ClearValue(dsFormat == default ? format : dsFormat, new DepthStencilValue()
				{
					Depth = clearColor[0],
					Stencil = 0
				});
			}
			// Calculate clear value for RT.
			else
			{
				ClearValue = new ClearValue(format, new Vortice.Mathematics.Color(clearColor.R, clearColor.G, clearColor.B, clearColor.A));
			}

			// Create buffer.
			GPUContext.Device.CreateCommittedResource(HeapProperties.DefaultHeapProperties, HeapFlags.None, GetDescription(), ResourceStates.CopyDest, ClearValue, out Resource);
			State = ResourceStates.CopyDest;

			uavs = new UnorderedAccessView[mipmapCount];
			srvs = new ShaderResourceView[mipmapCount + 1]; // Extra index for "all mips"

			Resource.Name = "Standard texture";
		}

		/// <summary>
		/// Constructs a Texture object from a DXGI backbuffer
		/// </summary>
		internal Texture(ID3D12Resource resource, int width, int height)
		{
			Resource = resource;
			Width = width;
			Height = height;
			Format = GPUContext.RTFormat;
			MipmapCount = 1;
			State = ResourceStates.CopyDest;

			Resource.Name = "Resource texture";
		}

		/// <summary>
		/// Resizes and resets the texture.
		/// </summary>
		public void Resize(int width, int height)
		{
			Width = width;
			Height = height;

			// Release existing D3D resource.
			Resource.Release();
			rtv = null;
			dsv = null;
			for (int i = 0; i < MipmapCount; i++)
			{
				uavs[i] = null;
			}
			for (int i = 0; i < MipmapCount + 1; i++)
			{
				srvs[i] = null;
			}

			// Create new resource with new size.
			GPUContext.Device.CreateCommittedResource(HeapProperties.DefaultHeapProperties, HeapFlags.None, GetDescription(), ResourceStates.CopyDest, ClearValue, out Resource);
			State = ResourceStates.CopyDest;
		}

		public UnorderedAccessView GetUAV(int mipLevel = 0)
		{
			if (uavs[mipLevel] == null)
			{
				uavs[mipLevel] = new UnorderedAccessView(this, mipLevel);
			}

			return uavs[mipLevel];
		}

		public ShaderResourceView GetSRV(int mipLevel = -1)
		{
			if (srvs[mipLevel + 1] == null)
			{
				srvs[mipLevel + 1] = new ShaderResourceView(this, mipLevel);
			}

			return srvs[mipLevel + 1];
		}

		public RenderTargetView GetRTV()
		{
			if (rtv == null)
			{
				Debug.Assert(dsv == null, "Cannot use one texture for both depth/stencil and render target");
				Debug.Assert(!Format.IsDepthStencil(), $"{Format} is not a supported render target format");

				rtv = new RenderTargetView(this);
			}

			return rtv;
		}

		public DepthStencilView GetDSV()
		{
			if (dsv == null)
			{
				Debug.Assert(rtv == null, "Cannot use one texture for both depth/stencil and render target");
				Debug.Assert(Format.IsDepthStencil() || DSFormat.IsDepthStencil(), $"{Format} is not a supported depth/stencil format");

				dsv = new DepthStencilView(this);
			}

			return dsv;
		}

		public void Dispose()
		{
			Resource.Release();
		}

		private ResourceDescription GetDescription()
		{
			return new()
			{
				Dimension = ResourceDimension.Texture2D,
				Alignment = 0,
				Width = (ulong)Width,
				Height = Height,
				DepthOrArraySize = 1,
				MipLevels = MipmapCount,
				Format = Format,
				SampleDescription = new SampleDescription(Samples, 0),
				Flags = Format.IsDepthStencil() || Format.IsTypeless() ? ResourceFlags.AllowDepthStencil : ResourceFlags.AllowRenderTarget
					| (Samples == 1 ? ResourceFlags.AllowUnorderedAccess : ResourceFlags.None),
			};
		}

		internal override ID3D12Resource GetBaseResource()
		{
			return Resource;
		}
	}
}
