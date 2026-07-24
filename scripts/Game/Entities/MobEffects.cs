using GDF.Resources;

namespace Game.Entities;

[LibraryAccessibleInEditor]
public partial class MobEffects : SceneResourceLibrary<MobEffect>
{
    public override LibraryConfig GetLibraryConfig()
    {
        return new()
        {
            Roots = new[] { new LibraryConfig.LibraryRoot("res://scenes/objects/mob_effects") },
            PreloadAll = true
        };
    }
}