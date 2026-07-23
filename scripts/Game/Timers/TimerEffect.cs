using GDF.Data;
using GDF.Resources;
using Godot;

namespace Game.Timers;

[Tool]
[GlobalClass]
public partial class TimerEffect : SummarizableScene, IDataContext
{
    [Signal]
    public delegate void TriggeredEventHandler();

    [Export] [StoreInSummary] public float SpawnWeight = 1;
    [Export] [StoreInSummary] public string Description = "";
    [Export] [StoreInSummary] public string LogMessage = "";
    [Export] [StoreInSummary] public string ParticleMessage = "";

    public TimerEffects.Descriptor Descriptor => TimerEffects.From(this);

    public void Trigger()
    {
        EmitSignalTriggered();
        TimerEffectLog.Instance?.Log(Descriptor.Reference.LogMessage);
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