using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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

	[Export]
	public float maxPickupDistance = 1.4f;

	[Export]
	public float maxCapacity = 15f;

	[Export]
	public float capacity = 0f;

	[Signal]
	public delegate void DebugManualTimeEventHandler(bool state);

	[Signal]
	public delegate void DebugManualNewTimeEventHandler(float value);

	[Signal]
	public delegate void WantsPickupEventHandler(PickableObject pickableObject, Vector2 screenPos);

	[Signal]
	public delegate void WantsFireStartEventHandler(Campfire campfire, Vector2 screenPos);

	private Camera3D Camera;
	private AnimationPlayer animationPlayer;
	private Node3D playerModel;
	private AnimationTree animationTree;
	private double lastStepTime = Time.GetTicksUsec();
	private AudioStreamPlayer audioStreamPlayer;
	private AudioStreamPlaybackPolyphonic polyphonicPlayback;
	private List<AudioStreamWav> snowStepsSounds = new List<AudioStreamWav>(10); 
	private Random randomizer = new Random();
	private Vector3 lastPos;
	private PackedScene footstepScene;
	private bool lastStepInverse = false;
	private float worldTime = 1;
	private Node2D debugOverlay;
	private float internalTempMax = 100f;
	private float internalTemp = 100f;
	private float fatigueMax = 100f;
	private float fatigue = 100f;
	private float thirstMax = 100f;
	private float thirst = 100f;
	private float hungerMax = 100f;
	private float hunger = 100f;
	private float healthMax = 100f;
	private float health = 100f;
	private float staminaMax = 100f;
	private float stamina = 100f;
	private bool sprinting = false;
	private bool moving = false;
	private HUD PlayerHUD;
	private bool staminaDrained = false;
	public game world;
	private float worldTimeDelta = 0;
	private float minuteScale = 0.0006944444444444444f;
	public bool godMode = false;
	private Vector2 screenSize;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	PackedScene debugRect;
	private bool rotationControlled = false;
	public Inventory inventory;
	public bool movementBlock = false;
	public float distancePassed = 0;
	public PickableObject pickupTarget;
	public AudioStream hintSound;
	public AudioStream itemSound;
	PlayerCmd playerCmd;
	WorldCmd worldCmd;
	PackedScene deathScreenScene;
	public AudioStream deathMusic;
	public RandomNumberGenerator random;

	public bool campfireBonus = false;
	public AudioStream flintSound;
	public bool inPosSelectMode = false;
	public PositionPicker positionPicker;
	public AudioStream boilSound;
	public AudioStream drinkingSound;
	public AudioStream eatingSound;
	public BoneAttachment3D handAttachment;
	public PackedScene throwableRockScene;
	public MeshInstance3D crosshair;
	public AudioStream cuttingSound;

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
		lastPos = new Vector3(Position.X + 1000, Position.Y + 1000, Position.Z + 1000);
		footstepScene = GD.Load<PackedScene>("res://Scenes/footstep.tscn");
		debugOverlay = GetNode<Node2D>("DebugOverlay");
		PlayerHUD = GetNode<HUD>("HUD");
		world = GetParent<game>();
		PlayerHUD.tempStatus.SetMaxValue(internalTempMax);
		PlayerHUD.fatigueStatus.SetMaxValue(fatigueMax);
		PlayerHUD.thirstStatus.SetMaxValue(thirstMax);
		PlayerHUD.hungerStatus.SetMaxValue(hungerMax);
		PlayerHUD.healthbar.SetMaxValue(healthMax);
		PlayerHUD.staminaStatus.SetMaxValue(stamina);
		UpdateHUDInfos();
		screenSize = GetViewport().GetVisibleRect().Size;
		debugRect = GD.Load<PackedScene>("res://Scenes/debug_rect.tscn");
		inventory = new Inventory();
		inventory.maxCapacity = maxCapacity;
		animationTree.AnimationFinished += (e) => OnAnimFinish(e);
		hintSound = GD.Load<AudioStream>("res://Assets/Sounds/ui/hint.wav");
		playerCmd = GetNode<PlayerCmd>("/root/PlayerCmd");
		itemSound = GD.Load<AudioStream>("res://Assets/Sounds/pickup/itemthing.wav");
		worldCmd = GetNode<WorldCmd>("/root/WorldCmd");
		deathScreenScene = GD.Load<PackedScene>("res://Scenes/deathScreen.tscn");
		ConnectThings();
		playerCmd.globalInventory = inventory;
		deathMusic = GD.Load<AudioStream>("res://Assets/Sounds/ambience/deathodessy.mp3");
		playerCmd.CampfireSwitch += OnCampfireSwitch;
		inventory.AddItem("knife", 1);
		random = new RandomNumberGenerator();
		random.Randomize();
		flintSound = GD.Load<AudioStream>("res://Assets/Sounds/flintsteel.wav");
		positionPicker = GetNode<PositionPicker>("positionPicker");
		boilSound = GD.Load<AudioStream>("res://Assets/Sounds/waterpour.mp3");
		drinkingSound = GD.Load<AudioStream>("res://Assets/Sounds/drinking.mp3");
		eatingSound = GD.Load<AudioStream>("res://Assets/Sounds/eating.wav");
		handAttachment = playerModel.GetNode<BoneAttachment3D>("Armature/Skeleton3D/HandAttachment");
		throwableRockScene = GD.Load<PackedScene>("res://Scenes/ThrowableRock.tscn");
		crosshair = GetNode<MeshInstance3D>("Crosshair");
		cuttingSound = GD.Load<AudioStream>("res://Assets/Sounds/cut.wav");
    }

	public void ConnectThings()
	{
		playerCmd.DropItem += OnDropItem;
		worldCmd.WorldTimeUpdate += OnWorldTimeUpdate;
		playerCmd.CraftItem += OnCraft;
		playerCmd.Drink += OnDrink;
		playerCmd.Eat += OnEat;
	}

	public void DisconnectThings()
	{
		playerCmd.DropItem -= OnDropItem;
		worldCmd.WorldTimeUpdate -= OnWorldTimeUpdate;
		playerCmd.CraftItem -= OnCraft;
		playerCmd.Drink -= OnDrink;
		playerCmd.Eat -= OnEat;
	}

    public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("debug_menu"))
		{
			debugOverlay.Visible = !debugOverlay.Visible;
		}

		if (Input.IsActionJustPressed("inventory_open") && !movementBlock)
		{
			PlayerHUD.SwitchInventory();
		}
		else if (Input.IsActionJustPressed("craftmenu_open") && !movementBlock)
		{
			PlayerHUD.SwitchCraftmenu();
		}
		crosshair.Visible = Input.IsActionPressed("crosshair") && rotationControlled;

		Vector3 velocity = Velocity;

		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		sprinting = Input.IsActionPressed("sprint") && stamina > 0 && !staminaDrained && fatigue > 0 && !rotationControlled;
		moving = direction != Vector3.Zero;
		if (direction != Vector3.Zero && !movementBlock)
		{
			velocity.X = Mathf.Lerp(velocity.X, direction.X * (Speed * (sprinting ? SprintMultiplier : 1)), velocityLerp);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * (Speed * (sprinting ? SprintMultiplier : 1)), velocityLerp);

			Vector3 newRotation = playerModel.Rotation;
			newRotation.Y = Mathf.LerpAngle(newRotation.Y, Mathf.Atan2(velocity.X, velocity.Z) - 0.7853981633974483f, angleLerp);
			if (!rotationControlled) playerModel.Rotation = newRotation;
			Vector3 playerPos = this.Position;
			distancePassed += playerPos.DistanceTo(lastPos);
			lastPos = playerPos;

			if (distancePassed >= 1.2f)
			{
				distancePassed = 0;
				PlaySound(snowStepsSounds[randomizer.Next(0, 8)]);
				Sprite3D footStep = footstepScene.Instantiate<Sprite3D>();
				Vector3 footStepPos = playerPos;
				footStepPos.Y = 0;
				footStep.Position = footStepPos;
				newRotation.Y += 0.7853981633974483f;
				footStep.FlipH = lastStepInverse;
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

		if (rotationControlled && !movementBlock)
		{
			Vector3 newRotation = playerModel.Rotation;
			Vector2 viewportCenter = screenSize;
			viewportCenter.X /= 2;
			viewportCenter.Y /= 2;
			Vector2 mousePos = GetViewport().GetMousePosition();
			newRotation.Y = Mathf.LerpAngle(newRotation.Y, Mathf.Atan2(viewportCenter.X - mousePos.X, viewportCenter.Y - mousePos.Y) + Mathf.DegToRad(180), angleLerp);
			playerModel.Rotation = newRotation;
			crosshair.Rotation = newRotation;
		}

		animationTree.Set("parameters/WalkSpeed/scale", velocity.Length() / 2.5f);
		animationTree.Set("parameters/RunSpeed/scale", velocity.Length() / 4f);
		float animAmount = velocity.Length() / (sprinting ? Speed*SprintMultiplier : Speed) * (sprinting ? 2 : 1);
		float currRunBlend = (float)animationTree.Get("parameters/RunBlend/blend_amount");
		float controllRotationBlend = (float)animationTree.Get("parameters/controlledBlend/blend_amount");
		animationTree.Set("parameters/controlledBlend/blend_amount", Mathf.Lerp(controllRotationBlend, rotationControlled && !movementBlock ? 1 : 0, velocityLerp));

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
		UpdateHUDInfos();
		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
    {
		if (@event is InputEventMouse eventMouse)
		{
			if (eventMouse.ButtonMask == MouseButtonMask.Right)
			{
				if (eventMouse.IsPressed() || @event is InputEventMouseMotion) 
				{
					if (eventMouse.IsPressed())
					{
						Dictionary dict = ScreenToWorld(eventMouse.Position, 2);
						if (dict.ContainsKey("collider"))
						{
							Node node = dict["collider"].As<Node>();
							if (node is RigidBody3D)
							{
								InteractionMenu(node, eventMouse.Position);
							}
							else
							{
								Node parent = node.GetParent();
								InteractionMenu(parent, eventMouse.Position);
							}
						}
					}
					rotationControlled = true;
				}
				else 
				{
					rotationControlled = false;
				}
			}
			else
			{
				if (eventMouse.ButtonMask == (MouseButtonMask.Left | MouseButtonMask.Right))
				{
					if (rotationControlled)
					{
						if (inventory.Contains("rock", 1))
						{
							inventory.RemoveItem("rock");
							if (PlayerHUD.inventoryOpen) PlayerHUD.UpdateInventory();
							if (PlayerHUD.craftmenuOpen) PlayerHUD.UpdateCraftMenu();
							ThrowableRock rock = throwableRockScene.Instantiate<ThrowableRock>();
							world.AddChild(rock);
							Vector3 startPos = GlobalPosition;
							startPos.Y = 2;
							startPos.X += Mathf.Sin(playerModel.Rotation.Y + 0.7853981633974483f);
							startPos.Z += Mathf.Cos(playerModel.Rotation.Y + 0.7853981633974483f);
							rock.GlobalPosition = startPos;
							Vector3 velocity = new Vector3(0, 2, 0);
							velocity.X = Mathf.Sin(playerModel.Rotation.Y + 0.7853981633974483f) * 5;
							velocity.Z = Mathf.Cos(playerModel.Rotation.Y + 0.7853981633974483f) * 5;
							rock.LinearVelocity = velocity;
						}
	
					}
				}
				rotationControlled = false;
			}
		}
    }

	private Dictionary ScreenToWorld(Vector2 screenPos, uint collisionMask = 1)
	{
		var spaceState = GetWorld3D().DirectSpaceState;
		var origin = Camera.ProjectRayOrigin(screenPos);
		var end = origin + Camera.ProjectRayNormal(screenPos) * 10000;
		var query = PhysicsRayQueryParameters3D.Create(origin, end);
		query.CollisionMask = collisionMask;
		query.CollideWithAreas = true;
		query.CollideWithBodies = true;
		return spaceState.IntersectRay(query);
	}

	private void InteractionMenu(Node node, Vector2 screenPos)
	{
		if (node is PickableObject pickableObject)
		{
			if (GlobalPosition.DistanceTo(pickableObject.GlobalPosition) <= maxPickupDistance)
			{
				EmitSignal(SignalName.WantsPickup, pickableObject, screenPos);
			}
		}
		else if (node is Campfire campfire)
		{
			if (GlobalPosition.DistanceTo(campfire.GlobalPosition) <= maxPickupDistance)
			{
				if (campfire.started && campfire.alive)
				{
					PlayerHUD.campfireCookingContextMenu.Update();
					PlayerHUD.campfireCookingContextMenu.Position = screenPos;
					PlayerHUD.campfireCookingContextMenu.Visible = true;
				}
				else if (campfire.alive)
				{
					EmitSignal(SignalName.WantsFireStart, campfire, screenPos);
				}
			}
		}
		else if (node is Animal animal)
		{
			if (GlobalPosition.DistanceTo(animal.GlobalPosition) <= maxPickupDistance)
			{
				PlayerHUD.cutAnimalContextMenu.Position = screenPos;
				PlayerHUD.cutAnimalContextMenu.Visible = true;
				PlayerHUD.cutAnimalContextMenu.animal = animal;
			}
		}
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

	public void OnWorldTimeUpdate(float newWorldTime)
	{
		worldTimeDelta += Mathf.Abs(worldTime - newWorldTime);
		if (worldTimeDelta >= minuteScale)
		{
			UpdateConditions(worldTimeDelta / minuteScale);
			worldTimeDelta = 0;
		}
		worldTime = newWorldTime;
		debugOverlay.Call("UpdateWorldTime", newWorldTime);
	}

	public void OnManualTimeChange(bool state)
	{
		EmitSignal(SignalName.DebugManualTime, state);
	}

	public void OnManualNewTime(float value)
	{
		EmitSignal(SignalName.DebugManualNewTime, value);
	}

	public void UpdateHUDInfos()
	{
		PlayerHUD.tempStatus.SetValue(internalTemp);
		PlayerHUD.fatigueStatus.SetValue(fatigue);
		PlayerHUD.thirstStatus.SetValue(thirst);
		PlayerHUD.hungerStatus.SetValue(hunger);
		PlayerHUD.healthbar.SetValue(health);
		PlayerHUD.staminaStatus.SetValue(stamina);
		PlayerHUD.SetDayTime(!world.night);
	}

	public void UpdateConditions(float delta)
	{
		//called every ~1 minute of game world time
		if (godMode) return;
		if (sprinting && moving)
		{
			stamina = Mathf.Clamp(stamina - staminaMax / 100 * 2.8f, 0, staminaMax);
			if (stamina == 0) staminaDrained = true;
		}
		else
		{
			stamina = Mathf.Clamp(stamina + staminaMax / 100 * 1.2f, 0, staminaMax);
			if (stamina / staminaMax > 0.2) staminaDrained = false;
		}

		if (campfireBonus)
		{
			internalTemp = Mathf.Clamp(internalTemp + internalTempMax / 100f * 0.5f * delta, 0, internalTempMax);
		}
		else
		{
			internalTemp = Mathf.Clamp(internalTemp - internalTempMax / 100f * (world.night ? 0.4f : 0.2f) * delta, 0, internalTempMax);
		}
		//fatigue = Mathf.Clamp(fatigue - fatigueMax / 100f * 0.07f * delta * (sprinting && moving ? 2f : 1f), 0, fatigueMax);
		thirst = Mathf.Clamp(thirst - thirstMax / 100f * 0.09f * delta * (sprinting && moving ? 2f : 1f), 0, thirstMax);
		hunger = Mathf.Clamp(hunger - hungerMax / 100f * 0.06f * delta * (sprinting && moving ? 2f : 1f), 0, hungerMax);

		health = Mathf.Clamp(health - healthMax / 100f * (internalTemp == 0 ? 0.3333f : 0) * delta, 0, healthMax);
		health = Mathf.Clamp(health - healthMax / 100f * (fatigue == 0 ? 0.01666f : 0) * delta, 0, healthMax);
		health = Mathf.Clamp(health - healthMax / 100f * (thirst == 0 ? 0.03333f : 0) * delta, 0, healthMax);
		health = Mathf.Clamp(health - healthMax / 100f * (hunger == 0 ? 0.01666f : 0) * delta, 0, healthMax);

		if (health == 0) Die();
	}

	public void OnPickup(PickableObject pickableObject)
	{
		if (GlobalPosition.DistanceTo(pickableObject.GlobalPosition) <= maxPickupDistance)
		{
			pickupTarget = pickableObject;
			movementBlock = true;
			animationTree.Set("parameters/pickupShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
		}
	}
	
	public void OnAnimFinish(string animationName)
	{
		if (animationName == "pickup")
		{
			movementBlock = false;
			uint itemCount = inventory.AddItem(pickupTarget);
			Dictionary item = (Dictionary)inventory.globalItems[pickupTarget.item_name];
			string name = (string)item["name"];
			pickupTarget.QueueFree();
			if (PlayerHUD.inventoryOpen) PlayerHUD.UpdateInventory();
			if (PlayerHUD.craftmenuOpen) PlayerHUD.UpdateCraftMenu();
			PlayerHUD.Popup($"Подобрано \"{name}\" ({itemCount})", false);
			PlaySound(itemSound);
		}
	}

	public void OnDropItem(string realName)
	{
		inventory.RemoveItem(realName);
		PlayerHUD.UpdateInventory();
		PackedScene itemScene = inventory.globalItems[realName].As<Dictionary>()["scene"].As<PackedScene>();
		if (itemScene == null) return;
		PickableObject item = itemScene.Instantiate<PickableObject>();
		item.Position = Position with {Y = 0};
		item.Rotation = playerModel.Rotation with {X = 0, Z = 0};
		world.AddChild(item);
		PlaySound(itemSound);
	}

	public void Die()
	{
		movementBlock = true;
		DisconnectThings();
		worldCmd.RequestStopWorld();
		deathScreen dscreen = deathScreenScene.Instantiate<deathScreen>();
		AddChild(dscreen);
		dscreen.Start();
		animationTree.Set("parameters/dieBlend/blend_amount", 1);
		PlayerHUD.Visible = false;
	}

	public void OnCraft(string craftName)
	{
		Dictionary craft = playerCmd.craftingSystem.Recipes[craftName].As<Dictionary>();
		int type = craft["type"].As<int>();
		Dictionary taked = craft["taked"].As<Dictionary>();
		foreach (string key in taked.Keys)
		{
			inventory.RemoveItem(key, taked[key].As<uint>());
		}
		if (type == (int)CraftingSystem.CraftType.item)
		{
			inventory.AddItem(craft["result"].As<string>(), craft["count"].As<uint>());
		}
		else if (type == (int)CraftingSystem.CraftType.worldObject)
		{
			PlayerHUD.SwitchCraftmenu();
			positionPicker.Enable(playerModel, craft["scene"].As<PackedScene>());
		}
		PlayerHUD.UpdateCraftMenu();
	}

	public void OnCampfireSwitch(bool entered)
	{
		campfireBonus = entered;
	}

	public void OnFireStart(Campfire campfire)
	{
		if (inventory.inventoryItems.ContainsKey("knife"))
		{
			if (inventory.inventoryItems.ContainsKey("flint"))
			{
				if (inventory.inventoryItems["flint"].As<Dictionary>()["count"].As<uint>() > 0)
				{
					worldCmd.RequestSkipWorldTime(0.01f);
					PlaySound(flintSound);
					if (random.RandfRange(0, 1) < 0.1f)
					{
						campfire.Burn();
					}
					if (random.RandfRange(0, 1) < 0.1f)
					{
						inventory.RemoveItem("flint");
						PlayerHUD.Popup("Кремень сломался", true);
					}
				}
			}
		}
	}

	public void OnBoilWater()
	{
		worldCmd.RequestSkipWorldTime(0.005f);
		PlaySound(boilSound);
		inventory.AddItem("water", 1);
		PlayerHUD.Popup("Полчено \"Вода\"", true);
	}

	public void OnCookFood()
	{
		inventory.RemoveItem("meat");
		PlayerHUD.UpdateInventory();
		inventory.AddItem("cookedmeat", 1);
		PlayerHUD.Popup("Полчено \"Приготовленное мясо\"", true);
	}

	public void OnDrink(string realName)
	{
		inventory.RemoveItem(realName);
		PlayerHUD.UpdateInventory();
		PlaySound(drinkingSound);
		thirst += thirstMax / 100 * 15;
	}

	public void OnEat(string realName)
	{
		inventory.RemoveItem(realName);
		PlayerHUD.UpdateInventory();
		PlaySound(eatingSound);
		hunger += hungerMax / 100 * 30;
	}

	public void OnCutAnimal(Animal animal)
	{
		animal.QueueFree();
		inventory.AddItem("meat", 1);
		PlayerHUD.UpdateInventory();
		PlaySound(cuttingSound);
		PlayerHUD.Popup("Получено \"Мясо\"", true);
	}
}
