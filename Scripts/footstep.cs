using Godot;
using System;

public partial class footstep : Sprite3D
{
	[Export]
	public float Lifetime = 2000;  
	private float lifeStart;
	public override void _Ready()
	{
		lifeStart = Time.GetTicksUsec() / 1000;
	}

	public override void _PhysicsProcess(double delta)
	{
		float timeDelta = Time.GetTicksUsec() / 1000 - lifeStart; 
		if (timeDelta >= Lifetime) QueueFree();
		Modulate = Modulate with {A = 1 - timeDelta / Lifetime};
	}
}
