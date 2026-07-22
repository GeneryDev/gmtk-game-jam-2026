using Godot;

namespace Game.Entities;

public partial class DamageableComponent : Node
{
    public void Damage()
    {
        Owner.QueueFree();
    }
}