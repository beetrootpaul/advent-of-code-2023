using System;
using Godot;

namespace AoC2023.Day21;

internal partial class Day21MainCamera : Camera2D
{
    [Export]
    private Day21Visualization? _visualization;

    public override void _Ready()
    {
        AdjustZoomToVisualization();
    }

    private void AdjustZoomToVisualization()
    {
        if (_visualization == null) return;

        var viewportRect = GetViewportRect();
        var visualizationRect = _visualization.GetRect();
        GD.Print("[MainCamera] viewport rect: ", viewportRect);
        GD.Print("[MainCamera] visualization rect: ", visualizationRect);

        var stretchZoom =
            viewportRect.Size / visualizationRect.Size;
        GD.Print("[MainCamera] stretch zoom: ", stretchZoom);

        var aspectPreservingZoom = new Vector2(
            Math.Min(stretchZoom.X, stretchZoom.Y),
            Math.Min(stretchZoom.X, stretchZoom.Y)
        );
        GD.Print("[MainCamera] aspect preserving zoom: ", aspectPreservingZoom);
        Zoom = aspectPreservingZoom;


        Transform =
            Transform.Translated(
                visualizationRect.Position -
                (viewportRect.Size / aspectPreservingZoom -
                 visualizationRect.Size) / 2
            );
    }
}