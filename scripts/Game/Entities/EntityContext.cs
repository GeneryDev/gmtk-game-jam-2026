using GDF;
using GDF.Composition;
using GDF.Data;
using Godot;

namespace Game.Entities;

[Icon($"{GdfConstants.IconRoot}/data_context.png")]
public partial class EntityContext : Node, IDataContext
{
    private ComponentCache<DamageableComponent> _damageable;
    private ComponentCache<EntityFlags> _flags;
    private ComponentCache<MobEffectHost> _mobEffectHost;
    
    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "entity":
            case "mob":
            case "body":
            {
                output = Owner;
                return true;
            }
            case "position":
            {
                if (Owner is Node2D body)
                {
                    output = body.GlobalPosition;
                    return true;
                }

                return false;
            }
        }

        return false;
    }

    public bool GetSubContext(string key, string input, ref IDataContext output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "health_context":
            case "damageable_context":
            {
                output = _damageable.Get(this);
                return true;
            }
            case "flags":
            case "flags_context":
            {
                output = _flags.Get(this);
                return true;
            }
            case "mob_effect_host":
            case "mob_effect_host_context":
            {
                output = _mobEffectHost.Get(this);
                return true;
            }
        }

        return false;
    }
}