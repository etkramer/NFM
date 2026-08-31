using Avalonia.Input;
using NFM.GPU;
using NFM.World;
using NFM.Threading;

namespace NFM.Graphics;

/// <summary>
/// Contains the game logic for a UI viewport
/// </summary>
public class Viewport : IDisposable
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

		Dispatcher.OnTick += OnTick;

		All.Add(this);
	}

	/// <summary>
	/// Cursor position in viewport pixels, or null when the cursor is elsewhere.
	/// </summary>
	public Vector2i? CursorPosition { get; set; }

	/// <summary>
	/// The node under the cursor, or null for empty space.
	/// Trails the cursor by a frame or two, since it's resolved by reading back the visbuffer.
	/// </summary>
	public ModelNode? HoveredNode { get; private set; }

	private Vector3 flyVelocity = Vector3.Zero;

	public void OnTick(double deltaTime)
	{
		// Hand this frame's cursor to the renderer, and pick up the result of the last one.
		Camera.PickCoords = CursorPosition;
		HoveredNode = Camera.HoveredInstance < 0 ? null : Scene.RenderData.GetInstanceOwner(Camera.HoveredInstance);

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
		Dispatcher.OnTick -= OnTick;
		All.Remove(this);

		workCamera.Dispose();
	}
}
