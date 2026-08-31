using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace NFM.Components;

enum DragOperation
{
    None = 0,
    Left,
    Right,
    Top,
    Bottom
}

sealed partial class DockPanel
{
    public const double MinSplit = 0.05d;

    DragOperation operation = DragOperation.None;

    double startingSize;
    (double x, double y) startingMousePos = (0, 0);
    (double width, double height) containerSize = (0, 0);

    async Task OnBeginDrag(PointerEventArgs args, DragOperation newOperation)
    {
        // Gather info needed for measurements
        this.containerSize = (
            await this.JS.InvokeAsync<double>(
                "HTMLElement.getClientWidth",
                this.rowContainerElement
            ),
            await this.JS.InvokeAsync<double>(
                "HTMLElement.getClientHeight",
                this.rowContainerElement
            )
        );
        this.startingMousePos = (args.ClientX, args.ClientY);
        this.startingSize = newOperation switch
        {
            DragOperation.Left
                => await this.JS.InvokeAsync<double>(
                    "HTMLElement.getClientWidth",
                    this.leftContainerElement
                ),
            DragOperation.Right
                => await this.JS.InvokeAsync<double>(
                    "HTMLElement.getClientWidth",
                    this.rightContainerElement
                ),
            DragOperation.Top
                => await this.JS.InvokeAsync<double>(
                    "HTMLElement.getClientHeight",
                    this.topContainerElement
                ),
            DragOperation.Bottom
                => await this.JS.InvokeAsync<double>(
                    "HTMLElement.getClientHeight",
                    this.bottomContainerElement
                ),
            _ => throw new NotImplementedException()
        };

        // Capture pointer
        await this.JS.InvokeVoidAsync(
            "HTMLElement.setPointerCapture",
            this.rowContainerElement,
            args.PointerId
        );
        await this.JS.InvokeVoidAsync(
            "HTMLElement.setStyle",
            this.rowContainerElement,
            "cursor",
            newOperation switch
            {
                DragOperation.Left => "ew-resize",
                DragOperation.Right => "ew-resize",
                DragOperation.Top => "ns-resize",
                DragOperation.Bottom => "ns-resize",
                _ => throw new NotImplementedException()
            }
        );

        this.operation = newOperation;
    }

    async Task OnEndDrag(PointerEventArgs args)
    {
        if (this.operation is DragOperation.None)
        {
            return;
        }

        this.operation = DragOperation.None;

        // Release pointer
        await Task.WhenAll(
            this.JS
                .InvokeVoidAsync(
                    "HTMLElement.releasePointerCapture",
                    this.rowContainerElement,
                    args.PointerId
                )
                .AsTask(),
            this.JS
                .InvokeVoidAsync("HTMLElement.setStyle", this.rowContainerElement, "cursor", "auto")
                .AsTask()
        );
    }

    async Task OnMouseMove(PointerEventArgs args)
    {
        if (this.operation is DragOperation.None)
        {
            return;
        }

        var isHorizontal = this.operation switch
        {
            DragOperation.Left => true,
            DragOperation.Right => true,
            _ => false
        };

        var isReversed = this.operation switch
        {
            DragOperation.Right => true,
            DragOperation.Bottom => true,
            _ => false
        };

        var maxSplit = this.operation switch
        {
            DragOperation.Left => 1 - (this.Layout.Right?.Split ?? 0),
            DragOperation.Right => 1 - (this.Layout.Left?.Split ?? 0),
            DragOperation.Top => 1 - (this.Layout.Bottom?.Split ?? 0),
            DragOperation.Bottom => 1 - (this.Layout.Top?.Split ?? 0),
            _ => throw new NotImplementedException()
        };

        // "Size"s are width for left/right drags, and height for top/bottom drags.
        var totalSize = isHorizontal ? this.containerSize.width : this.containerSize.height;
        var distanceMoved = isHorizontal
            ? (args.ClientX - this.startingMousePos.x)
            : (args.ClientY - this.startingMousePos.y);

        var newSize = isReversed
            ? this.startingSize - distanceMoved
            : this.startingSize + distanceMoved;
        var newSplit = Math.Min(newSize / totalSize, maxSplit);

        switch (this.operation)
        {
            case DragOperation.Left:
                this.Layout.Left!.Split = newSplit;
                break;
            case DragOperation.Right:
                this.Layout.Right!.Split = newSplit;
                break;
            case DragOperation.Top:
                this.Layout.Top!.Split = newSplit;
                break;
            case DragOperation.Bottom:
                this.Layout.Bottom!.Split = newSplit;
                break;
        }

        await this.UpdateStyles();
    }

    async Task UpdateStyles()
    {
        static double clampToRange(double value) => Math.Clamp(value, MinSplit, 1 - MinSplit);

        await Task.WhenAll(
            this.Left is not null
                ? this.JS
                    .InvokeVoidAsync(
                        "HTMLElement.setStyle",
                        this.leftContainerElement,
                        "width",
                        $"{clampToRange(this.Layout.Left?.Split ?? 0) * 100}%"
                    )
                    .AsTask()
                : Task.CompletedTask,
            this.Right is not null
                ? this.JS
                    .InvokeVoidAsync(
                        "HTMLElement.setStyle",
                        this.rightContainerElement,
                        "width",
                        $"{clampToRange(this.Layout.Right?.Split ?? 0) * 100}%"
                    )
                    .AsTask()
                : Task.CompletedTask,
            this.Top is not null
                ? this.JS
                    .InvokeVoidAsync(
                        "HTMLElement.setStyle",
                        this.topContainerElement,
                        "height",
                        $"{clampToRange(this.Layout.Top?.Split ?? 0) * 100}%"
                    )
                    .AsTask()
                : Task.CompletedTask,
            this.Bottom is not null
                ? this.JS
                    .InvokeVoidAsync(
                        "HTMLElement.setStyle",
                        this.bottomContainerElement,
                        "height",
                        $"{clampToRange(this.Layout.Bottom?.Split ?? 0) * 100}%"
                    )
                    .AsTask()
                : Task.CompletedTask
        );
    }
}
