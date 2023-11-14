using Godot;
using System;

public partial class healthBar : Node2D
{
	[Export]
	public float width = 200;

	[Export]
	public float height = 20;

	[Export]
	public float maxValue = 100;

	[Export]
	public float value = 50;

	[Export]
	public Color fillColor = new Color(255, 255, 255, 1);

	[Export]
	public Color backgroundColor = new Color(42, 42, 42, 0.5f);

	public override void _Ready()
	{
	}

    public override void _Draw()
    {
		Rect2 rectBack = new Rect2(new Vector2(Position.X, Position.Y - height / 2), new Vector2(width, height));
		Rect2 rectFill = new Rect2(new Vector2(Position.X, Position.Y - height / 2), new Vector2(width*(value/maxValue), height));
		DrawRect(rectBack, backgroundColor);
		DrawRect(rectFill, fillColor);
    }

	public void SetMaxValue(float newvalue)
	{
		maxValue = newvalue;
		QueueRedraw();
	}

	public void SetValue(float newvalue)
	{
		value = newvalue;
		QueueRedraw();
	}
}
