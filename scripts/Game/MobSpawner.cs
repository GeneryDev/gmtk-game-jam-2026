using GDF.Data;
using GDF.Util;
using Godot;

namespace Game;

public partial class MobSpawner : Node
{
    [Export] public NodeTemplate Template;
    [Export] public int StartAmount = 20;

    private RandomNumberGenerator _rng = new();
    private Accumulator _timer;

    public override void _PhysicsProcess(double delta)
    {
        _timer.Add((float)delta);
        while (_timer.Consume(2))
        {
            Spawn();
        }
        base._PhysicsProcess(delta);
    }

    public override void _Ready()
    {
        base._Ready();
        for (int i = 0; i < StartAmount; i++)
        {
            CallDeferred(MethodName.Spawn);
        }
    }
    
    public void Spawn()
    {
        if (Template == null) return;
        var task = Template.New();
        ((Node2D)task.Instance).Position = NextSpawnPos();
        task.Insert();
    }

    private Vector2 NextSpawnPos()
    {
        return new Vector2(_rng.RandfRange(-1, 1), _rng.RandfRange(-1, 1)) * new Vector2(500, 300);
    }
}