using GDF.Data;
using Godot;

namespace Game.Entities;

public partial class DamageableComponent : Node, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();
    
    [Export]
    public int MaxHitPoints = 1;

    public int HitPoints = 1;

    public override void _Ready()
    {
        HitPoints = MaxHitPoints;
        base._Ready();
    }

    public void Damage()
    {
        HitPoints--;
        if (HitPoints <= 0)
        {
            Kill();
        }
        EmitSignalUpdated();
    }

    private void Kill()
    {
        Owner.QueueFree();
    }

    public StringName UpdatedSignalName => SignalName.Updated;

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "health":
            {
                output = (float)HitPoints;
                return true;
            }
            case "max_health":
            {
                output = (float)MaxHitPoints;
                return true;
            }
        }

        return false;
    }
}