using Godot;

namespace Game.Entities;

public partial class MobRandomStroll : Node
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
        _targetPos = new Vector2(_rng.RandfRange(-1, 1), _rng.RandfRange(-1, 1)) * new Vector2(500, 300);
        _targetReselectCooldown = _rng.RandfRange(1, 4);
    }

    public Vector2 GetTargetPosition()
    {
        return _targetPos;
    }
}