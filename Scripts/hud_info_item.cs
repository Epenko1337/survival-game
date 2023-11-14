using Godot;
using System;

public partial class hud_info_item : Node2D
{
	[Export]
	Texture2D texture;
	public ValueCircle valueCircle;
	
	public override void _Ready()
	{
		Sprite2D icon = GetNode<Sprite2D>("Icon");
		valueCircle = GetNode<ValueCircle>("ValueCircle");
		icon.Texture = texture;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetMaxValue(float value)
	{
		valueCircle.SetMax(value);
	}

	public void SetValue(float value)
	{
		valueCircle.SetValue(value);
	}

	public float GetValue()
	{
		return valueCircle.value;
	}

	public float GetMaxValue()
	{
		return valueCircle.maxValue;
	}
}
