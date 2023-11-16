using Godot;
using System;

public partial class menu : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void StartGame()
	{
		GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
	}

	public void GotoSettings()
	{
		GetTree().ChangeSceneToFile("res://Scenes/soundsettings.tscn");
	}

	public void Exit()
	{
		GetTree().Quit();
	}
}
