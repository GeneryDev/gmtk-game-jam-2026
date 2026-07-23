using GDF.Resources;

namespace Game.Timers;

[LibraryAccessibleInEditor]
public partial class TimerEffects : SceneResourceLibrary<TimerEffect>
{
    public override LibraryConfig GetLibraryConfig()
    {
        return new()
        {
            Roots = new[] { new LibraryConfig.LibraryRoot("res://scenes/objects/timer_effects") },
            PreloadAll = true
        };
    }
}