using GDF.Composition;
using GDF.Util;
using Godot;

namespace Game.Entities;

public partial class EntityDrag : Node
{
    [Export] public float Drag = 1;
    
    private ComponentCache<MotionComponent> _motionComponent;
    
    public override void _PhysicsProcess(double delta)
    {
        _motionComponent.Get(this).ScaleVelocity(1.0f - ExpDecay.GetDecayOverTime((float)(Drag * delta)));
        base._PhysicsProcess(delta);
    }
}