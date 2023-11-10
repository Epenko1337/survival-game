using Godot;
using System;

public partial class UIButton : Button
{
	AudioStreamPlayer clickStream;
	AudioStreamPlayer hoverStream;
	public override void _Ready()
	{
		clickStream = GetNode<AudioStreamPlayer>("clickStream");
		hoverStream = GetNode<AudioStreamPlayer>("hoverStream");
	}

	public override void _Process(double delta)
	{
	}

	public void BtnDown()
	{
		clickStream.Play();
	}

	public void BtnHover()
	{
		hoverStream.Play();
	}
}
