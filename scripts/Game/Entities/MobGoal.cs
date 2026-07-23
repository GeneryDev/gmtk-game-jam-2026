using Godot;

namespace Game.Entities;

[GlobalClass]
public abstract partial class MobGoal : Node
{
    public abstract Vector2? GetTargetPosition();
}