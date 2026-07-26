using GDF.Composition;
using GDF.Input;
using Godot;
using Godot.Collections;

namespace Game.Player;

public partial class PlayerAttackSwitcher : Node
{
    [Export] public Dictionary<GdfInputAction, StringName> AttackOptions;

    private ComponentCache<GdfPlayerInput> _playerInput;

    public override void _Process(double delta)
    {
        base._Process(delta);
        var playerInput = _playerInput.Get(this);
        if (AttackOptions != null && playerInput != null)
        {
            foreach (var (action, attackId) in AttackOptions)
            {
                if (playerInput.ConsumeActionEvent(action))
                {
                    var attackDescriptor = PlayerAttacks.FromId(attackId);
                    WeaponSystem.Instance.SetEquipped(attackDescriptor);
                }
            }
        }
    }
}