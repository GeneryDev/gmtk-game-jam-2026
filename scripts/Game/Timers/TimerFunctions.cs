using System;
using Godot;

namespace Game.Timers;

public partial class TimerFunctions : Node
{
    public void AddTime(int sec)
    {
        GameTimer.Instance.RemainingTime += sec;
    }
    public void SubtractTime(int sec)
    {
        GameTimer.Instance.RemainingTime -= sec;
    }
    
    public void MultiplyTime(float factor)
    {
        GameTimer.Instance.RemainingTime = Mathf.CeilToInt(GameTimer.Instance.RemainingTime * factor);
    }
    
    public void RoundTimeToNearest(float sec)
    {
        GameTimer.Instance.RemainingTime = Mathf.CeilToInt(Mathf.Round(GameTimer.Instance.RemainingTime / sec) * sec);
    }
    
    public void SwapMinutesAndSeconds()
    {
        var timeSpan = TimeSpan.FromSeconds(GameTimer.Instance.RemainingTime);
        int prevMin = Mathf.FloorToInt(timeSpan.TotalMinutes);
        int prevSec = timeSpan.Seconds;
        GameTimer.Instance.RemainingTime = (prevSec * 60) + prevMin;
    }
    
    public void MultiplyTickRate(float factor)
    {
        GameTimer.Instance.TickRate *= factor;
    }
}