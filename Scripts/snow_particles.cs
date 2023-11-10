using Godot;
using System;



public partial class snow_particles : Node3D
{
	[Export]
	public int particlesAmount = 4000;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GpuParticles3D particles3D = GetNode<GpuParticles3D>("GPUParticles3D");
		particles3D.Amount = particlesAmount;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
