using GDF.Data;
using GDF.Resources;
using GDF.Util;
using Godot;
using Godot.Collections;

namespace Game.Timers;

[Tool]
[GlobalClass]
public partial class TimerEffect : SummarizableScene, IDataContext, ITagged<string>
{
    [Signal]
    public delegate void TriggeredEventHandler();

    [ExportGroup("Texts")]
    [Export] [StoreInSummary] public string Description = "";
    [Export] [StoreInSummary] public string LogMessage = "";
    [Export] [StoreInSummary] public string ParticleMessage = "";

    [ExportGroup("Spawning")]
    [Export] [StoreInSummary] public float SpawnWeight = 1;
    [Export] [StoreInSummary] public TimerCondition SpawnCondition;

    [ExportGroup("Metadata")]
    [Export] [StoreInSummary] public Array<string> Tags = new();

    public TimerEffects.Descriptor Descriptor => TimerEffects.From(this);

    public void Trigger()
    {
        EmitSignalTriggered();
        TimerEffectLog.Instance?.Log(LogMessage, this);
    }
    
    public bool HasTag(string tag)
    {
        return Tags?.Contains(tag) ?? false;
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "has_tag":
            {
                output = HasTag(input);
                return true;
            }
            case "first_tag_index":
            {
                output = this.FirstTagIndex(input.Split(','));
                return true;
            }
        }

        return false;
    }

    public bool GetContextString(string key, string input, ref string replacement, IDataQueryOptions options)
    {
        switch (key)
        {
            case "description":
            {
                replacement = Description;
                return true;
            }
            case "log_message":
            {
                replacement = LogMessage;
                return true;
            }
            case "particle_message":
            {
                replacement = ParticleMessage;
                return true;
            }
        }

        return false;
    }
}