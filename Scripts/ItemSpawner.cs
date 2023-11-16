using Godot;
using Godot.Collections;
using System;

public partial class ItemSpawner : Node3D
{
	[Export]
	public string itemName;

	[Export]
	public uint maxItemCount = 1;

	[Export]
	public float radius = 0;
	public uint itemCount;

	[Export]
	public float spawnTime = 0.2f;

	[Export]
	public float spawnChance = 1;

	[Export]
	public uint initialValue = 0;

	[Export]
	public bool debugInfo = false;
	PlayerCmd playerCmd;
	PackedScene itemScene;
	RandomNumberGenerator random;
	WorldCmd worldCmd;
	float worldTime = 1f;
	bool initialized = false;

	float worldTimeDelta = 0;
	public override void _Ready()
	{
		SetProcess(false);
		SetPhysicsProcess(false);
		playerCmd = GetNode<PlayerCmd>("/root/PlayerCmd");
		itemScene = playerCmd.globalInventory.globalItems[itemName].As<Dictionary>()["scene"].As<PackedScene>();
		random = new RandomNumberGenerator();
		random.Randomize();
		worldCmd = GetNode<WorldCmd>("/root/WorldCmd");
		worldCmd.WorldTimeUpdate += OnWorldTimeUpdate;
		this.TreeExiting += Clear;
		for (uint i = 0; i < initialValue; i++) SpawnTick();
	}

	public void OnWorldTimeUpdate(float newWorldTime)
	{
		worldTimeDelta += Mathf.Abs(worldTime - newWorldTime);
		if (worldTimeDelta >= spawnTime)
		{
			SpawnTick();
			worldTimeDelta = 0;
		}
		worldTime = newWorldTime;
	}

	public void SpawnTick()
	{
		if (GetChildCount() <= maxItemCount && random.RandfRange(0, 1) <= spawnChance)
		{
			if (debugInfo) GD.Print($"[{itemName}] {GetChildCount()}");
			float randX = random.RandfRange(GlobalPosition.X - radius, GlobalPosition.X + radius);
			float randZ = random.RandfRange(GlobalPosition.Z - radius, GlobalPosition.Z + radius);
			PickableObject item = itemScene.Instantiate<PickableObject>();
			AddChild(item);
			item.GlobalPosition = new Vector3(randX, GlobalPosition.Y, randZ);
			item.Rotation = item.Rotation with {Y = Mathf.DegToRad(random.RandfRange(0, 360))};
		}
	}

	public void Clear()
	{
		worldCmd.WorldTimeUpdate -= OnWorldTimeUpdate;
	}
}
