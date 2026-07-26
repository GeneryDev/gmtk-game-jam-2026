using GDF.Resources;
using Godot;

namespace Game.Player;

[Tool]
[GlobalClass]
public partial class PlayerAttack : SummarizableScene
{
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
}