using Godot;

namespace AoC2023.Day21;

public partial class Day21Part1 : Node2D
{
	public override void _Ready()
	{
		GD.Print(">> READY");
	}

	public override void _Process(double delta)
	{
		GD.Print($"process: {delta}");
	}
}
