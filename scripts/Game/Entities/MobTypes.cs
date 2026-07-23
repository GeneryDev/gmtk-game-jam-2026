using System.Collections.Generic;
using Game.Timers;
using GDF.Resources;
using Godot;

namespace Game.Entities;

[LibraryAccessibleInEditor]
public partial class MobTypes : ResourceLibrary<MobType, MobType>
{
    public override LibraryConfig GetLibraryConfig()
    {
        return new()
        {
            Roots = new[] { new LibraryConfig.LibraryRoot("res://resources/mob_types") },
            PreloadAll = true,
            FallbackId = "trivial"
        };
    }

    private static readonly List<MobTypes.Descriptor> TempTypes = new();
    
    public static MobTypes.Descriptor SelectForEffect(TimerEffects.Descriptor effect, RandomNumberGenerator rng)
    {
        if (effect.IsEmpty) return null;
        
        TempTypes.Clear();
        CollectAll(TempTypes);
        
        // Filter mob types by the ones that actually support the effect being spawned.
        for (int i = 0; i < TempTypes.Count; i++)
        {
            var type = TempTypes[i].Resource;
            var applicable = false;
            foreach (var tag in type.ApplicableEffectTags)
            {
                if (effect.Reference.HasTag(tag))
                {
                    applicable = true;
                    break;
                }
            }

            if (!applicable)
            {
                TempTypes.RemoveAt(i);
                i--;
            }
        }

        if (TempTypes.Count == 0)
        {
            GD.PrintErr($"There is no mob type that supports effect '{effect.Id}'. Please fix.");
            return Fallback;
        }

        if (TempTypes.Count == 1) return TempTypes[0];

        int pickedIndex = rng.RandiRange(0, TempTypes.Count - 1);
        return TempTypes[pickedIndex];
    }
}