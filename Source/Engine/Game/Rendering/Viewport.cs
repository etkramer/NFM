using System;
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
		public CameraActor WorkCamera = new CameraActor();

		#region Rendering

		// Command list and constants
		public CommandList CommandList = new();
		public GraphicsBuffer<ViewConstants> ViewCB = new(1, GraphicsBuffer.ConstantAlignment);

		// Render targets
		public Texture ColorTarget;
		public Texture DepthBuffer;	

		// Material (G-) buffers
		public Texture MatBuffer0;
		public Texture MatBuffer1;	

		#endregion

		/// <summary>
		/// Constructs a viewport from a given UI host
		/// </summary>
		public Viewport(ViewportHost host)
		{
			Host = host;
			All.Add(this);

			// Create RTs and RT-sized buffers.
			ColorTarget = new Texture(Size.X, Size.Y, 1, Format.R8G8B8A8_UNorm);
			DepthBuffer = new Texture(Size.X, Size.Y, 1, Format.R32_Typeless, dsFormat: Format.D32_Float, srFormat: Format.R32_Float);
			MatBuffer0 = new Texture(Size.X, Size.Y, 1, Format.R8G8B8A8_UNorm);
			MatBuffer1 = new Texture(Size.X, Size.Y, 1, Format.R8G8B8A8_UNorm);
			
			// Register callbacks
			Game.OnTick += OnTick;
			host.Swapchain.OnResize += Resize;
		}

		private Vector3 flyVelocity = Vector3.Zero;

		public void OnTick(double deltaTime)
		{
			const float lookSens = 0.15f;
			const float dampingCoefficient = 5;
			const float acceleration = 5;
			const float sprintMult = 2.5f;

			// Process input only if right click is down.
			if (InputHelper.RightMouseButton == KeyState.Down && InputHelper.InputSource == Host)
			{
				// Mouse look
				Vector3 cameraRotation = WorkCamera.Rotation;
				cameraRotation.Z += InputHelper.MouseDelta.X * lookSens;
				cameraRotation.X -= InputHelper.MouseDelta.Y * lookSens;
				cameraRotation.X = Math.Clamp(cameraRotation.X, -90, 90);
				WorkCamera.Rotation = cameraRotation;

				// Movement
				Vector3 accelVector = Vector3.Zero;
				if (InputHelper.W == KeyState.Down)
				{
					accelVector.Y -= 1;
				}
				if (InputHelper.A == KeyState.Down)
				{
					accelVector.X += 1;
				}
				if (InputHelper.S == KeyState.Down)
				{
					accelVector.Y += 1;
				}
				if (InputHelper.D == KeyState.Down)
				{
					accelVector.X -= 1;
				}

				// Transform WASD accelerations by camera direction.
				accelVector = Vector3.TransformVector(accelVector, Matrix4.CreateRotation(WorkCamera.Rotation));

				if (InputHelper.Space == KeyState.Down)
				{
					accelVector.Z += 1;
				}
				if (InputHelper.C == KeyState.Down)
				{
					accelVector.Z -= 1;
				}

				// Apply acceleration to velocity.
				flyVelocity += (accelVector * acceleration * (InputHelper.Shift == KeyState.Down ? sprintMult : 1)) * (float)deltaTime;
			}

			// Flycam physics.
			flyVelocity = Vector3.Lerp(flyVelocity, Vector3.Zero, dampingCoefficient * (float)deltaTime);
			WorkCamera.Position += flyVelocity * (float)deltaTime;
		}

		public void UpdateView()
		{
			// Calculate view/projection matrices.
			Matrix4 view = Matrix4.CreateTransform(WorkCamera.Position, WorkCamera.Rotation, Vector3.One).Inverse();
			Matrix4 projection = Matrix4.CreatePerspectiveReversed(WorkCamera.CalcFOV(), Size.X / (float)Size.Y, 0.01f);

			// Orient the camera in the opposite direction (facing +Y).
			view = Matrix4.CreateRotation(new Vector3(0, 0, 180)) * view;

			// World is in Z-up space (Blender coords). Translate it to Y-up (render coords).
			view = view * Matrix4.CreateRotation(new Vector3(-90, 180, 0)).Inverse();

			ViewCB.SetData(0, new ViewConstants()
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
			MatBuffer0.Resize(size.X, size.Y);
			MatBuffer1.Resize(size.X, size.Y);
		}

		public void Dispose()
		{
			ColorTarget.Dispose();
			DepthBuffer.Dispose();
			MatBuffer0.Dispose();
			MatBuffer1.Dispose();

			Game.OnTick -= OnTick;
			All.Remove(this);
		}
	}
}
