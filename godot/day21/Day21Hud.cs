using Godot;

namespace AoC2023.Day21;

internal partial class Day21Hud : CanvasLayer
{
    [Export] private Label? _label;

    internal void SetText(string text)
    {
        if (_label != null) _label.Text = text;
    }
}