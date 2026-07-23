using System.Collections.Generic;
using GDF.Composition;
using GDF.Data;
using Godot;

namespace Game.Timers;

public partial class TimerEffectHost : Node, IDataContext
{
    public static readonly StringName Group = "timer_effect_host";

    [Signal]
    public delegate void TriggeredEventHandler();

    [Signal]
    public delegate void UpdatedEventHandler();

    [Export] public float TriggerWeight = 1;

    private readonly List<TimerEffect> _installedEffects = new();
    private readonly RandomNumberGenerator _rng = new();

    private TimerEffect _lastTriggeredEffect;

    public override void _EnterTree()
    {
        base._EnterTree();
        this.AddToGroup(Group);
    }

    public void InstallEffect(TimerEffects.Descriptor effectDescriptor)
    {
        if (effectDescriptor.IsEmpty) return;
        var effect = effectDescriptor.New();
        _installedEffects.Add(effect);
        this.AddChild(effect);
        effect.Owner = this.Owner;
    }

    private void UninstallEffect(TimerEffect effect)
    {
        if (effect == null) return;
        effect.GetParent()?.RemoveChild(effect);
        effect.QueueFree();
        _installedEffects.Remove(effect);
        if (_lastTriggeredEffect == effect) _lastTriggeredEffect = null;
    }

    public void Trigger()
    {
        if (_installedEffects.Count == 0) return;

        int pickedIndex = _rng.RandiRange(0, _installedEffects.Count - 1);
        var pickedEffect = _installedEffects[pickedIndex];
        Trigger(pickedEffect);
    }

    private void Trigger(TimerEffect effect)
    {
        if (effect == null) return;
        _lastTriggeredEffect = effect;
        effect.Trigger();
        EmitSignalTriggered();
        EmitSignalUpdated();
    }

    public StringName UpdatedSignalName => SignalName.Updated;

    public bool GetSubContext(string key, string input, ref IDataContext output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "first_effect":
            case "first_effect_context":
            {
                if (_installedEffects.Count > 0)
                {
                    output = _installedEffects[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            case "last_triggered_effect":
            case "last_triggered_effect_context":
            {
                output = _lastTriggeredEffect;
                return true;
            }
        }

        return false;
    }

    public bool GetContextCollection(string key, string input, List<IDataContext> output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "effects":
            {
                output.AddRange(_installedEffects);
                return true;
            }
        }

        return false;
    }
}