using Godot;
using Godot.Collections;

namespace Game.Timers;

[Tool]
[GlobalClass]
public partial class TimerCondition : Resource
{
    [ExportGroup("Require Mob Count","RequireMobCount")]
    [Export(PropertyHint.GroupEnable)] public bool RequireMobCount;
    [Export] public Variant RequireMobCountMin;
    [Export] public Variant RequireMobCountMax;
    [ExportGroup("Require Elapsed Real Time","RequireElapsedRealTime")]
    [Export(PropertyHint.GroupEnable)] public bool RequireElapsedRealTime;
    [Export(PropertyHint.Enum,"Seconds:1,Minutes:60")]
    public int RequireElapsedRealTimeUnit = 1;
    [Export] public Variant RequireElapsedRealTimeMin;
    [Export] public Variant RequireElapsedRealTimeMax;

    public bool Evaluate()
    {
        if (RequireMobCount)
        {
            int mobCount = GameTimer.Instance.GetTree().GetNodeCountInGroup("mob");
            if (RequireMobCountMin.VariantType != Variant.Type.Nil && mobCount < RequireMobCountMin.AsInt32())
                return false;
            if (RequireMobCountMax.VariantType != Variant.Type.Nil && mobCount > RequireMobCountMax.AsInt32())
                return false;
        }
        if (RequireElapsedRealTime)
        {
            double elapsedTimeInUnit = GameTimer.Instance.TotalElapsedRealTime / RequireElapsedRealTimeUnit;
            if (RequireElapsedRealTimeMin.VariantType != Variant.Type.Nil && elapsedTimeInUnit < RequireElapsedRealTimeMin.AsInt32())
                return false;
            if (RequireElapsedRealTimeMax.VariantType != Variant.Type.Nil && elapsedTimeInUnit > RequireElapsedRealTimeMax.AsInt32())
                return false;
        }

        return true;
    }

    public override void _ValidateProperty(Dictionary property)
    {
        var propName = property["name"].AsStringName();
        var usage = property["usage"].As<PropertyUsageFlags>();

        if (propName == PropertyName.RequireMobCountMin || propName == PropertyName.RequireMobCountMax)
        {
            usage |= PropertyUsageFlags.Checkable;
            property["type"] = Variant.From(Variant.Type.Int);
        }
        if (propName == PropertyName.RequireElapsedRealTimeMin || propName == PropertyName.RequireElapsedRealTimeMax)
        {
            usage |= PropertyUsageFlags.Checkable;
            property["type"] = Variant.From(Variant.Type.Float);
        }

        property["usage"] = Variant.From(usage);
    }
}