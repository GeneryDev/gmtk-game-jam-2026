using GDF.Composition;
using GDF.Data;
using Godot;

namespace Game.Timers;

public partial class TimerEffectHost : Node, IDataContext
{
    public static readonly StringName Group = "timer_effect_host";

    [Signal]
    public delegate void TriggeredEventHandler();

    [Export] public float TriggerWeight = 1;

    private TimerEffect _activeEffect;

    public override void _EnterTree()
    {
        base._EnterTree();
        this.AddToGroup(Group);
    }

    public void InstallEffect(TimerEffects.Descriptor effect)
    {
        if (_activeEffect != null)
        {
            _activeEffect.GetParent().RemoveChild(_activeEffect);
            _activeEffect.QueueFree();
            _activeEffect = null;
        }

        _activeEffect = effect.New();
        this.AddChild(_activeEffect);
        _activeEffect.Owner = this.Owner;
        if (_activeEffect.SpriteId != "trivial")
            TestRandomColor();
    }

    private void TestRandomColor()
    {
        if (_activeEffect == null) return;
        var colorRandom = new RandomNumberGenerator()
        {
            // Seed = _activeEffect.Descriptor.Id.ToString().Hash()
        };
        var color = new Color(colorRandom.RandfRange(0.5f, 1), colorRandom.RandfRange(0.5f, 1),
            colorRandom.RandfRange(0.5f, 1));
        if (this.GetComponent<MeshInstance2D>() is { } mesh)
            mesh.Modulate = color;
    }

    public void Trigger()
    {
        if (_activeEffect == null) return;
        _activeEffect?.Trigger();
        EmitSignalTriggered();
    }

    public bool GetSubContext(string key, string input, ref IDataContext output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "effect":
            case "effect_context":
            {
                output = _activeEffect;
                return true;
            }
        }

        return false;
    }
}