using Game.Timers;
using GDF.Data;
using Godot;

namespace Game.UI;

public partial class GameOverScreenExtras : Node, IDataContext
{
    public bool GetContextString(string key, string input, ref string replacement, IDataQueryOptions options)
    {
        switch (key)
        {
            case "end_quote":
            {
                replacement = GetEndQuote();
                return true;
            }
        }

        return false;
    }

    private string GetEndQuote()
    {
        if (!GameTimer.InstanceExists) return "";
        double totalTimeSec = GameTimer.Instance.TotalElapsedRealTime;
        double totalTimeMins = totalTimeSec / 60;
        return totalTimeMins switch
        {
            <= 2 => "Did I forget the zero?",
            <= 4 => "Is this thing broken?",
            <= 6 => "Huh? I swear that was not 10 minutes...",
            <= 9 => "What? I wasn't ready!",
            <= 11 => "Right on time!",
            <= 14 => "It's all sticking to the pan!",
            <= 17 => "Do I smell smoke?",
            _ => "FIRE!!!"
        };
    }
}