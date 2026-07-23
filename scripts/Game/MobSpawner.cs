using System.Collections.Generic;
using Game.Entities;
using Game.Timers;
using GDF.Composition;
using GDF.Data;
using GDF.Util;
using Godot;

namespace Game;

public partial class MobSpawner : Node
{
    [Export] public NodeTemplate Template;
    [Export] public int StartAmount = 20;

    private RandomNumberGenerator _rng = new();
    private Accumulator _timer;

    public override void _PhysicsProcess(double delta)
    {
        _timer.Add((float)delta);
        while (_timer.Consume(2))
        {
            Spawn();
        }
        base._PhysicsProcess(delta);
    }

    public override void _Ready()
    {
        base._Ready();
        for (int i = 0; i < StartAmount; i++)
        {
            CallDeferred(MethodName.Spawn);
        }
    }
    
    public void Spawn()
    {
        if (Template == null) return;
        var task = Template.New();
        var instance = task.Instance;
        ((Node2D)instance).Position = NextSpawnPos();
        if (instance.GetComponent<TimerEffectHost>() is { } timerEffectHost)
        {
            var effect = NextTimerEffect();
            if (!effect.IsEmpty)
            {
                timerEffectHost.InstallEffect(effect);
            }

            if (instance.GetComponent<DamageableComponent>() is { } damageable)
            {
                if (effect.Reference.SpriteId == "trivial")
                {
                    damageable.MaxHitPoints = 1;
                }
                else
                {
                    damageable.MaxHitPoints = 4;
                }
            }
        }
        task.Insert();
    }

    private Vector2 NextSpawnPos()
    {
        return new Vector2(_rng.RandfRange(-1, 1), _rng.RandfRange(-1, 1)) * new Vector2(500, 300);
    }

    private readonly List<TimerEffects.Descriptor> _tempEffects = new();
    private float[] _tempWeights;
    
    private TimerEffects.Descriptor NextTimerEffect()
    {
        _tempEffects.Clear();
        TimerEffects.CollectAll(_tempEffects);
        if (_tempEffects.Count <= 0) return default;
        
        if (_tempWeights?.Length != _tempEffects.Count)
            _tempWeights = new float[_tempEffects.Count];
        for (int i = 0; i < _tempEffects.Count; i++)
        {
            var effect = _tempEffects[i];
            _tempWeights[i] = effect.Reference.SpawnWeight;
        }


        var pickedIndex = (int)_rng.RandWeighted(_tempWeights);
        var pickedEffect = _tempEffects[pickedIndex];
        return pickedEffect;
    }
}