using System;
using Avalonia.Input;
using Engine.Editor;
using Engine.Frontend;
using Engine.GPU;
using Engine.World;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.Rendering
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ViewConstants
	{
		public Matrix4 WorldToView;
		public Matrix4 ViewToWorld;

		public Matrix4 ViewToClip;
		public Matrix4 ClipToView;

		public Vector2 ViewportSize;
	}

	/// <summary>
	/// Contains the game logic for a UI viewport
	/// </summary>
	public unsafe class Viewport : IDisposable
	{
		public static List<Viewport> All { get; } = new();

		public int ID { get; private set; }

		// Basic properties
		public ViewportHost Host { get; }
		public Vector2i Size => Host.Swapchain.RT.Size;

		// Work camera settings
		private CameraNode workCamera = null;
		public CameraNode Camera => workCamera;
		public Scene Scene => workCamera.Scene;

		#region Rendering

		// Command list and constants
		public CommandList CommandList = new();
		public GraphicsBuffer<ViewConstants> ViewCB = new(1, GraphicsBuffer.ConstantAlignment);

		// Render targets
		public Texture ColorTarget;
		public Texture DepthBuffer;	

		#endregion

		/// <summary>
		/// Constructs a viewport from a given UI host
		/// </summary>
		public Viewport(ViewportHost host)
		{
			Host = host;

			ID = All.Count;
			CommandList.Name = "Viewport List";

			// Create work camera.
			workCamera = new CameraNode().Spawn<CameraNode>();
			workCamera.Name = "Work Camera";

			// Create RTs and RT-sized buffers.
			ColorTarget = new Texture(Size.X, Size.Y, 1, Format.R8G8B8A8_UNorm, samples: 1);
			DepthBuffer = new Texture(Size.X, Size.Y, 1, Format.R32_Typeless, dsFormat: Format.D32_Float, srFormat: Format.R32_Float, samples: 1);
			
			// Register callbacks
			Game.OnTick += OnTick;
			host.Swapchain.OnResize += Resize;

			StaticNotify.Subscribe(typeof(Scene), nameof(Scene.Main), () =>
			{
				//workCamera?.Dispose();
				//workCamera = new CameraActor();
			});

			All.Add(this);
		}

		private Vector3 flyVelocity = Vector3.Zero;

		public void OnTick(double deltaTime)
		{
			const float lookSens = 0.15f;
			const float dampingCoefficient = 5;
			const float acceleration = 10;
			const float sprintMult = 2.5f;

			// Process input only if right click is down.
			if (Input.IsDown(MouseButton.Right) && Input.InputSource == Host)
			{
				// Mouse look
				Vector3 cameraRotation = Camera.Rotation;
				cameraRotation.Y += Input.MouseDelta.X * lookSens;
				cameraRotation.X += Input.MouseDelta.Y * lookSens;
				cameraRotation.X = Math.Clamp(cameraRotation.X, -90, 90);
				Camera.Rotation = cameraRotation;

				// Movement
				Vector3 accelVector = Vector3.Zero;
				if (Input.IsDown(Key.W))
				{
					accelVector.Z -= 1;
				}
				if (Input.IsDown(Key.A))
				{
					accelVector.X -= 1;
				}
				if (Input.IsDown(Key.S))
				{
					accelVector.Z += 1;
				}
				if (Input.IsDown(Key.D))
				{
					accelVector.X += 1;
				}

				// Transform WASD accelerations by camera direction.
				accelVector = Vector3.TransformVector(accelVector, Matrix4.CreateRotation(Camera.Rotation));

				if (Input.IsDown(Key.Space))
				{
					accelVector.Y += 1;
				}
				if (Input.IsDown(Key.C))
				{
					accelVector.Y -= 1;
				}

				// Apply acceleration to velocity.
				flyVelocity += (accelVector * acceleration * (Input.IsDown(Key.LeftShift) ? sprintMult : 1)) * (float)deltaTime;
			}

			// Flycam physics.
			flyVelocity = Vector3.Lerp(flyVelocity, Vector3.Zero, dampingCoefficient * (float)deltaTime);
			Camera.Position += flyVelocity * (float)deltaTime;
		}

		public void UpdateView()
		{
			// Calculate view/projection matrices.
			var viewMatrix = Matrix4.CreateRotation(new Vector3(0, 180, 0)) * Matrix4.CreateTransform(Camera.Position, Camera.Rotation, Vector3.One).Inverse();
			var projectionMatrix = Matrix4.CreatePerspectiveReversed(Camera.FOV, Size.X / Size.Y, 0.01f);

			// Upload to constant buffer.
			CommandList.UploadBuffer(ViewCB, new ViewConstants()
			{
				WorldToView = viewMatrix,
				ViewToWorld = viewMatrix.Inverse(),
				ViewToClip = projectionMatrix,
				ClipToView = projectionMatrix.Inverse(),
				ViewportSize = Size,
			});
		}

		/// <summary>
		/// Resizes the viewport and it's RTs
		/// </summary>
		private void Resize(Vector2i size)
		{
			ColorTarget.Resize(size.X, size.Y);
			DepthBuffer.Resize(size.X, size.Y);
		}

		public void Dispose()
		{
			ColorTarget.Dispose();
			DepthBuffer.Dispose();

			Game.OnTick -= OnTick;
			All.Remove(this);
		}
	}
}
