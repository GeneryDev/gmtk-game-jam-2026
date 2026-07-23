using Godot;
using System;

public partial class WalkingEffect : Node

{
	[Export] CpuParticles2D leftFootParticles;
	[Export] CpuParticles2D rightFootParticles;
	[Export] public double VelocityToParticles = 0.05;
	[Export] public CharacterBody2D body;
	CpuParticles2D[] particles;
	public override void _Ready()
	{
		base._Ready();
	}

	public override void _Process(double delta)
	{
		double velocityMagnitude = body.Velocity.Length();
		GD.Print(velocityMagnitude);
		
		//if (velocityMagnitude < 0.005f)
		//{
			//leftFootParticles.Emitting = false;
			//rightFootParticles.Emitting = false;
		//}
		//else
		//{
			//leftFootParticles.Emitting = true;
			//rightFootParticles.Emitting = true; 
		//}

		//int amount = (int) Math.Ceiling(velocityMagnitude * VelocityToParticles);
		//// GD.Print(amount);
		//leftFootParticles.Amount = amount;
		//rightFootParticles.Amount = amount;
		// Velocity magnitude reporting 0!??!? for all of its movement. That could not be right, and it think it is reading within the scope of its scene.
		// DO NOT USE Dynamic steps. 
	}
	
}
