using Godot;
using System;

public partial class Campfire : InteractableObject
{
	[Export]
	public float lifetime = 0.2f;
	Area3D warmArea;
	WorldCmd worldCmd;
	private float worldTime = -1;
	float worldTimeDelta = 0;
	OmniLight3D light;
	GpuParticles3D particles;
	public bool alive = true;
	PlayerCmd playerCmd;
	bool inside = false;
	public bool started = false;
	AudioStreamPlayer audioStreamPlayer;
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		SetProcess(false);
		warmArea = GetNode<Area3D>("WarmArea");
		worldCmd = GetNode<WorldCmd>("/root/WorldCmd");
		playerCmd = GetNode<PlayerCmd>("/root/PlayerCmd");
		light = GetNode<OmniLight3D>("OmniLight3D");
		particles = GetNode<GpuParticles3D>("GPUParticles3D");
		warmArea.AreaEntered += OnAreaEntered;
		warmArea.AreaExited += OnAreaExited;
		light.Visible = false;
		particles.Visible = false;
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
	}

	public void TimeTick(float newWorldTime)
	{
		if (worldTime == -1) 
		{
			worldTime = newWorldTime;
			return;
		}
		worldTimeDelta += Mathf.Abs(worldTime - newWorldTime);
		if (worldTimeDelta >= lifetime)
		{
			BurnOut();
		}
		worldTime = newWorldTime;
	}

    public override void _ExitTree()
    {
		worldCmd.WorldTimeUpdate -= TimeTick;
    }

	public void OnAreaEntered(Area3D area)
	{
		if (area.GetParent() is CharacterBody3D)
		{
			if (alive && started) 
			{
				playerCmd.RequestCampfireEnter(true);
			}
			inside = true;
		}
	}

	public void OnAreaExited(Area3D area)
	{
		if (area.GetParent() is CharacterBody3D)
		{
			if (alive && started) 
			{
				playerCmd.RequestCampfireEnter(false);
			}
			inside = false;
		}
	}

	public void Burn()
	{
		playerCmd.RequestCampfireEnter(true);
		audioStreamPlayer.Play();
		worldCmd.WorldTimeUpdate += TimeTick;
		light.Visible = true;
		particles.Visible = true;
		started = true;
	}

	public void BurnOut()
	{
		worldCmd.WorldTimeUpdate -= TimeTick;
		light.Visible = false;
		particles.Visible = false;
		alive = false;
		if (inside) playerCmd.RequestCampfireEnter(false);
	}
}
