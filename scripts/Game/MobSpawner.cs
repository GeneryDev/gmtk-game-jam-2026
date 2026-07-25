using Game.Entities;
using Game.Timers;
using GDF.Data;
using GDF.Data.Static;
using GDF.Debug;
using GDF.Util;
using Godot;

namespace Game;

[HasDebugCommands]
[SingletonUsage(SingletonUsage.Scene)]
public partial class MobSpawner : SingletonNode<MobSpawner>, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();
    
    [Export] public int StartAmount = 20;
    
    private int _totalSpawned = 0;
    private int _highestMobCount = 0;

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
        NotifySpawned();
    }

    private void NotifySpawned()
    {
        _totalSpawned++;
        _highestMobCount = GetTree().GetNodeCountInGroup("mob");
        EmitSignalUpdated();
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "total_mobs_spawned":
            {
                output = _totalSpawned;
                return true;
            }
            case "highest_mob_count":
            {
                output = _highestMobCount;
                return true;
            }
        }

        return false;
    }

    [DebugCommand("spawn", DebugCommandType.TriggerWithArguments)]
    public static void DebugSpawn(DebugCommandArgumentParser args)
    {
        if (args.ReadWord(out string effectId))
        {
            var effect = MobEffects.FromId(effectId);
            var instance = MobBuilder.NewMob(effect);
            Instance?.GetParent().AddChild(instance);
            Instance?.NotifySpawned();
        }
        else
        {
            args.PrintError();
        }
    }
}

[StaticDataContext("mob_spawner_context")]
public struct MobSpawnerContext : ISingletonContext<MobSpawner>, ICacheableDataContext<MobSpawnerContext>
{
    public bool EqualsContext(MobSpawnerContext otherCtx) => true;

    public bool CanCache() => true;
}
