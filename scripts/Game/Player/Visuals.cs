using Godot;
using System;

public partial class Visuals : CanvasGroup
{
    [Export] AnimationPlayer anim;
    public override void _Ready()
    {
        base._Ready();
        anim.Play("idle");
    }

}
