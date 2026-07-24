using System.Collections.Generic;
using Game.Timers;
using GDF.Composition;
using GDF.Util;
using Godot;

namespace Game.Entities;

public static class MobBuilder
{
    private static RandomNumberGenerator _rng = new();

    private static List<TimerEffects.Descriptor> _tempEffects = new();
    private static float[] _tempWeights;
    
    public static TimerEffects.Descriptor PickTimerEffect()
    {
        _tempEffects.Clear();
        TimerEffects.CollectAll(_tempEffects);
        if (_tempEffects.Count <= 0) return default;
        
        if (_tempWeights?.Length != _tempEffects.Count)
            _tempWeights = new float[_tempEffects.Count];
        for (int i = 0; i < _tempEffects.Count; i++)
        {
            var effect = _tempEffects[i];
            if (effect.Reference.SpawnCondition?.Evaluate() ?? true)
            {
                _tempWeights[i] = effect.Reference.SpawnWeight;
            }
            else
            {
                _tempWeights[i] = 0;
            }
        }

        var pickedIndex = (int)_rng.RandWeighted(_tempWeights);
        if (pickedIndex == -1) return default;
        var pickedEffect = _tempEffects[pickedIndex];
        return pickedEffect;
    }
    
    public static Vector2 PickSpawnPos()
    {
        return new Vector2(_rng.RandfRange(-1, 1), _rng.RandfRange(-1, 1)) * new Vector2(500, 300);
    }

    public static Node NewMob()
    {
        var effect = PickTimerEffect();
        if (effect.IsEmpty) return null;
        var mobType = MobTypes.SelectForEffect(effect, _rng);
        if (mobType.IsEmpty) return null;

        var templateScene = mobType.Resource.TemplateScene;
        var instance = templateScene.GdfInstantiate<Node2D>();
        
        // Initialize
        instance.Position = PickSpawnPos();
        instance.GetComponent<TimerEffectHost>()?.InstallEffect(effect);

        return instance;
    }
}