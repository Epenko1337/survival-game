using Godot;
using System;

public partial class InteractableObject : Node3D
{
	public override void _Ready()
	{
		SetProcess(false);
		SetPhysicsProcess(false);
		SetProcessInput(false);
		SetProcessUnhandledInput(false);
	}
}
