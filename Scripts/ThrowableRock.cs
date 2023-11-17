using Godot;
using Godot.Collections;
using System;

public partial class ThrowableRock : RigidBody3D
{
	WorldCmd worldCmd;
	PlayerCmd playerCmd;
	public override void _Ready()
	{
		this.BodyEntered += OnBodyEntered;
		worldCmd = GetNode<WorldCmd>("/root/WorldCmd");
		playerCmd = GetNode<PlayerCmd>("/root/PlayerCmd");
	}

	public override void _Process(double delta)
	{
	}

	public void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("Animal"))
		{
			Animal bodycast = (Animal)body;
			bodycast.Kill();
		}
		else if (body is CsgBox3D)
		{
			QueueFree();
			PackedScene rockscene = playerCmd.globalInventory.globalItems["rock"].As<Dictionary>()["scene"].As<PackedScene>();
			PickableObject rock = rockscene.Instantiate<PickableObject>();
			worldCmd.RequestAddChild(rock);
			rock.GlobalPosition = GlobalPosition with {Y = 0.35f};
		}
	}
}
