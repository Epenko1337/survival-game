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
		audioStreamPlayer.Finished += () => GotoMenu();
    }
    public void Start()
	{
		audioStreamPlayer.Play();
		animationPlayer.Play("fade");
	}

	public void GotoMenu()
	{
		worldCmd.RequestDestroyWorld();
	}
}
