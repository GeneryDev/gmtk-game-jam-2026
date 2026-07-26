using System.Collections.Generic;
using GDF.Data;
using Godot;

namespace Game.Entities;

public partial class DamageableComponent : Node, IDataContext
{
	[Signal]
	public delegate void UpdatedEventHandler();

	[Signal]
	public delegate void HurtEventHandler();

	[Signal]
	public delegate void DiedEventHandler();
	
	[Export]
	public int MaxHitPoints = 1;

	[Export]
	public AnimationPlayer deathAnim;

	public int HitPoints = 1;

	public override void _Ready()
	{
		HitPoints = MaxHitPoints;
		base._Ready();
	}

	public void Damage(int hp = 1)
	{
		HitPoints -= hp;
		if (HitPoints < 0) HitPoints = 0;
		EmitSignalHurt();
		if (HitPoints <= 0)
		{
			Kill();
		}
		EmitSignalUpdated();
	}

	public void Kill()
	{
		HitPoints = 0;
		EmitSignalDied();
	}
	public void DeathAnimationFinished(StringName anim)
	{
		Owner.QueueFree();
	}

	public StringName UpdatedSignalName => SignalName.Updated;

	public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
	{
		switch (key)
		{
			case "health":
			{
				output = (float)HitPoints;
				return true;
			}
			case "max_health":
			{
				output = (float)MaxHitPoints;
				return true;
			}
		}

		return false;
	}

	public bool GetContextCollection(string key, string input, List<IDataContext> output, IDataQueryOptions options)
	{
		switch (key)
		{
			case "hit_points":
			{
				for (int i = 0; i < MaxHitPoints; i++)
				{
					output.Add(new PlaceholderDataContext(i < HitPoints ? "filled" : "empty").Boxed());
				}
				return true;
			}
		}

		return false;
	}
}
