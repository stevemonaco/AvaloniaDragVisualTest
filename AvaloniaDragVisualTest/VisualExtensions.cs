using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Diagnostics;

namespace AvaloniaDragVisualTest;
internal static class VisualExtensions
{
    /// <summary>
    /// Render control to the destination stream.
    /// </summary>
    /// <param name="source">Control to be rendered.</param>
    /// <param name="destination">Destination stream.</param>
    /// <param name="dpi">Dpi quality.</param>
    public static RenderTargetBitmap? RenderTo(this Control source, double dpi = 96)
    {
        //var bounds = source.GetTransformedBounds()!.Value;
        //var transform = bounds.Transform;
        var rect = source.Bounds;

        Debug.WriteLine($"source {source.Bounds.Left}, {source.Bounds.Top}");
        Debug.WriteLine($"Rect {rect.Left}, {rect.Top}");

        //var rect = new Rect(source.Bounds.Size).TransformToAABB(transform);
        var top = rect.TopLeft;
        var pixelSize = new PixelSize((int)rect.Width, (int)rect.Height);
        var dpiVector = new Vector(dpi, dpi);

        //var root = source.GetVisualRoot() as Control ?? source;

        IDisposable? clipSetter = default;
        IDisposable? clipToBoundsSetter = default;
        IDisposable? renderTransformOriginSetter = default;
        IDisposable? renderTransformSetter = default;
        try
        {
            // Set clip region
            //var clipRegion = new RectangleGeometry(rect);
            //clipToBoundsSetter = source.SetValue(Visual.ClipToBoundsProperty, true, BindingPriority.Animation);
            //clipSetter = source.SetValue(Visual.ClipProperty, clipRegion, BindingPriority.Animation);

            // Translate origin
            renderTransformOriginSetter = source.SetValue(Visual.RenderTransformOriginProperty,
                new RelativePoint(top, RelativeUnit.Absolute),
                BindingPriority.Animation);

            renderTransformSetter = source.SetValue(Visual.RenderTransformProperty,
                new TranslateTransform(-top.X, -top.Y),
                BindingPriority.Animation);

            Debug.WriteLine($"Rect {top.X}, {top.Y}");

            var bitmap = new RenderTargetBitmap(pixelSize, dpiVector);
            bitmap.Render(source);
            return bitmap;
        }
        finally
        {
            // Restore values
            renderTransformSetter?.Dispose();
            renderTransformOriginSetter?.Dispose();
            clipSetter?.Dispose();
            clipToBoundsSetter?.Dispose();
            source?.InvalidateVisual();
        }
    }
}