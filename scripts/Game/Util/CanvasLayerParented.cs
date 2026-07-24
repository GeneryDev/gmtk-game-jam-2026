using Godot;

namespace Game.Util;

[GlobalClass]
public partial class CanvasLayerParented : CanvasLayer
{
    private CanvasItem _parent;

    public override void _Process(double delta)
    {
        base._Process(delta);
        this.Transform = _parent.GetGlobalTransform();
    }

    public override void _Notification(int what)
    {
        base._Notification(what);
        switch ((long)what)
        {
            case NotificationParented:
            {
                _parent = GetParentOrNull<CanvasItem>();
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