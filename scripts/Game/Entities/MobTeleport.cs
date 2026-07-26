using Godot;

namespace Game.Entities;

public partial class MobTeleport : Node
{
    public void SwapPlacesWithPlayer()
    {
        var body = Owner as Node2D;
        if (body == null) return;
        var player = GetTree().GetFirstNodeInGroup("player_character") as Node2D;
        if (player == null) return;

        (body.GlobalPosition, player.GlobalPosition) = (player.GlobalPosition, body.GlobalPosition);
    }
}