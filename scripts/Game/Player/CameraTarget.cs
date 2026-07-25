using GDF.Composition;
using GDF.PropertyStacks;
using Godot;

namespace Game.Player;

public partial class CameraTarget : Node2D
{
    private PropertyFrame _frame;

    [Export] public bool CopyPosition = true;
    [Export] public bool CopyRotation = true;

    private PropertyStack GetStack()
    {
        return GetViewport()?.GetCamera2D()?.GetComponent<PropertyStack>();
    }

    private void UpdateFrame()
    {
        if(CopyPosition)
            _frame?.Set("camera_position", this.GlobalPosition);
        if (CopyRotation)
            _frame?.Set("camera_rotation", this.GlobalRotation);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        UpdateFrame();
    }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        _frame = GetStack()?.NewFrame("Camera Target", order: -1).BindToNode(this);
    }

    public override void _ExitTree()
    {
        _frame = _frame?.Remove();
    }
}