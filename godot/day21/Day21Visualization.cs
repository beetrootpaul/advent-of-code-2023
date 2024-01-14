using System;
using System.Collections.Generic;
using Godot;

namespace AoC2023.Day21;

internal partial class Day21Visualization : Node2D
{
    private static readonly Dictionary<string, Vector2I> Tiles = new()
    {
        { "grass", new Vector2I(1, 0) },
        { "rock", new Vector2I(2, 0) },
        { "final", new Vector2I(3, 0) },
        { "start", new Vector2I(4, 0) },
        { "start_final", new Vector2I(5, 0) },
    };

    [Export]
    private TileMap? _tileMap;

    private Vector2I _tileSize = new(16, 16);

    internal Rect2I GetRect()
    {
        var tilesRect = _tileMap?.GetUsedRect() ?? new Rect2I(0, 0, 1, 1);
        return new Rect2I(tilesRect.Position * _tileSize, tilesRect.Size * _tileSize);
    }

    internal void Visualize(bool[][] rocks, Vector2I start)
    {
        _tileMap?.ClearLayer(0);
        for (var y = 0; y < rocks.Length; y++)
        {
            for (var x = 0; x < rocks[y].Length; x++)
            {
                var tile = (x == start.X && y == start.Y)
                    ? Tiles["start"]
                    : rocks[y][x]
                        ? Tiles["rock"]
                        : Tiles["grass"];
                _tileMap?.SetCell(0, new Vector2I(x, y), 0, tile);
            }
        }
    }
}