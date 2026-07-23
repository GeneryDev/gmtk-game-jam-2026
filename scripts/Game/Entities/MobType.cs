using Godot;
using Godot.Collections;

namespace Game.Entities;

[GlobalClass]
public partial class MobType : Resource
{
    [Export] public PackedScene TemplateScene;

    [Export] public Array<string> ApplicableEffectTags = new();
}