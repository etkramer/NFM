using System;
using Engine.Editor;
using Engine.Frontend;
using Engine.GPU;
using Engine.World;
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
	}

	/// <summary>
	/// Contains the game logic for a UI viewport
	/// </summary>
	public unsafe class Viewport : IDisposable
	{
		public static List<Viewport> All { get; } = new();

		// Basic properties
		public ViewportHost Host { get; }
		public Vector2i Size => Host.Swapchain.RT.Size;

		// Work camera settings
		private CameraActor workCamera = new();
		public CameraActor Camera => workCamera;
		public Scene Scene => Scene.Main;

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

			// Create RTs and RT-sized buffers.
			ColorTarget = new Texture(Size.X, Size.Y, 1, Format.R8G8B8A8_UNorm, samples: 1);
			DepthBuffer = new Texture(Size.X, Size.Y, 1, Format.R32_Typeless, dsFormat: Format.D32_Float, srFormat: Format.R32_Float, samples: 1);
			
			// Register callbacks
			Game.OnTick += OnTick;
			host.Swapchain.OnResize += Resize;

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
			if (Input.RightMouseButton == KeyState.Down && Input.InputSource == Host)
			{
				// Mouse look
				Vector3 cameraRotation = Camera.Rotation;
				cameraRotation.Y += Input.MouseDelta.X * lookSens;
				cameraRotation.X += Input.MouseDelta.Y * lookSens;
				cameraRotation.X = Math.Clamp(cameraRotation.X, -90, 90);
				Camera.Rotation = cameraRotation;

				// Movement
				Vector3 accelVector = Vector3.Zero;
				if (Input.W == KeyState.Down)
				{
					accelVector.Z -= 1;
				}
				if (Input.A == KeyState.Down)
				{
					accelVector.X -= 1;
				}
				if (Input.S == KeyState.Down)
				{
					accelVector.Z += 1;
				}
				if (Input.D == KeyState.Down)
				{
					accelVector.X += 1;
				}

				// Transform WASD accelerations by camera direction.
				accelVector = Vector3.TransformVector(accelVector, Matrix4.CreateRotation(Camera.Rotation));

				if (Input.Space == KeyState.Down)
				{
					accelVector.Y += 1;
				}
				if (Input.C == KeyState.Down)
				{
					accelVector.Y -= 1;
				}

				// Apply acceleration to velocity.
				flyVelocity += (accelVector * acceleration * (Input.Shift == KeyState.Down ? sprintMult : 1)) * (float)deltaTime;
			}

			// Flycam physics.
			flyVelocity = Vector3.Lerp(flyVelocity, Vector3.Zero, dampingCoefficient * (float)deltaTime);
			Camera.Position += flyVelocity * (float)deltaTime;
		}

		public void UpdateView()
		{
			// Calculate view/projection matrices.
			Matrix4 view = Matrix4.CreateTransform(Camera.Position, Camera.Rotation, Vector3.One).Inverse();
			Matrix4 projection = Matrix4.CreatePerspectiveReversed(Camera.FOV, Size.X / (float)Size.Y, 0.01f);

			// Orient the camera in the opposite direction (facing +Z).
			view = Matrix4.CreateRotation(new Vector3(0, 180, 0)) * view;

			Renderer.DefaultCommandList.UploadBuffer(ViewCB, new ViewConstants()
			{
				WorldToView = view,
				ViewToWorld = view.Inverse(),
				ViewToClip = projection,
				ClipToView = projection.Inverse()
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
