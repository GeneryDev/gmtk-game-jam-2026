using GDF.Composition;
using GDF.Debug;
using GDF.Scenes;
using Godot;

namespace Game.Player;

[HasDebugCommands]
public partial class PlayerAttackController
{
    [DebugCommand("attack", DebugCommandType.TriggerWithArguments)]
    public static void Attack(DebugCommandArgumentParser args)
    {
        if (args.ReadWord(out var attackId))
        {
            var attackDescriptor = PlayerAttacks.FromId(attackId);
            if (!attackDescriptor.IsEmpty)
            {
                WeaponSystem.Instance.SetEquipped(attackDescriptor);
                GD.Print($"Attack changed to {attackDescriptor.Summary.RootNodeName}");
            }
            else
            {
                args.PrintCustomError($"No such attack ID '{attackId}'");
            }
        }
        else
        {
            args.PrintError();
        }
    }
}