using Avalonia.Input;
using NFM.GPU;
using NFM.World;

namespace NFM.Graphics;

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
	private CameraNode workCamera;
	public CameraNode Camera => workCamera;
	public Scene Scene => workCamera.Scene;

	/// <summary>
	/// Constructs a viewport from a given UI host
	/// </summary>
	public Viewport(Swapchain swapchain, object hostControl)
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
		const float dampingCoefficient = 15;
		const float acceleration = 30;
		const float sprintMult = 2.5f;

		// WASD Camera (RMB)
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
		// Pan Camera (Alt+Shift+LMB)
		else if (Input.IsDown(MouseButton.Left) && Input.IsDown(Key.LeftAlt) && Input.IsDown(Key.LeftShift))
		{
			Vector3 panVector = new Vector3()
			{
				X = -Input.MouseDelta.X * 0.002f,
				Z = -Input.MouseDelta.Y * 0.002f
			};

			flyVelocity = Vector3.Zero;
			Camera.Position += Vector3.TransformVector(panVector, Matrix4.CreateRotation(Camera.Rotation));
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
