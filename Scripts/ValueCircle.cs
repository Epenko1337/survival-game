using Godot;
using System;

public partial class ValueCircle : Node2D
{
	[Export]
	public float minValue = 0;

	[Export]
	public float maxValue = 100f;
	
	[Export]
	public float value = 50f;

	[Export]
	public float radius = 130f;

	[Export]
	public float width = 5f;

	[Export]
	public Color fillColor = new Color(255, 255, 255, 1);

	[Export]
	public Color backgroundColor = new Color(42, 42, 42, 0.5f);

	public Sprite2D backgroundTexture;

	public override void _Ready()
	{
	}

    public override void _Draw()
    {
		DrawArc(new Vector2(0, 0), radius, Mathf.DegToRad(ClampAnagle(0)), Mathf.DegToRad(ClampAnagle(360)), 12, backgroundColor, width, true);
		DrawArc(new Vector2(0, 0), radius, Mathf.DegToRad(ClampAnagle(0)), Mathf.DegToRad(ClampAnagle(-360*(value/maxValue))), 12, fillColor, width, true);
    }

	private float ClampAnagle(float angle)
	{
		return angle + 270f;
	}

	public void SetMax(float newvalue)
	{
		maxValue = newvalue;
		QueueRedraw();
	}

	public void SetValue(float newvalue)
	{
		value = newvalue;
		QueueRedraw();
	}

	public float GetValue()
	{
		return value;
	}
}
