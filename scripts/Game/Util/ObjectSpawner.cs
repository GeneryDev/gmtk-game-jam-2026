using GDF;
using GDF.Util;
using Godot;

namespace Game.Util;

[GlobalClass]
[Icon($"{GdfConstants.IconRoot}/spawner_2d.png")]
public partial class ObjectSpawner : Node
{
    [Export(PropertyHint.Enum,"As Child,As Sibling,In Level")] public int RelativeMode = 0;
    
    public void Spawn(StringName name, PackedScene scene, Vector2 position)
    {
        if (scene == null) return;
        var instance = scene.GdfInstantiate();
        instance.Name = name;
        
        if (instance is Node2D node2d)
            node2d.SetPosition(position);

        switch (RelativeMode)
        {
            case 0:
            {
                this.AddChild(instance);
                instance.Owner = this.Owner;
                break;
            }
            case 1:
            {
                this.AddSibling(instance);
                instance.Owner = this.Owner;
                break;
            }
            case 2:
            {
                MobSpawner.Instance.GetParent().AddChild(instance);
                instance.Owner = MobSpawner.Instance.Owner;
                break;
            }
        }
    }
}