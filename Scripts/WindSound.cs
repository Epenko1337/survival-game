using Godot;
using System;

public partial class WindSound : AudioStreamPlayer
{
	public override void _Ready()
	{
		Random random = new Random();
		this.Play(random.Next(0, (int)this.Stream.GetLength()));	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnFinished()
	{
		Play();
	}
}
