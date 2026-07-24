using Godot;

namespace Game.Util;

[GlobalClass]
public partial class YSorted : Node
{
    private Node2D _parent;
    
    public float SortKey { get; private set; }

    public void UpdateSortKey()
    {
        SortKey = _parent.GlobalPosition.Y;
    }

    public void SetZIndex(int zIndex)
    {
        _parent.ZIndex = zIndex;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        YSortSystem.Instance?.Add(this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        YSortSystem.Instance?.Remove(this);
    }

    public override void _Notification(int what)
    {
        base._Notification(what);
        switch ((long)what)
        {
            case NotificationParented:
            {
                _parent = GetParentOrNull<Node2D>();
                break;
            }
            case NotificationUnparented:
            {
                _parent = null;
                break;
            }
        }
    }
}