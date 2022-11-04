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
		private CameraNode workCamera = null;
		public CameraNode Camera => workCamera;
		public Scene Scene => workCamera.Scene;

		/// <summary>
		/// Constructs a viewport from a given UI host
		/// </summary>
		public Viewport(ViewportHost host)
		{
			Host = host;

			// Create work camera.
			workCamera = new CameraNode(null);
			workCamera.Name = "Work Camera";

			Game.OnTick += OnTick;

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
				cameraRotation.Z = (cameraRotation.Z + Input.MouseDelta.X * lookSens) % 360;
				cameraRotation.X = Math.Clamp((cameraRotation.X - Input.MouseDelta.Y * lookSens) % 360, -90, 90);

				Camera.Rotation = cameraRotation;

				// Movement
				Vector3 accelVector = Vector3.Zero;
				if (Input.IsDown(Key.W))
				{
					accelVector.Y -= 1;
				}
				if (Input.IsDown(Key.S))
				{
					accelVector.Y += 1;
				}
				if (Input.IsDown(Key.A))
				{
					accelVector.X += 1;
				}
				if (Input.IsDown(Key.D))
				{
					accelVector.X -= 1;
				}

				// Transform WASD accelerations by camera direction.
				accelVector = Vector3.TransformVector(accelVector, Matrix4.CreateRotation(Camera.Rotation));

				if (Input.IsDown(Key.Space))
				{
					accelVector.Z += 1;
				}
				if (Input.IsDown(Key.C))
				{
					accelVector.Z -= 1;
				}

				// Apply acceleration to velocity.
				flyVelocity += (accelVector * acceleration * (Input.IsDown(Key.LeftShift) ? sprintMult : 1)) * (float)deltaTime;
			}

			// Flycam physics.
			flyVelocity = Vector3.Lerp(flyVelocity, Vector3.Zero, dampingCoefficient * (float)deltaTime);
			Camera.Position += flyVelocity * (float)deltaTime;
		}

		public void Dispose()
		{
			Game.OnTick -= OnTick;
			All.Remove(this);
		}
	}
}
