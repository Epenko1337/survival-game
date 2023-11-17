using Godot;
using System;

public partial class deathScreen : Node2D
{
	AudioStreamPlayer audioStreamPlayer;
	AnimationPlayer animationPlayer;
	PackedScene menuScene;
	WorldCmd worldCmd;

    public override void _Ready()
    {
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		worldCmd = GetNode<WorldCmd>("/root/WorldCmd");
    }
    public void Start()
	{
		audioStreamPlayer.Play();
		animationPlayer.Play("blink_loop");
	}

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("space")) GotoMenu();
    }

    public void GotoMenu()
	{
		worldCmd.RequestDestroyWorld();
	}
}
