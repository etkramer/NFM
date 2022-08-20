using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Engine.GPU;
using Engine.Rendering;
using Avalonia.Input;

namespace Engine.Frontend
{
	public partial class ViewportHost : Panel
	{
		private Vector2 lastMousePos = Vector2.NaN;

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			PointerPointProperties props = e.GetCurrentPoint(this).Properties;

			if (props.IsRightButtonPressed)
			{
				// Update mouse look.
				Vector2 mousePos = new((float)e.GetCurrentPoint(null).Position.X, (float)e.GetCurrentPoint(null).Position.Y);
				UpdateLook(mousePos);
			}
			else
			{
				// Uncapture cursor.
			}

			// Update last mouse pos.
			lastMousePos = new((int)e.GetCurrentPoint(null).Position.X, (int)e.GetCurrentPoint(null).Position.Y);
			base.OnPointerMoved(e);
		}

		private void UpdateLook(Vector2 mousePos)
		{
			const float sensitivity = 0.15f;

			Vector2 mouseDelta = lastMousePos == Vector2.NaN ? Vector2.Zero : lastMousePos - mousePos;
			Vector3 cameraRotation = viewport.WorkCamera.Rotation;

			cameraRotation.Y += mouseDelta.X * sensitivity;
			cameraRotation.X += mouseDelta.Y * sensitivity;
			cameraRotation.X = Math.Clamp(cameraRotation.X, -90, 90);

			viewport.WorkCamera.Rotation = cameraRotation;
		}
	}
}