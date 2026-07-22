using GDF.Data;
using Godot;

namespace Game.Util;

public partial class RayCast2DExtension : RayCast2D, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();
    [Signal]
    public delegate void CollidedEventHandler(Node2D node);
    
    private Node _lastCollided;
    
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (!Enabled) return;
        var collider = this.GetCollider() as Node2D;
        if (collider != _lastCollided)
        {
            _lastCollided = collider;
            EmitSignalUpdated();
            EmitSignalCollided(collider);
        }
    }

    public StringName UpdatedSignalName => SignalName.Updated;
    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "collided":
            {
                output = _lastCollided != null;
                return true;
            }
            case "collision_pos":
            case "collision_position":
            {
                if (!IsInsideTree())
                {
                    output = default;
                    return true;
                }
                output = GetCollisionPoint();
                return true;
            }
            case "distance":
            {
                if (!IsInsideTree())
                {
                    output = 0;
                    return true;
                }
                output = this.ToLocal(GetCollisionPoint()).Length();
                return true;
            }
        }

        return false;
    }
}