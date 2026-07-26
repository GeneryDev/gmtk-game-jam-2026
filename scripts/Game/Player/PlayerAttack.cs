using GDF.Data;
using GDF.Input;
using GDF.Resources;
using Godot;

namespace Game.Player;

[Tool]
[GlobalClass]
public partial class PlayerAttack : SummarizableScene, IDataContext
{
    [Export] [StoreInSummary] public Texture2D Icon;
    [Export] [StoreInSummary] public GdfInputAction TriggerAction;
    [Export] [StoreInSummary] public int Order;
    [Export] [StoreInSummary] public int MaxAmmo = 1;
    [Export] [StoreInSummary] public float AttackInterval = 0.5f;

    [ExportGroup("Copy Position")]
    [Export(PropertyHint.GroupEnable)] public bool CopyPosition = false;
    [Export] public Node2D CopyPositionFrom;
    [Export] public Node2D CopyPositionTo;
    
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (CopyPosition && CopyPositionFrom != null && CopyPositionTo != null)
        {
            CopyPositionTo.GlobalPosition = CopyPositionFrom.GlobalPosition;
        }
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "icon":
            {
                output = Icon;
                return true;
            }
            case "trigger_action":
            {
                output = TriggerAction;
                return true;
            }
            case "order":
            {
                output = Order;
                return true;
            }
            case "ammo_unlimited":
            case "is_ammo_unlimited":
            {
                output = MaxAmmo < 0;
                return true;
            }
            case "max_ammo":
            {
                output = MaxAmmo;
                return true;
            }
        }

        return false;
    }

    public bool GetSubContext(string key, string input, ref IDataContext output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "live_context":
            {
                output = new LiveWeaponContext(PlayerAttacks.From(this).Id).Boxed();
                return true;
            }
        }

        return false;
    }
}