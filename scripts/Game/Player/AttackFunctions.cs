using Game.Entities;
using GDF.Composition;
using Godot;

namespace Game.Player;

public partial class AttackFunctions : Node
{
    [Signal]
    public delegate void HurtSuccessEventHandler();
    [Signal]
    public delegate void KillSuccessEventHandler();
    
    public void Hurt(Node2D target)
    {
        if (target.GetComponent<DamageableComponent>() is { } damageable)
        {
            damageable.Damage();
            EmitSignalHurtSuccess();
        }
    }
    public void Hurt(Node2D target, int hp)
    {
        if (target.GetComponent<DamageableComponent>() is { } damageable)
        {
            damageable.Damage(hp);
            EmitSignalHurtSuccess();
        }
    }

    public void Kill(Node2D target)
    {
        if (target.GetComponent<DamageableComponent>() is { } damageable)
        {
            damageable.Kill();
            EmitSignalKillSuccess();
        }
    }
}