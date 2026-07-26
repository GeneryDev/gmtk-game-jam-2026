using System.Collections.Generic;
using GDF.Data;
using GDF.Util;
using Godot;

namespace Game.Player;

[SingletonUsage(SingletonUsage.Scene)]
public partial class WeaponSystem : SingletonNode<WeaponSystem>, IDataContext
{
    public static CallableEvent UpdatedEvent = new();
    [Signal]
    public delegate void UpdatedEventHandler();
    
    public PlayerAttacks.Descriptor EquippedAttack { get; private set; }
    private Dictionary<StringName, int> _ammoCounts = new();
    
    public override void _Ready()
    {
        base._Ready();
        SetEquipped(PlayerAttacks.Fallback);
        foreach (var attack in PlayerAttacks.CollectAll(new()))
        {
            SetAmmo(attack, attack.Reference.MaxAmmo);
        }
    }

    public void SetEquipped(PlayerAttacks.Descriptor attack)
    {
        EquippedAttack = attack;
        UpdatedEvent.Invoke();
        EmitSignalUpdated();
    }

    public int GetAmmo(PlayerAttacks.Descriptor attack)
    {
        return _ammoCounts[attack.Id];
    }

    public void SetAmmo(PlayerAttacks.Descriptor attack, int ammo)
    {
        _ammoCounts[attack.Id] = ammo;
        UpdatedEvent.Invoke();
        EmitSignalUpdated();
    }

    public bool HasAmmo(PlayerAttacks.Descriptor attack)
    {
        return GetAmmo(attack) != 0;
    }

    public bool ConsumeAmmo(PlayerAttacks.Descriptor attack)
    {
        var ammo = GetAmmo(attack);
        if (ammo != 0)
        {
            if (ammo > 0)
            {
                SetAmmo(attack, ammo - 1);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}

public partial struct LiveWeaponContext : IDataContext, ICacheableDataContext<LiveWeaponContext>
{
    public StringName AttackId;

    public LiveWeaponContext(StringName attackId)
    {
        AttackId = attackId;
    }
    public IDataContext ParentContext => PlayerAttacks.FromId(AttackId).Reference;

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        if (!WeaponSystem.InstanceExists) return false;
        switch (key)
        {
            case "is_equipped":
            {
                output = WeaponSystem.Instance.EquippedAttack.Id == AttackId;
                return true;
            }
            case "ammo":
            {
                output = WeaponSystem.Instance.GetAmmo(PlayerAttacks.FromId(AttackId));
                return true;
            }
        }

        return false;
    }

    public bool EqualsContext(LiveWeaponContext otherCtx)
    {
        return AttackId == otherCtx.AttackId;
    }

    public bool CanCache() => true;

    public void ConnectUpdateSignal(Callable callable)
    {
        WeaponSystem.UpdatedEvent.Connect(callable);
    }
    public void DisconnectUpdateSignal(Callable callable)
    {
        WeaponSystem.UpdatedEvent.Disconnect(callable);
    }
}