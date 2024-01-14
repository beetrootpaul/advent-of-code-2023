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

    public override void _Ready()
    {
        Input.ParseInputEvent(new InputEventAction { Action = "cycle_debug_menu", Pressed = true });

        _hud?.SetText("...");

        var rawInputData = FileAccess.Open(_myInputFile switch
        {
            InputFile.Example => "day21/example1_in.txt",
            InputFile.Puzzle => "day21/puzzle1_in.txt",
            _ => "NOT_SET"
        }, FileAccess.ModeFlags.Read).GetAsText();
        GD.Print(rawInputData);

        _hud?.SetText(rawInputData);
    }

    internal enum InputFile
    {
        Example,
        Puzzle
    }
}