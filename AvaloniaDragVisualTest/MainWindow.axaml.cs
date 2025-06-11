using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System.Diagnostics;

namespace AvaloniaDragVisualTest;
public partial class MainWindow : Window
{
    private RenderTargetBitmap? _dragBitmap;
    private Point? _controlDragDelta;

    public MainWindow()
    {
        InitializeComponent();
        AddHandler(PointerExitedEvent, OnPointerExited, RoutingStrategies.Bubble);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        _dragBitmap?.Dispose();
        _dragBitmap = null;

        // Tag here is being lazy for demo purposes -- use a better approach to mark controls that can be dragged
        // If you mark a parent control with "CanDrag", you will need to detect drags from its children by crawling the visual tree
        if (e.Source is Control { Tag: "CanDrag" } control)
        {
            var size = new PixelSize((int)control.Bounds.Width, (int)control.Bounds.Height);

            _dragBitmap = control.RenderTo();

            var screenDragOrigin = e.GetPosition(this);
            _controlDragDelta = e.GetPosition(control);

            Debug.WriteLine($"Size: {size.Width}, {size.Height}");
            Debug.WriteLine($"DragOrigin: {screenDragOrigin.X}, {screenDragOrigin.Y}");
            Debug.WriteLine($"DragDelta: {_controlDragDelta.Value.X}, {_controlDragDelta.Value.Y}");

            dragVisual.Width = size.Width;
            dragVisual.Height = size.Height;

            var x = screenDragOrigin.X - _controlDragDelta.Value.X;
            var y = screenDragOrigin.Y - _controlDragDelta.Value.Y;

            dragVisual.Margin = new Thickness(x, y, 0, 0);
            dragVisual.Source = _dragBitmap;
            dragVisual.Opacity = 0.5;

            e.Pointer.Capture(rootPanel);
            e.Handled = true;
        }
        else
        {
            dragVisual.Source = null;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (_dragBitmap is null || _controlDragDelta is null)
            return;

        var location = e.GetPosition(this);

        var x = location.X - _controlDragDelta.Value.X;
        var y = location.Y - _controlDragDelta.Value.Y;

        dragVisual.Margin = new Thickness(x, y, 0, 0);

        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        ResetDragVisual();
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    private void ResetDragVisual()
    {
        _dragBitmap?.Dispose();
        _dragBitmap = null;

        dragVisual.Source = null;
        dragVisual.Width = 0;
        dragVisual.Height = 0;
    }
}