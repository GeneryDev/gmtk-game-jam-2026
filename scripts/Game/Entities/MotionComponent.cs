using Godot;

namespace Game.Entities;

public partial class MotionComponent : Node
{
    [Export] public CharacterBody2D Body;
    
    [Export] public double TimeScale = 1.0f;

    public Vector2 Velocity;
    public Vector2 Translation;
    public Vector2 LastObservedVelocity { get; private set; }
    
    private double _prevDelta;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var totalVelocity = Vector2.Zero;

        totalVelocity += Velocity;
        totalVelocity *= (float)TimeScale;

        Vector2 velocityFromTranslation = Vector2.Zero;
        if (delta > 0)
        {
            velocityFromTranslation += Translation *= (float)(TimeScale / delta);
        }

        LastObservedVelocity = velocityFromTranslation + totalVelocity;
        
        // Move character (translation)
        Body.Velocity = velocityFromTranslation;
        Body.MoveAndSlide();
        
        // Move character (velocity)
        Body.Velocity = totalVelocity;
        Body.MoveAndSlide();
        
        // Use remainder velocity
        var remainderVelocity = Body.Velocity;
        Velocity = remainderVelocity;
        Translation = Vector2.Zero;

        _prevDelta = delta;
    }

    public void ApplyImpulse(Vector2 impulse)
    {
        Velocity += impulse;
    }

    public void ApplyForce(Vector2 force)
    {
        Velocity += force * (float)_prevDelta;
    }

    public void ApplyTranslation(Vector2 translation)
    {
        Translation += translation;
    }

    public void StopMoving()
    {
        Velocity = Translation = Vector2.Zero;
    }

    public void StopMoving(Vector2 axis)
    {
        Velocity = axis.Project(Velocity);
        Translation = axis.Project(Translation);
    }

    public void ScaleVelocity(float factor)
    {
        Velocity *= factor;
    }
}