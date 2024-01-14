using Godot;

namespace AoC2023.Day21;

internal partial class Day21Visualization : Node2D
{
    [Export]
    private TileMap? _tileMap;

    private Vector2I _tileSize = new(16, 16);

    internal Rect2I GetRect()
    {
        var tilesRect = _tileMap?.GetUsedRect() ?? new Rect2I(0, 0, 1, 1);
        return new Rect2I(tilesRect.Position * _tileSize, tilesRect.Size * _tileSize);
    }
}