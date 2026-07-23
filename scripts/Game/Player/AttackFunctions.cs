using Game.Entities;
using GDF.Composition;
using Godot;

namespace Game.Player;

public partial class AttackFunctions : Node
{
    [Signal]
    public delegate void HurtSuccessEventHandler();
    
    public void Hurt(Node2D target)
    {
        if (target.GetComponent<DamageableComponent>() is { } damageable)
        {
            damageable.Damage();
            EmitSignalHurtSuccess();
        }
    }
}