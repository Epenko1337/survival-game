using Godot;
using System;

public partial class AnimalSpawner : ItemSpawner
{
    public override void _Ready()
    {
        SetProcess(false);
		SetPhysicsProcess(false);
		playerCmd = GetNode<PlayerCmd>("/root/PlayerCmd");
		itemScene = GD.Load<PackedScene>(itemName);
		random = new RandomNumberGenerator();
		random.Randomize();
		worldCmd = GetNode<WorldCmd>("/root/WorldCmd");
		worldCmd.WorldTimeUpdate += OnWorldTimeUpdate;
		this.TreeExiting += Clear;
		for (uint i = 0; i < initialValue; i++) SpawnTick();
    }

    public override void SpawnTick()
	{
		if (GetChildCount() <= maxItemCount && random.RandfRange(0, 1) <= spawnChance)
		{
			if (debugInfo) GD.Print($"[{itemName}] {GetChildCount()}");
			float randX = random.RandfRange(GlobalPosition.X - radius, GlobalPosition.X + radius);
			float randZ = random.RandfRange(GlobalPosition.Z - radius, GlobalPosition.Z + radius);
			Animal item = itemScene.Instantiate<Animal>();
			AddChild(item);
			item.GlobalPosition = new Vector3(randX, GlobalPosition.Y, randZ);
			item.Rotation = item.Rotation with {Y = Mathf.DegToRad(random.RandfRange(0, 360))};
		}
	}
}