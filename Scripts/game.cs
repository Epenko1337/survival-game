using Godot;
using System;

public partial class game : Node3D
{
	[Export]
	public float worldTime = 1f;

	[Export]
	public float worldTimeSpeed = 0.1f;

	[Export]
	public float worldTimeDaySpeed = 0.05f;

	[Export]
	public float worldTimeNightSpeed = 0.03f;

	[Export]
	public float dayStart = 0.8f;

	[Export]
	public float nightStart = 0.2f;
	private bool timeDirection = true;
	private WorldEnvironment worldEnvironment;

	private bool debugManualTime = false;
	public bool night = false;
	public bool day = true;

	[Export]
	public float timeScale = 1f;

	[Signal]
	public delegate void WorldTimeUpdateEventHandler(float WorldTime);
	public override void _Ready()
	{
		worldEnvironment = GetNode<WorldEnvironment>("WorldEnvironment");
	}

	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
		if (!debugManualTime)
		{
			if (day)
			{
				worldTime = Mathf.Clamp(worldTime + (timeDirection ? -1 : 1) * worldTimeDaySpeed * (float)delta * timeScale, 0, 1);
			}
			else if (night)
			{
				worldTime = Mathf.Clamp(worldTime + (timeDirection ? -1 : 1) * worldTimeNightSpeed * (float)delta * timeScale, 0, 1);
			}
			else
			{
				worldTime = Mathf.Clamp(worldTime + (timeDirection ? -1 : 1) * worldTimeSpeed * (float)delta * timeScale, 0, 1);
			}
			if (worldTime == 0) timeDirection = false;
			else if (worldTime == 1) timeDirection = true;
			night = worldTime <= nightStart;
			day = worldTime >= dayStart;
			EmitSignal(SignalName.WorldTimeUpdate, worldTime);
		}

		worldEnvironment.Environment.BackgroundEnergyMultiplier = worldTime;
		worldEnvironment.Environment.FogLightEnergy = worldTime;
	}

	public void OnDebugManulTime(bool state)
	{
		debugManualTime = state;
	}

	public void OnDebugTimeChange(float newWorldTime)
	{
		if (debugManualTime) worldTime = newWorldTime;
	}
}
