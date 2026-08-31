using System.Runtime.InteropServices;
using NFM.GPU;
using NFM.Graphics;
using Windows.Win32.Graphics.DirectComposition;

namespace NFM.Hosting;

/// <summary>
/// A swapchain living in its own composition visual, positioned to track a viewport element in the page.
/// </summary>
sealed class ViewportHost : IDisposable
{
	public int Id { get; }
	public Swapchain Swapchain { get; private set; }

	private readonly IDCompositionVisual visual;
	private readonly Viewport viewport;

	private Vector2i origin;

	private readonly IDCompositionVisual parent;

	public ViewportHost(int id, IDCompositionDevice device, IDCompositionVisual parent, Vector2i size)
	{
		Id = id;
		this.parent = parent;

		device.CreateVisual(out visual);
		parent.AddVisual(visual, false, null);

		Swapchain = new Swapchain(size);
		visual.SetContent(Marshal.GetObjectForIUnknown(Swapchain.NativePointer));

		viewport = new Viewport(Swapchain, this);
	}

	public void SetRect(Vector2i position, Vector2i size)
	{
		if (Swapchain.Size != size)
		{
			Swapchain.Resize(size);
		}

		origin = position;
		visual.SetOffsetX(position.X);
		visual.SetOffsetY(position.Y);
	}

	/// <summary>
	/// Feeds a window-space pointer position to the viewport, or null when the pointer is elsewhere.
	/// </summary>
	public void SetCursor(Vector2i? position)
	{
		viewport.CursorPosition = position is Vector2i pos ? pos - origin : null;
	}

	public void OnPress(bool isLeft)
	{
		// Alt+Shift+LMB belongs to the camera pan gesture.
		bool isPanning = Input.IsDown(Keys.LMenu) && Input.IsDown(Keys.LShiftKey);

		if (isLeft && !isPanning)
		{
			Select(viewport.HoveredNode, Input.IsDown(Keys.LControlKey));
		}
	}

	private static void Select(ModelNode? node, bool additive)
	{
		if (!additive)
		{
			Selection.DeselectAll();
		}

		if (node is null)
		{
			return;
		}

		if (Selection.Selected.Contains(node))
		{
			Selection.Deselect(node);
		}
		else
		{
			Selection.Select(node);
		}
	}

	public void Dispose()
	{
		viewport.Dispose();
		Swapchain.Dispose();

		visual.SetContent(null);
		parent.RemoveVisual(visual);
	}
}
