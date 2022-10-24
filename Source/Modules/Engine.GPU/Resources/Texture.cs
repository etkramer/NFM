using System;
using Vortice.DXGI;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public unsafe class Texture : Resource, IDisposable
	{
		public bool IsRT => rtv != null;
		public bool IsDS => dsv != null;

		internal ResourceDescription Description { get; set; }

		private UnorderedAccessView[] uavs;
		private ShaderResourceView[] srvs;
		private RenderTargetView rtv;
		private DepthStencilView dsv;

		internal override ID3D12Resource D3DResource { get; private protected set; }

		public readonly Format Format = Format.R8G8B8A8_UNorm;
		public readonly Format DSFormat = default;
		public readonly Format SRFormat = default;

		public int Width { get; private set; }
		public int Height { get; private set; }
		public Vector2i Size => new(Width, Height);
		public byte MipmapCount { get; private set; }
		public byte Samples;

		internal ClearValue? ClearValue { get; private set; }

		public string Name
		{
			get => D3DResource.Name;
			set => D3DResource.Name = value;
		}

		/// <summary>
		/// Constructs a Texture object
		/// </summary>
		public Texture(int width, int height, byte mipmapCount = 1, Format format = Format.R8G8B8A8_UNorm, Color clearColor = default, Format dsFormat = default, Format srFormat = default, byte samples = 1)
		{
			Format = format;
			Width = width;
			Height = height;
			MipmapCount = (byte)Math.Min(mipmapCount, Math.Floor(Math.Log2(Math.Max(width, height))) + 1);
			Samples = samples;

			DSFormat = dsFormat;
			SRFormat = srFormat;

			ResourceFlags flags = ResourceFlags.None;

			// Check if this format supports use as an RT.
			if (Format.SupportsRTV())
			{
				flags |= ResourceFlags.AllowRenderTarget;
			}
			// Otherwise, format may be eligible to use as DS.
			else if (Format.SupportsDSV())
			{
				flags |= ResourceFlags.AllowDepthStencil;
			}

			// Check for UAV support.
			if (Samples <= 1 && Format.SupportsUAV())
			{
				flags |= ResourceFlags.AllowUnorderedAccess;
			}

			// Calculate clear value for depth/stencil.
			if ((flags & ResourceFlags.AllowDepthStencil) != 0)
			{
				ClearValue = new ClearValue(dsFormat == default ? format : dsFormat, new DepthStencilValue()
				{
					Depth = clearColor[0],
					Stencil = 0
				});
			}
			// Calculate clear value for RT.
			else if ((flags & ResourceFlags.AllowRenderTarget) != 0)
			{
				ClearValue = new ClearValue(format, new Vortice.Mathematics.Color(clearColor.R, clearColor.G, clearColor.B, clearColor.A));
			}

			// Calculate resource description
			Description = new ResourceDescription()
			{
				Dimension = ResourceDimension.Texture2D,
				Alignment = 0,
				Width = (ulong)Width,
				Height = Height,
				DepthOrArraySize = 1,
				MipLevels = MipmapCount,
				Format = Format,
				SampleDescription = new SampleDescription(Samples, 0),
				Flags = flags,
			};

			// Create D3D resource.
			Graphics.Device.CreateCommittedResource(HeapProperties.DefaultHeapProperties, HeapFlags.None, Description, ResourceStates.CopyDest, ClearValue, out var resource);

			D3DResource = resource;
			State = ResourceStates.CopyDest;

			uavs = new UnorderedAccessView[mipmapCount];
			srvs = new ShaderResourceView[mipmapCount + 1]; // Extra index for "all mips"

			D3DResource.Name = "Standard texture";
		}

		/// <summary>
		/// Constructs a Texture object from a DXGI backbuffer
		/// </summary>
		internal Texture(ID3D12Resource resource, int width, int height)
		{
			D3DResource = resource;
			Width = width;
			Height = height;
			Format = Graphics.RTFormat;
			MipmapCount = 1;
			State = ResourceStates.CopyDest;

			D3DResource.Name = "Resource texture";
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
			D3DResource.Release();
			IsAlive = false;
		}
	}
}
