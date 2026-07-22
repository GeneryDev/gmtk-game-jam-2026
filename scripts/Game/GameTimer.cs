using System;
using System.Text;
using GDF.Data;
using Godot;

namespace Game;

[GlobalClass]
public partial class GameTimer : Node, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();
    
    [Export(PropertyHint.Range, "0,600,1,suffix:s")] public float RemainingTime = 0;

    private float _lastDisplayedTime = 0;
    private string _lastDisplayedTimeFormatted = "";

    [Export]
    public float Precision = 1;

    private readonly StringBuilder _sb = new();
    
    private bool UpdateDisplayedTime()
    {
        float displayedTime = Mathf.RoundToInt(RemainingTime / Precision) * Precision;
        if (Mathf.Abs(displayedTime - _lastDisplayedTime) < Precision)
        {
            return false;
        }

        var time = TimeSpan.FromSeconds(displayedTime);

        if (time.TotalHours >= 1)
        {
            _sb.Append(time.Hours.ToString().PadZeros(2));
            _sb.Append(':');
        }
        _sb.Append(time.Minutes.ToString().PadZeros(2));
        _sb.Append(':');
        _sb.Append(time.Seconds.ToString().PadZeros(2));
        if (Precision < 1)
        {
            _sb.Append('.');
            _sb.Append(time.Milliseconds.ToString().PadZeros(3));
        }
        _lastDisplayedTimeFormatted = _sb.ToString();
        _sb.Clear();
        _lastDisplayedTime = displayedTime;
        return true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (RemainingTime > 0)
        {
            RemainingTime -= (float)delta;
            if (RemainingTime <= 0)
            {
                RemainingTime = 0;
            }
        }
        
        if (UpdateDisplayedTime())
        {
            EmitSignalUpdated();
        }
    }

    public StringName UpdatedSignalName => SignalName.Updated;

    public bool GetContextString(string key, string input, ref string replacement, IDataQueryOptions options)
    {
        switch (key)
        {
            case "time":
            {
                replacement = _lastDisplayedTimeFormatted;
                return true;
            }
        }

        return false;
    }
}