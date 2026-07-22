using GDF.Composition;
using Godot;

namespace Game.Entities;

public partial class MobController : Node
{
    [Export] public float MovementSpeed = 100;
    
    public Vector2 MoveVector = new Vector2();

    private ComponentCache<MobRandomStroll> _randomStroll;
    private ComponentCache<MotionComponent> _motionComponent;

    public override void _Process(double delta)
    {
        var targetPos = _randomStroll.Get(this).GetTargetPosition();
        var diff = (targetPos - _motionComponent.Get(this).Body.GlobalPosition);
        if (diff.LengthSquared() > 1)
        {
            MoveVector = diff.Normalized();
        }
        else
        {
            MoveVector = Vector2.Zero;
        }
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        var motion = _motionComponent.Get(this);

        var speed = MovementSpeed;
        motion.ApplyTranslation(MoveVector * speed * (float)delta);
        base._PhysicsProcess(delta);
    }
}