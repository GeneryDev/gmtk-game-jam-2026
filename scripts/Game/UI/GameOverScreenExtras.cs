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
            <= 4 => "A bit chilly, innit?",
            <= 6 => "Perfection.",
            <= 9 => "HOT HOT HOT!!!",
            <= 12 => "Do I smell smoke?",
            _ => "FIRE!!!"
        };
    }
}