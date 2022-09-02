using System;
using Vortice.DXGI;
using Vortice.Direct3D12;
using System.Runtime.InteropServices;

namespace Engine.GPU
{
	public unsafe class Texture : Resource, IDisposable
	{
		public bool IsRT => rtv != null;
		public bool IsDS => dsv != null;

		private UnorderedAccessView uav;
		public UnorderedAccessView UAV
		{
			get
			{
				if (uav == null)
				{
					uav = new UnorderedAccessView(this);
				}

				return uav;
			}
		}

		private ShaderResourceView srv;
		public ShaderResourceView SRV
		{
			get
			{
				if (srv == null)
				{
					srv = new ShaderResourceView(this);
				}

				return srv;
			}
		}

		private RenderTargetView rtv;
		public RenderTargetView RTV
		{
			get
			{
				if (rtv == null)
				{
					Debug.Assert(dsv == null, "Cannot use one texture for both depth/stencil and render target");
					Debug.Assert(!Format.IsDepthStencil(), "Cannot use a depth/stencil texture as a render target");
					rtv = new RenderTargetView(this);
				}

				return rtv;
			}
		}

		private DepthStencilView dsv;
		public DepthStencilView DSV
		{
			get
			{
				if (dsv == null)
				{
					Debug.Assert(rtv == null, "Cannot use one texture for both depth/stencil and render target");
					dsv = new DepthStencilView(this);
				}

				return dsv;
			}
		}

		internal ID3D12Resource Resource;

		public readonly Format Format = Format.R8G8B8A8_UNorm;
		public readonly Format DSFormat = default;
		public readonly Format SRFormat = default;

		public int Width { get; private set; }
		public int Height { get; private set; }
		public Vector2i Size => new(Width, Height);
		public byte Depth;
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
		public Texture(int width, int height, byte levels = 1, Format format = Format.R8G8B8A8_UNorm, Color clearColor = default, Format dsFormat = default, Format srFormat = default, byte samples = 1)
		{
			Format = format;
			Width = width;
			Height = height;
			Depth = levels;
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
			Depth = 1;
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
			srv = null;
			rtv = null;
			dsv = null;
			uav = null;

			// Create new resource with new size.
			GPUContext.Device.CreateCommittedResource(HeapProperties.DefaultHeapProperties, HeapFlags.None, GetDescription(), ResourceStates.CopyDest, ClearValue, out Resource);
			State = ResourceStates.CopyDest;
		}

		public void Dispose()
		{
			Resource.Release();
		}

		private ResourceDescription GetDescription()
		{
			return new()
			{
				Dimension = Depth == 1 ? ResourceDimension.Texture2D : ResourceDimension.Texture3D,
				Alignment = 0,
				Width = (ulong)Width,
				Height = Height,
				DepthOrArraySize = 1,
				MipLevels = Depth,
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
