using Godot;
using System;
using System.ComponentModel;

public partial class debug_overlay : Node2D
{
	VBoxContainer container;
	Label fps;
	Label time;
	HSlider timeSlider;
	Label timescale;
	HSlider timescaleSlider;

	[Export]
	public float worldTime = 1;

	[Signal]
	public delegate void RequestManualTimeEventHandler(bool state);

	[Signal]
	public delegate void RequestManualNewTimeEventHandler(bool state);

	public bool manualTime = false;
	public bool manualTimeScale = false;
	private player playerInstance;
	private Label posLabel;
	private WorldCmd worldCmd;
	private Button testBtn;
	private TextEdit itemEdit;
	private Button addItem;
	public override void _Ready()
	{
		container = GetNode<VBoxContainer>("VBoxContainer");
		fps = container.GetNode<Label>("FPS");
		posLabel = container.GetNode<Label>("Position");
		time = container.GetNode<Label>("Time");
		timescale = container.GetNode<Label>("Timescale");
		timeSlider = container.GetNode<HSlider>("TimeSlider");
		timescaleSlider = container.GetNode<HSlider>("TimescaleSlider");
		playerInstance = GetParent<player>();
		worldCmd = GetNode<WorldCmd>("/root/WorldCmd");
		itemEdit = container.GetNode<TextEdit>("ItemText");
		addItem = container.GetNode<Button>("AddItem");
	}

	public override void _Process(double delta)
	{
		if (Visible)
		{
			fps.Text = $"FPS: {Engine.GetFramesPerSecond()}";
			time.Text = $"WorldTime: {worldTime}";
			timescale.Text = $"Timescale: {playerInstance.world.timeScale}";
			posLabel.Text = $"Position: \n\tX: {playerInstance.GlobalPosition.X}\n\tY: {playerInstance.GlobalPosition.Y}\n\tZ: {playerInstance.GlobalPosition.Z}";
			if (!manualTime)
			{
				timeSlider.Value = worldTime;
			}
			timescaleSlider.Value = playerInstance.world.timeScale;
		}
	}

	public void UpdateWorldTime(float newWorldTime)
	{
		worldTime = newWorldTime;
	}

	public void ManualTimeChange(bool state)
	{
		manualTime = state;
		EmitSignal(SignalName.RequestManualTime, state);
	}
	
	public void ManualTimeNew(float value)
	{
		if (manualTime)
		{
			worldTime = value;
			EmitSignal(SignalName.RequestManualNewTime, value);
		}
	}

	public void OnGodmodeChange(bool state)
	{
		playerInstance.godMode = state;
	}

	public void OnTimescaleChange(float newTimescale)
	{
		playerInstance.world.timeScale = newTimescale;
	}

	public void AddItem()
	{
		if (playerInstance.inventory.globalItems.ContainsKey(itemEdit.Text))
		{
			playerInstance.inventory.AddItem(itemEdit.Text, 1);
		}
	}
}
