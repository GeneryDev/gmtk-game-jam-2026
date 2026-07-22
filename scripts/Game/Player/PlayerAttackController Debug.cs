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
                foreach (var node in SceneManager.Instance.GetTree().GetNodesInGroup("player_character"))
                {
                    if (node.GetComponent<PlayerAttackController>() is { } controller)
                    {
                        controller.AttackScene = attackDescriptor.Scene;
                        GD.Print($"Attack changed to {attackDescriptor.Summary.RootNodeName}");
                    }
                }
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