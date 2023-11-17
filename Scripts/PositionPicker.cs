using Godot;
using System;

public partial class PositionPicker : MeshInstance3D
{
	public PackedScene targetScene;
	public Node3D parent;
	public WorldCmd worldCmd;
	public AudioStreamPlayer audioStreamPlayer;
	public override void _Ready()
	{
		worldCmd = GetNode<WorldCmd>("/root/WorldCmd");
		audioStreamPlayer = GetNode<AudioStreamPlayer>("placeSound");
		SetProcess(false);
		SetPhysicsProcess(false);
	}

    public override void _PhysicsProcess(double delta)
    {
		this.GlobalPosition = this.GlobalPosition with {X = parent.GlobalPosition.X + (float)Math.Sin(parent.Rotation.Y + 0.7853981633974483f), Z = parent.GlobalPosition.Z + (float)Math.Cos(parent.Rotation.Y + 0.7853981633974483f)};
        if (Input.IsMouseButtonPressed(MouseButton.Left))
		{
			audioStreamPlayer.Play();
			Disable();
			Node3D node = targetScene.Instantiate<Node3D>();
			worldCmd.RequestAddChild(node);
			node.GlobalPosition = this.GlobalPosition;
		}
    }

    public void Enable(Node3D originParent, PackedScene target)
	{
		parent = originParent;
		targetScene = target;
		SetPhysicsProcess(true);
		this.Visible = true;
	}

	public void Disable()
	{
		SetPhysicsProcess(false);
		this.Visible = false;
	}
}
