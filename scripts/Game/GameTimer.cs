using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Timers;
using GDF.Data;
using GDF.Util;
using Godot;
using Microsoft.VisualBasic;

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
			if (_remainingTime <= 0)
			{
				_tickRate = 0;
			}
			EmitSignalUpdated();
		}
	}

	[Export]
	public float TickRate
	{
		get => _tickRate;
		set
		{
			value = Mathf.Clamp(value, 0.0625f, 64f);
			if (Math.Abs(value - _tickRate) < 0.0001f) return;
			_tickRate = value;
			EmitSignalUpdated();
		}
	}

	private Accumulator _tickTimer;

	private readonly StringBuilder _sb = new();
	private long _remainingTime = 0;
	private float _tickRate = 1;


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
		UpdateAverageTimeRate(delta);
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
			case "rate":
			{
				replacement = GetFormattedRate();
				return true;
			}
		}
		return false;
	}

	private double lastRemainingTime = 0;
	private double GetCurrentTimeRate(double delta)
	{
		double rate = (lastRemainingTime - RemainingTime)/delta;
		lastRemainingTime = RemainingTime;
		return rate;
	}
	private double[] rateSamples = {};
	[Export] int RateSampleAmount = 100; // last 100 frames of updates
	
	[Export] ProgressBar RateBar;
	public void UpdateAverageTimeRate(double delta)
	{
		rateSamples = rateSamples.Append(GetCurrentTimeRate(delta)).ToArray();
		if (rateSamples.Length > RateSampleAmount)
		{
			rateSamples = rateSamples[1..];   
		}
	}
	public double GetAverageRate()
	{
		if (rateSamples.Length >= 1)
		{
			return rateSamples.Average();
		}
		return 0;
	}
	public String GetFormattedRate()
	{   
		// Update the visual bar at the same time because it is the same information that just updated
		RateBar.Value = GetRateBarFill();
		return rateSamples.Average().ToString();
	}
	public float GetRateBarFill()
	{
		return (float)(rateSamples.Average())+50;
	}
   

}
