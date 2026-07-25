using Godot;

namespace Game.Entities;

[GlobalClass]
public partial class MobRandomStrollGoal : MobGoal
{
    private Vector2 _targetPos;
    private float _targetReselectCooldown = 0;

    private RandomNumberGenerator _rng = new();
    
    public override void _PhysicsProcess(double delta)
    {
        _targetReselectCooldown -= (float)delta;
        if (_targetReselectCooldown < 0)
        {
            ReselectTarget();
        }
        base._PhysicsProcess(delta);
    }

    private void ReselectTarget()
    {
        _targetPos = MobSpawner.Instance.GetRandomPoint();
        _targetReselectCooldown = _rng.RandfRange(1, 4);
    }

    public override Vector2? GetTargetPosition()
    {
        return _targetPos;
    }
}