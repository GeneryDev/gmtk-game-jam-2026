using Godot;

namespace Game.Util;

public partial class WallBehindFade : Area2DExtension
{
    public override void _Ready()
    {
        base._Ready();
        this.Entered += OnWallBehindEntered;
        this.Exited += OnWallBehindExited;
    }

    private Tween _wallFadeTween;

    private void OnWallBehindEntered()
    {
        _wallFadeTween?.Kill();
        var parent = GetParent<Node2D>();
        _wallFadeTween = CreateTween();
        _wallFadeTween.TweenProperty(parent, new NodePath(CanvasItem.PropertyName.Modulate), new Color(1, 1, 1, 0), 0.2f);
    }

    private void OnWallBehindExited()
    {
        _wallFadeTween?.Kill();
        var parent = GetParent<Node2D>();
        _wallFadeTween = CreateTween();
        _wallFadeTween.TweenProperty(parent, new NodePath(CanvasItem.PropertyName.Modulate), new Color(1, 1, 1, 1), 0.2f);
    }
}