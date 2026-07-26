using Godot;

namespace Game.Player;

public partial class WeaponFunctions : Node
{
    public void GiveAmmo(StringName attackId, int amount)
    {
        var attack = PlayerAttacks.FromId(attackId);
        WeaponSystem.Instance.SetAmmo(attack, WeaponSystem.Instance.GetAmmo(attack)+amount);
    }
}