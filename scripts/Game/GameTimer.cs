using System;
using System.Collections.Generic;
using System.Text;
using Game.Timers;
using GDF.Data;
using GDF.Util;
using Godot;

namespace Game;

[GlobalClass]
public partial class GameTimer : SingletonNode<GameTimer>, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();

    [Signal]
    public delegate void TickedEventHandler();

    [Export(PropertyHint.Range, "0,600,1,suffix:s")]
    public long RemainingTime
    {
        get => _remainingTime;
        set
        {
            if (_remainingTime == value) return;
            _remainingTime = value;
            EmitSignalUpdated();
        }
    }
    [Export]
    public float TickRate = 1;

    private Accumulator _tickTimer;

    private readonly StringBuilder _sb = new();
    private long _remainingTime = 0;
    

    public string GetFormattedTime()
    {
        _sb.Clear();
        if (_remainingTime < 0) _sb.Append('-');
        var time = TimeSpan.FromSeconds(_remainingTime).Duration();

        if (time.TotalHours >= 1)
        {
            _sb.Append(time.Hours.ToString().PadZeros(2));
            _sb.Append(':');
        }
        _sb.Append(time.Minutes.ToString().PadZeros(2));
        _sb.Append(':');
        _sb.Append(time.Seconds.ToString().PadZeros(2));
        var formatted = _sb.ToString();
        _sb.Clear();
        return formatted;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        _tickTimer.Add((float)(delta * TickRate));

        while (_tickTimer.Consume(1))
        {
            Tick();
        }
    }

    private void Tick()
    {
        EmitSignalTicked();
        TriggerEffects();
        // EmitSignalUpdated();
    }

    public StringName UpdatedSignalName => SignalName.Updated;

    public bool GetContextString(string key, string input, ref string replacement, IDataQueryOptions options)
    {
        switch (key)
        {
            case "time":
            {
                replacement = GetFormattedTime();
                return true;
            }
        }

        return false;
    }
}