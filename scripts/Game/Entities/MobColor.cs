using Godot;

namespace Game.Entities;

public partial class MobColor : Node
{
    [Export] public Node2D Visual;

    [Export]
    public Color Color
    {
        get => _color;
        set
        {
            if (_color == value) return;
            _color = value;
            if (IsInsideTree()) ApplyColor();
        }
    }
    [Export] public bool Random = false;
    
    private Color _color = Colors.White;

    public override void _Ready()
    {
        base._Ready();
        if (Random) SetRandomColor();
        else ApplyColor(Color);
    }

    public void SetRandomColor()
    {
        var colorRandom = new RandomNumberGenerator()
        {
            // Seed = ???
        };
        var color = new Color(colorRandom.RandfRange(0.5f, 1), colorRandom.RandfRange(0.5f, 1),
            colorRandom.RandfRange(0.5f, 1));
        Color = color;
    }

    public void ApplyColor()
    {
        ApplyColor(Color);
    }

    public void ApplyColor(Color color)
    {
        Visual?.SetModulate(color);
    }
}