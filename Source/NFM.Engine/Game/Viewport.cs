using System;
using Avalonia.Input;
using NFM.GPU;
using NFM.World;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace NFM.Rendering;

/// <summary>
/// Contains the game logic for a UI viewport
/// </summary>
public unsafe class Viewport : IDisposable
{
	public static List<Viewport> All { get; } = new();

	// Basic properties
	public Swapchain Swapchain { get; }
	public object HostControl { get; }

	// Work camera settings
	private CameraNode workCamera = null;
	public CameraNode Camera => workCamera;
	public Scene Scene => workCamera.Scene;

	/// <summary>
	/// Constructs a viewport from a given UI host
	/// </summary>
	public Viewport(Swapchain swapchain, object hostControl = null)
	{
		Swapchain = swapchain;
		HostControl = hostControl;

		// Create work camera.
		workCamera = new CameraNode(null);
		workCamera.Name = "Work Camera";

		Engine.OnTick += OnTick;

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
		if (Input.IsDown(MouseButton.Right) && Input.InputSource == HostControl)
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
		Engine.OnTick -= OnTick;
		All.Remove(this);
	}
}
