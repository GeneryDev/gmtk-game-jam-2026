using System.Collections.Generic;
using Godot;
using MobEffectHost = Game.Entities.MobEffectHost;

namespace Game.Timers;

public partial class GameTimer
{
    private readonly List<MobEffectHost> _tempEffectHosts = new();
    private float[] _tempWeights;

    private readonly RandomNumberGenerator _rng = new();

    private void TriggerEffects()
    {
        _tempEffectHosts.Clear();
        foreach (var node in GetTree().GetNodesInGroup(MobEffectHost.Group))
        {
            if (node is not MobEffectHost host) continue;
            if (host.TriggerWeight <= 0) continue;
            if (!host.ValidTarget) continue;
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