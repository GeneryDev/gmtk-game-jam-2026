using System.Collections.Generic;
using GDF.Data;
using Godot;

namespace Game.Entities;

public partial class DamageableComponent : Node, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();

    [Signal]
    public delegate void HurtEventHandler();

    [Signal]
    public delegate void DiedEventHandler();
    
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
        EmitSignalHurt();
        if (HitPoints <= 0)
        {
            Kill();
        }
        EmitSignalUpdated();
    }

    private void Kill()
    {
        EmitSignalDied();
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

    public bool GetContextCollection(string key, string input, List<IDataContext> output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "hit_points":
            {
                for (int i = 0; i < MaxHitPoints; i++)
                {
                    output.Add(new PlaceholderDataContext(i < HitPoints ? "filled" : "empty").Boxed());
                }
                return true;
            }
        }

        return false;
    }
}