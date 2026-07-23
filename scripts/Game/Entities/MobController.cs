using GDF.Composition;
using Godot;

namespace Game.Entities;

public partial class MobController : Node
{
    [Export] public float MovementSpeed = 100;
    
    public Vector2 MoveVector = new Vector2();

    private ComponentCache<MobGoal> _goal;
    private ComponentCache<MotionComponent> _motionComponent;
    private ComponentCache<EntityFlags> _entityFlags;

    public override void _Process(double delta)
    {
        var goal = _goal.Get(this);
        if (goal?.GetTargetPosition() is { } targetPos)
        {
            var diff = (targetPos - _motionComponent.Get(this).Body.GlobalPosition);
            if (diff.LengthSquared() > 1)
            {
                // Move toward goal
                MoveVector = diff.Normalized();
            }
            else
            {
                // Reached goal
                MoveVector = Vector2.Zero;
            }
        }
        else
        {
            // No goal
            MoveVector = Vector2.Zero;
        }

        // Set mirrored flag if moving left
        if (Mathf.Abs(MoveVector.X) > 0.1f && _entityFlags.Get(this) is { } entityFlags)
        {
            entityFlags.Mirrored = MoveVector.X < 0;
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