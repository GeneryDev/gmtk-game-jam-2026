using Game.Entities;
using GDF.Debug;
using GDF.Util;
using Godot;

namespace Game;

[HasDebugCommands]
public partial class MobSpawner : SingletonNode<MobSpawner>
{
    [Export] public int StartAmount = 20;
    [Export] public float SpawnInterval = 2;

    private Accumulator _timer;

    public override void _PhysicsProcess(double delta)
    {
        _timer.Add((float)delta);
        while (_timer.Consume(SpawnInterval))
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
        var instance = MobBuilder.NewMob();
        GetParent().AddChild(instance);
    }

    [DebugCommand("spawn", DebugCommandType.TriggerWithArguments)]
    public static void DebugSpawn(DebugCommandArgumentParser args)
    {
        if (args.ReadWord(out string effectId))
        {
            var effect = MobEffects.FromId(effectId);
            var instance = MobBuilder.NewMob(effect);
            Instance?.GetParent().AddChild(instance);
        }
        else
        {
            args.PrintError();
        }
    }
}