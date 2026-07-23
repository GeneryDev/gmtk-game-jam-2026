using GDF.Resources;

namespace Game.Player;

[LibraryAccessibleInEditor]
public partial class PlayerAttacks : SceneResourceLibrary<PlayerAttack>
{
    public override LibraryConfig GetLibraryConfig()
    {
        return new()
        {
            Roots = new[] { new LibraryConfig.LibraryRoot("res://scenes/objects/attacks") },
            PreloadAll = true
        };
    }
}