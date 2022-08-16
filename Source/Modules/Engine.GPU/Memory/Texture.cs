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

		private ShaderResourceView srv;
		internal ShaderResourceView SRV
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
		internal RenderTargetView RTV
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
		internal DepthStencilView DSV
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
		public bool Is2D => Levels == 1;

		public ulong Width { get; private set; }
		public ulong Height { get; private set; }
		public Vector2i Size => new((int)Width, (int)Height);
		public ushort Levels;

		public Color ClearColor { get; }
		internal ClearValue ClearValue { get; private set; }

		public string Name
		{
			get => Resource.Name;
			set => Resource.Name = value;
		}

		/// <summary>
		/// Constructs a Texture object
		/// </summary>
		public Texture(ulong width, ulong height, ushort levels = 1, Format format = Format.R8G8B8A8_UNorm, Color clearColor = default(Color))
		{
			Format = format;
			Width = width;
			Height = height;
			Levels = levels;

			ClearColor = clearColor;

			// Calculate clear value for depth/stencil.
			if (format.IsDepthStencil())
			{
				ClearValue = new ClearValue(format, new DepthStencilValue()
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
			GPUContext.Device.CreateCommittedResource(HeapProperties.DefaultHeapProperties, HeapFlags.None, GetDescription(Width, Height, Levels, format.IsDepthStencil()), ResourceStates.CopyDest, ClearValue, out Resource);
			State = ResourceStates.CopyDest;

			Resource.Name = "Standard texture";
		}

		/// <summary>
		/// Constructs a Texture object from a DXGI backbuffer
		/// </summary>
		internal Texture(ID3D12Resource resource, uint width, uint height)
		{
			Resource = resource;
			Width = width;
			Height = height;
			Format = GPUContext.RTFormat;
			Levels = 1;
			State = ResourceStates.CopyDest;

			Resource.Name = "Resource texture";
		}

		/// <summary>
		/// Resizes and resets the texture.
		/// </summary>
		public void Resize(ulong width, ulong height)
		{
			Width = width;
			Height = height;

			// Release existing D3D resource.
			Resource.Release();
			srv = null;
			rtv = null;
			dsv = null;

			// Create new resource with new size.
			GPUContext.Device.CreateCommittedResource(HeapProperties.DefaultHeapProperties, HeapFlags.None, GetDescription(Width, Height, Levels, Format.IsDepthStencil()), ResourceStates.CopyDest, ClearValue, out Resource);
			State = ResourceStates.CopyDest;
		}

		public void Dispose()
		{
			Resource.Release();
		}

		private ResourceDescription GetDescription(ulong width, ulong height, ushort levels, bool depthOnly = false)
		{
			return new()
			{
				Dimension = Is2D ? ResourceDimension.Texture2D : ResourceDimension.Texture3D,
				Alignment = 0,
				Width = width,
				Height = (int)height,
				DepthOrArraySize = 1,
				MipLevels = levels,
				Format = Format,
				SampleDescription = new(1, 0),
				Flags = depthOnly ? ResourceFlags.AllowDepthStencil : ResourceFlags.AllowRenderTarget,
			};
		}

		internal override ID3D12Resource GetBaseResource()
		{
			return Resource;
		}
	}
}
