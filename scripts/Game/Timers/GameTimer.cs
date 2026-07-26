using System;
using System.Text;
using GDF.Audio;
using GDF.Data;
using GDF.Data.Static;
using GDF.Util;
using Godot;

namespace Game.Timers;

[GlobalClass]
[SingletonUsage(SingletonUsage.Scene)]
public partial class GameTimer : SingletonNode<GameTimer>, IDataContext
{
	[Signal]
	public delegate void UpdatedEventHandler();

	[Signal]
	public delegate void TickedEventHandler();

	[Signal]
	public delegate void ReachedZeroEventHandler();

	[Signal]
	public delegate void ContextSignalReceivedEventHandler(StringName signalName, Godot.Collections.Array args);

	[Export(PropertyHint.Range, "0,600,1,suffix:s")]

	public long RemainingTime
	{
		get => _remainingTime;
		set
		{
			if (_remainingTime == value) return;
			_remainingTime = value;
			_highestReachedTime = Mathf.Max(_highestReachedTime, _remainingTime);
			if (_remainingTime <= 0)
			{
				OnTimeReachedZero();
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
			value = Mathf.Clamp(value, 1/8f, 64f);
			if (Math.Abs(value - _tickRate) < 0.0001f) return;
			_tickRate = value;
			EmitSignalUpdated();
		}
	}

	public bool Active { get; private set; } = true;

	public double TotalElapsedRealTime { get; private set; } = 0;
	private Accumulator _tickTimer;

	private static readonly StringBuilder _sb = new();
	private long _remainingTime = 0;
	private float _tickRate = 1;
	private float _highestReachedTime = 0;

	public static string GetFormattedTime(double timeSec)
	{
		_sb.Clear();
		if (timeSec < 0) _sb.Append('-');
		var time = TimeSpan.FromSeconds(timeSec).Duration();

		if (time.TotalHours >= 1)
		{
			_sb.Append(((int)time.TotalHours).ToString().PadZeros(2));
			_sb.Append(':');
		}
		_sb.Append(time.Minutes.ToString().PadZeros(2));
		_sb.Append(':');
		_sb.Append(time.Seconds.ToString().PadZeros(2));
		var formatted = _sb.ToString();
		_sb.Clear();
		return formatted;
	}

	public string GetFormattedTime()
	{
		return GetFormattedTime(_remainingTime);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (Active)
		{
			TotalElapsedRealTime += delta;

			_tickTimer.Add((float)(delta * TickRate));

			while (_tickTimer.Consume(1))
			{
				Tick();
			}
		}
	}

	private void Tick()
	{
		EmitSignalTicked();
		EmitSignalContextSignalReceived(SignalName.Ticked, new());
		TriggerEffects();
	}

	private void OnTimeReachedZero()
	{
		_tickRate = 0;
		Active = false;
		EmitSignalReachedZero();
		EmitSignalContextSignalReceived(SignalName.ReachedZero, new());
	}

	public StringName UpdatedSignalName => SignalName.Updated;
	public StringName ContextSignalReceivedSignalName => SignalName.ContextSignalReceived;

	public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
	{
		switch (key)
		{
			case "active":
			case "is_active":
			{
				output = Active;
				return true;
			}
			case "tick_rate":
			{
				output = TickRate;
				return true;
			}
		}

		return false;
	}

	public bool GetContextString(string key, string input, ref string replacement, IDataQueryOptions options)
	{
		switch (key)
		{
			case "time":
			{
				if (input == "non_negative")
				{
					replacement = GetFormattedTime(Mathf.Max(0, RemainingTime));
				}
				else
				{
					replacement = GetFormattedTime();
				}
				return true;
			}
			case "total_elapsed_real_time":
			{
				replacement = GetFormattedTime(TotalElapsedRealTime);
				return true;
			}
			case "highest_reached_time":
			{
				replacement = GetFormattedTime(_highestReachedTime);
				return true;
			}
		}
		return false;
	}
}

[StaticDataContext("game_timer_context")]
public struct GameTimerContext : ISingletonContext<GameTimer>, ICacheableDataContext<GameTimerContext>
{
	public bool EqualsContext(GameTimerContext otherCtx) => true;

	public bool CanCache() => true;
}
