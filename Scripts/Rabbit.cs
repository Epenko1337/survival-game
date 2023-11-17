using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Rabbit : Animal
{
	AudioStreamPlayer audioStreamPlayer;
	public override void _Ready()
	{
		base._Ready();
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
	}

    public override void OnKill()
    {
        audioStreamPlayer.Play();
    }
}