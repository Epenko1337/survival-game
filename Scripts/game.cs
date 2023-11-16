using Godot;
using Godot.Collections;
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
	private WorldCmd worldCmd;
	
	[Export]
	public float timeScale = 1f;
	GridMap pinesGridmap;
	PackedScene pineScene;
	bool worldEnd = false;
	RandomNumberGenerator random;
	AudioStreamPlayer ambientPlayer;

	public override void _Ready()
	{
		worldEnvironment = GetNode<WorldEnvironment>("WorldEnvironment");
		worldCmd = GetNode<WorldCmd>("/root/WorldCmd");
		pinesGridmap = GetNode<GridMap>("Pines");
		pineScene = GD.Load<PackedScene>("res://Scenes/pine.tscn");
		ambientPlayer = GetNode<AudioStreamPlayer>("Ambient");
		random = new RandomNumberGenerator();
		random.Randomize();
		ConnectThings();
		GenerateWorld();
	}

	public void ConnectThings()
	{
		worldCmd.StopWorld += Stop;
		worldCmd.DestroyWorld += Destroy;
	}

	public void DisconnectThings()
	{
		worldCmd.StopWorld -= Stop;
		worldCmd.DestroyWorld -= Destroy;
	}

    public override void _PhysicsProcess(double delta)
    {
		if (worldEnd)
		{
			worldTime = (float)Mathf.Clamp(worldTime - 0.1*delta, 0, 1);
			worldEnvironment.Environment.FogDensity = (float)Mathf.Clamp(worldEnvironment.Environment.FogDensity + 0.1*delta, 0, 1);
		}
		else if (!debugManualTime)
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
			worldCmd.RequestWorldTimeUpdate(worldTime);
		}

		worldEnvironment.Environment.BackgroundEnergyMultiplier = worldTime;
		worldEnvironment.Environment.FogLightEnergy = worldTime;
		if (!worldEnd) worldEnvironment.Environment.FogDensity = 0.1f;
	}

	public void OnDebugManulTime(bool state)
	{
		debugManualTime = state;
	}

	public void OnDebugTimeChange(float newWorldTime)
	{
		if (debugManualTime) worldTime = newWorldTime;
	}

	public void GenerateWorld()
	{
		Array<Vector3I> usedCells = pinesGridmap.GetUsedCells();
		for (int i = 0; i < usedCells.Count; i++)
		{
			Node3D pine = pineScene.Instantiate<Node3D>();
			AddChild(pine);
			pine.GlobalPosition = pinesGridmap.MapToLocal(usedCells[i]) with {Y = 0.35f};
			pine.Rotation = pine.Rotation with {Y = Mathf.DegToRad(random.RandfRange(0, 360))};
		}
		pinesGridmap.QueueFree();
	}

	public void Stop()
	{
		worldEnd = true;
		ambientPlayer.Stop();
	}

	public void Destroy()
	{
		DisconnectThings();
		GetTree().ChangeSceneToFile("res://Scenes/menu.tscn");
	}
}
