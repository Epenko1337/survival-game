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
	float maxX;
	float maxZ;
	float minX;
	float minZ;
	RandomNumberGenerator random;
	WorldCmd worldCmd;
	float worldTime = 1f;

	float worldTimeDelta = 0;
	public override void _Ready()
	{
		SetProcess(false);
		SetPhysicsProcess(false);
		playerCmd = GetNode<PlayerCmd>("/root/PlayerCmd");
		itemScene = playerCmd.globalInventory.globalItems[itemName].As<Dictionary>()["scene"].As<PackedScene>();
		maxX = GlobalPosition.X + radius;
		maxZ = GlobalPosition.Z + radius;
		minX = GlobalPosition.X - radius;
		minZ = GlobalPosition.Z - radius;
		random = new RandomNumberGenerator();
		random.Randomize();
		worldCmd = GetNode<WorldCmd>("/root/WorldCmd");
		worldCmd.WorldTimeUpdate += (e) => OnWorldTimeUpdate(e);
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
			float randX = random.RandfRange(minX, maxX);
			float randZ = random.RandfRange(minZ, maxZ);
			PickableObject item = itemScene.Instantiate<PickableObject>();
			AddChild(item);
			item.GlobalPosition = new Vector3(randX, GlobalPosition.Y, randZ);
			item.Rotation = item.Rotation with {Y = random.RandfRange(0, 360)};
		}
	}
}
