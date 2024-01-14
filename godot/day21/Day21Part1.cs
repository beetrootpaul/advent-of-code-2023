using System.Collections.Generic;
using System.Linq;
using Godot;
using FileAccess = Godot.FileAccess;

namespace AoC2023.Day21;

//
// Day 21: Step Counter
// https://adventofcode.com/2023/day/21
//
// part 1
//
internal partial class Day21Part1 : Node
{
    [ExportCategory("_ params _")]
    [Export]
    private InputFile _myInputFile;

    [ExportCategory("_ instrumentation _")]
    [Export]
    private Day21Hud? _hud;

    [Export]
    private Day21Visualization? _visualization;

    [Export]
    private Day21MainCamera? _mainCamera;

    private Vector2I _size;
    private Vector2I _start;
    private bool[][] _rocks;
    private bool[][] _reached;

    public override void _Ready()
    {
        Input.ParseInputEvent(new InputEventAction
            { Action = "cycle_debug_menu", Pressed = true });

        _hud?.SetText("...");

        Parse(_myInputFile switch
        {
            InputFile.Example => "day21/example1_in.txt",
            InputFile.Puzzle => "day21/puzzle1_in.txt",
            _ => "NOT_SET"
        });
        GD.Print(_start);

        _visualization?.Visualize(_rocks, _start, _reached);

        if (_mainCamera != null && _visualization != null)
        {
            _mainCamera.AdjustZoomToContent(_visualization.GetRect());
        }

        FindPositionsAfterSteps(_myInputFile switch
        {
            InputFile.Example => 6,
            InputFile.Puzzle => 64,
            _ => 1
        });

        _visualization?.Visualize(_rocks, _start, _reached);

        var reachedAmount =
            _reached.Select(row => row.Count(isTileReached => isTileReached)).Sum();
        GD.Print("reachedAmount: ", reachedAmount);
        _hud.SetText($"RESULT\n{reachedAmount}");
    }

    private void Parse(string inputFile)
    {
        var rawInputData = FileAccess.Open(inputFile, FileAccess.ModeFlags.Read)
            .GetAsText();
        GD.Print(rawInputData);

        var inputLines = rawInputData
            .Split('\n')
            .Select(line => line.Trim())
            .Where(line => line.Length > 0)
            .ToList();
        _size = new Vector2I(inputLines[0].Length, inputLines.Count);

        _rocks = new bool[_size.Y][];
        _reached = new bool[_size.Y][];
        for (var y = 0; y < _size.Y; y++)
        {
            _rocks[y] = new bool[_size.X];
            _reached[y] = new bool[_size.X];
            for (var x = 0; x < _size.X; x++)
            {
                _rocks[y][x] = inputLines[y][x] == '#';
                _reached[y][x] = false;
                if (inputLines[y][x] == 'S')
                {
                    _start = new Vector2I(x, y);
                }
            }
        }
    }

    private void FindPositionsAfterSteps(int steps)
    {
        var directions = new List<Vector2I>
            { Vector2I.Right, Vector2I.Down, Vector2I.Left, Vector2I.Up };

        Stack<(Vector2I position, int stepsLeft)> s = new();
        s.Push((_start, steps));
        while (s.Count > 0)
        {
            var (position, stepsLeft) = s.Pop();

            if (stepsLeft <= 0)
            {
                _reached[position.Y][position.X] = true;
                continue;
            }

            foreach (var direction in directions)
            {
                var next = position + direction;
                if (next >= Vector2I.Zero && next < _size && !_rocks[next.Y][next.X])
                {
                    s.Push((next, stepsLeft - 1));
                }
            }
        }
    }

    internal enum InputFile
    {
        Example,
        Puzzle
    }
}