using System;
using Godot;

namespace AoC2023.Day21;

internal partial class Day21MainCamera : Camera2D
{
    internal void AdjustZoomToContent(Rect2I contentRect)
    {
        var viewportRect = GetViewportRect();
        GD.Print("[MainCamera] viewport rect: ", viewportRect);
        GD.Print("[MainCamera] content rect: ", contentRect);

        var stretchZoom =
            viewportRect.Size / contentRect.Size;
        GD.Print("[MainCamera] stretch zoom: ", stretchZoom);

        var aspectPreservingZoom = new Vector2(
            Math.Min(stretchZoom.X, stretchZoom.Y),
            Math.Min(stretchZoom.X, stretchZoom.Y)
        );
        GD.Print("[MainCamera] aspect preserving zoom: ", aspectPreservingZoom);
        Zoom = aspectPreservingZoom;


        Transform =
            Transform.Translated(
                contentRect.Position -
                (viewportRect.Size / aspectPreservingZoom -
                 contentRect.Size) / 2
            );
    }
}