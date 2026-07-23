using System.Collections.Generic;
using Game.Timers;
using Godot;

namespace Game;

public partial class GameTimer
{
    private readonly List<TimerEffectHost> _tempEffectHosts = new();
    private float[] _tempWeights;

    private readonly RandomNumberGenerator _rng = new();

    private void TriggerEffects()
    {
        _tempEffectHosts.Clear();
        foreach (var node in GetTree().GetNodesInGroup(TimerEffectHost.Group))
        {
            if (node is not TimerEffectHost host) continue;
            if (host.TriggerWeight <= 0) continue;
            _tempEffectHosts.Add(host);
        }

        if (_tempEffectHosts.Count <= 0) return;

        _tempWeights = new float[_tempEffectHosts.Count];
        for (var i = 0; i < _tempEffectHosts.Count; i++)
        {
            var host = _tempEffectHosts[i];
            _tempWeights[i] = host.TriggerWeight;
        }

        var pickedIndex = (int)_rng.RandWeighted(_tempWeights);
        var pickedHost = _tempEffectHosts[pickedIndex];
        pickedHost.Trigger();
    }
}