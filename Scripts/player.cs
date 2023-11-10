using Godot;
using System;
using System.Collections.Generic;

public partial class player : CharacterBody3D
{
	[Export]
	public float Speed = 2.5f;

	[Export]
	public float SprintMultiplier = 1.3f;
	public const float JumpVelocity = 4.5f;
	public float cameraDistance = 0f;

	[Export]
	public float minCameraDistance = 0f;

	[Export]
	public float maxCameraDistance = 15.0f;

	[Export]
	public float angleLerp = 0.15f;

	[Export]
	public float velocityLerp = 0.15f;

	[Export]
	public float cameraStep = 0.5f;

	[Export]
	public float cameraLerp = 0.15f;

	private Camera3D Camera;
	private AnimationPlayer animationPlayer;
	private Node3D playerModel;
	private AnimationTree animationTree;
	private double lastStepTime = Time.GetTicksUsec();
	private AudioStreamPlayer audioStreamPlayer;
	private AudioStreamPlaybackPolyphonic polyphonicPlayback;
	private List<AudioStreamWav> snowStepsSounds = new List<AudioStreamWav>(10); 
	private Random randomizer = new Random();
	private Vector2 lastPos;
	private PackedScene footstepScene;
	private bool lastStepInverse = false;


	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
		Camera = GetNode<Camera3D>("Camera");
        SetCameraPosition(minCameraDistance, true);

		playerModel = GetNode<Node3D>("Model");
		animationPlayer = playerModel.GetNode<AnimationPlayer>("AnimationPlayer");
		animationTree = playerModel.GetNode<AnimationTree>("AnimationTree");
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioStreamPlayer.Play();
		polyphonicPlayback = (AudioStreamPlaybackPolyphonic)audioStreamPlayer.GetStreamPlayback();
		for (int i = 0; i <= 7; i++)
		{
			snowStepsSounds.Add(GD.Load<AudioStreamWav>($"res://Assets/Sounds/walking/snow_step{i}.wav"));
		}
		lastPos = new Vector2(Position.X + 1000, Position.Z + 1000);
		footstepScene = GD.Load<PackedScene>("res://Scenes/footstep.tscn");
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		//gravity
		//if (!IsOnFloor())
			//velocity.Y -= gravity * (float)delta;
		
		//jump
		//if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			//velocity.Y = JumpVelocity;

		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		bool sprinting = Input.IsActionPressed("sprint");
		if (direction != Vector3.Zero)
		{
			velocity.X = Mathf.Lerp(velocity.X, direction.X * (Speed * (sprinting ? SprintMultiplier : 1)), velocityLerp);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * (Speed * (sprinting ? SprintMultiplier : 1)), velocityLerp);

			Vector3 newRotation = playerModel.Rotation;
			newRotation.Y = Mathf.LerpAngle(newRotation.Y, Mathf.Atan2(velocity.X, velocity.Z) - 0.7853981633974483f, angleLerp);
			playerModel.Rotation = newRotation;

			Vector3 playerPos = this.Position;
			Vector2 currPos = new Vector2(playerPos.X, playerPos.Z);

			if (lastPos.DistanceTo(currPos) > 1.2)
			{
				lastPos = currPos;
				PlaySound(snowStepsSounds[randomizer.Next(0, 8)]);
				Decal footStep = footstepScene.Instantiate<Decal>();
				Vector3 footStepPos = playerPos;
				footStepPos.Y = 0;
				footStep.Position = footStepPos;
				newRotation.Y += 0.7853981633974483f;
				newRotation.Z = lastStepInverse ? Mathf.DegToRad(180) : 0;
				lastStepInverse = !lastStepInverse;
				footStep.Rotation = newRotation;
				GetParent().AddChild(footStep);
			}
		}
		else
		{
			velocity.X = Mathf.Lerp(Velocity.X, 0, velocityLerp);
			velocity.Z = Mathf.Lerp(Velocity.Z, 0, velocityLerp);
		}
		animationTree.Set("parameters/WalkSpeed/scale", velocity.Length() / 2.5);
		animationTree.Set("parameters/RunSpeed/scale", velocity.Length() / 4);
		float animAmount = velocity.Length() / (sprinting ? Speed*SprintMultiplier : Speed) * (sprinting ? 2 : 1);
		float currRunBlend = (float)animationTree.Get("parameters/RunBlend/blend_amount");

		if (animAmount <= 1)
		{
			animationTree.Set("parameters/WalkBlend/blend_amount", animAmount);
			animationTree.Set("parameters/RunBlend/blend_amount", Mathf.Lerp(currRunBlend, 0, velocityLerp));
		}
		else
		{
			animationTree.Set("parameters/WalkBlend/blend_amount", 1);
			animationTree.Set("parameters/RunBlend/blend_amount", Mathf.Lerp(currRunBlend, animAmount - 1, velocityLerp));
		}

		if (Input.IsActionJustReleased("camera+"))
		{
			float newDistance = cameraDistance + cameraStep;
			if (newDistance <= maxCameraDistance && newDistance >= minCameraDistance)
			{
				cameraDistance += cameraStep;
			}
		}
		else if (Input.IsActionJustReleased("camera-"))
		{
			float newDistance = cameraDistance - cameraStep;
			if (newDistance <= maxCameraDistance && newDistance >= minCameraDistance)
			{
				cameraDistance -= cameraStep;
			}
		}
		UpdateCameraPosition();
		Velocity = velocity;
		MoveAndSlide();
	}

	private void SetCameraPosition(float value, bool force=false)
	{
		float newDistance = cameraDistance + value;
		if (newDistance <= maxCameraDistance && newDistance >= minCameraDistance || force)
		{
			cameraDistance += value;
			Vector3 newTransform = Camera.Transform.Origin;
			newTransform += new Vector3(0, value, value*1.58f);
			Camera.Position = newTransform;
		}
	}

	private void UpdateCameraPosition()
	{
		Vector3 cameraPosition = Camera.Position;
		cameraPosition.Y = Mathf.Lerp(cameraPosition.Y, cameraDistance - 0.5f, cameraLerp);
		cameraPosition.Z = Mathf.Lerp(cameraPosition.Z, cameraDistance*1.58f, cameraLerp*1.58f);
		Camera.Position = cameraPosition;
	}

	private void PlaySound(AudioStream stream)
	{
		polyphonicPlayback.PlayStream(stream);
	}
}
