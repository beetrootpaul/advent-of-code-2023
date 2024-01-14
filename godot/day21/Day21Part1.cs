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

    private Vector2I _size;
    private Vector2I _start;
    private bool[][] _rocks;

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

        _hud?.SetText(_start.ToString());
    }

    private void Parse(string inputFile)
    {
        var rawInputData = FileAccess.Open(_myInputFile switch
            {
                InputFile.Example => "day21/example1_in.txt",
                InputFile.Puzzle => "day21/puzzle1_in.txt",
                _ => "NOT_SET"
            }, FileAccess.ModeFlags.Read)
            .GetAsText();
        GD.Print(rawInputData);

        var inputLines = rawInputData
            .Split('\n')
            .Select(line => line.Trim())
            .Where(line => line.Length > 0)
            .ToList();
        _size = new Vector2I(inputLines[0].Length, inputLines.Count);

        _rocks = new bool[_size.Y][];
        for (var y = 0; y < _size.Y; y++)
        {
            _rocks[y] = new bool[_size.X];
            for (var x = 0; x < _size.X; x++)
            {
                _rocks[y][x] = inputLines[y][x] == '#';
                if (inputLines[y][x] == 'S')
                {
                    _start = new Vector2I(x, y);
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