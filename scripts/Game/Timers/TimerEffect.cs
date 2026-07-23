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

    [Export] [StoreInSummary] public float SpawnWeight = 1;
    [Export] [StoreInSummary] public string Description = "";
    [Export] [StoreInSummary] public string LogMessage = "";
    [Export] [StoreInSummary] public string ParticleMessage = "";
    [Export] [StoreInSummary] public Array<string> Tags = new();
    [Export] [StoreInSummary] public string SpriteId = "";

    public TimerEffects.Descriptor Descriptor => TimerEffects.From(this);

    public void Trigger()
    {
        EmitSignalTriggered();
        TimerEffectLog.Instance?.Log(Descriptor.Reference.LogMessage);
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
            case "sprite_id":
            {
                output = SpriteId;
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