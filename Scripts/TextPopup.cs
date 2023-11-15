using Godot;
using System;

public partial class TextPopup : Node2D
{
	Label label;
	AnimationPlayer animationPlayer;
	AudioStreamPlayer audioStreamPlayer;
	public override void _Ready()
	{
		label = GetNode<Control>("Control").GetNode<Label>("Label");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		animationPlayer.AnimationFinished += (e) => Nuke(e);
	}

	public void Start(string text, bool sound)
	{
		label.Text = text;
		label.Visible = true;
		if (sound) audioStreamPlayer.Play();
		animationPlayer.Play("showandhide");
	}

	public void Nuke(string animationName)
	{
		QueueFree();
	}
}
