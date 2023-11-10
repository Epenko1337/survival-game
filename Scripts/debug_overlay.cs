using Godot;
using System;

public partial class debug_overlay : Node2D
{
	Label fps;
	public override void _Ready()
	{
		fps = GetNode<Label>("FPS");
	}

	public override void _Process(double delta)
	{
		fps.Text = $"FPS: {Engine.GetFramesPerSecond()}";
	}
}
