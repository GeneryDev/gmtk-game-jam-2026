using Game.Entities;
using Godot;
using System;

public partial class WalkingEffect : Node

{
	[Export] GpuParticles2D Particles;
	[Export] public float VelocityToParticles = 0.066f;
	[Export] public MotionComponent MotionBody;
	CpuParticles2D[] particles;
	public override void _Ready()
	{
		base._Ready();
	}

	public override void _Process(double delta)
	{
		double velocityMagnitude = MotionBody.LastObservedVelocity.Length();
		Particles.AmountRatio = (float)velocityMagnitude * VelocityToParticles;
		
	}
	
}
