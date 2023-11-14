using Godot;
using System;

public partial class HUD : Node2D
{
	public hud_info_item tempStatus;
	public hud_info_item fatigueStatus;
	public hud_info_item thirstStatus;
	public hud_info_item hungerStatus;
	public hud_info_item staminaStatus;
	public healthBar healthbar;
	public Sprite2D sunIcon;
	public Sprite2D moonIcon;
	public PickupContextMenu pickupContextMenu;
	public player playerInstance;
	public PackedScene popupScene;
	public override void _Ready()
	{
		tempStatus = GetNode<hud_info_item>("TempStatus");
		fatigueStatus = GetNode<hud_info_item>("FatigueStatus");
		thirstStatus = GetNode<hud_info_item>("ThirstStatus");
		hungerStatus = GetNode<hud_info_item>("HungerStatus");
		staminaStatus = GetNode<hud_info_item>("StaminaStatus");
		healthbar = GetNode<Node2D>("HealthBar").GetNode<healthBar>("healthBar");
		Node2D dayNightStatus = GetNode<Node2D>("DayNightStatus");
		sunIcon = dayNightStatus.GetNode<Sprite2D>("SunIcon");
		moonIcon = dayNightStatus.GetNode<Sprite2D>("MoonIcon");
		pickupContextMenu = GetNode<PickupContextMenu>("PickupContextMenu");
		playerInstance = GetParent<player>();
		popupScene = GD.Load<PackedScene>("res://Scenes/TextPopup.tscn");
	}

	public override void _Process(double delta)
	{
		staminaStatus.Visible = !(staminaStatus.valueCircle.value == staminaStatus.valueCircle.maxValue);
	}

	public void SetDayTime(bool time)
	{
		sunIcon.Visible = time;
		moonIcon.Visible = !time;
	}

	public void ShowPickupContext(PickableObject pickableObject, Vector2 screenPos)
	{
		pickupContextMenu.Position = screenPos;
		Godot.Collections.Dictionary item = (Godot.Collections.Dictionary)playerInstance.inventory.globalItems[pickableObject.item_name];
		pickupContextMenu.title = (string) item["name"];
		pickupContextMenu.pickableObject = pickableObject;
		pickupContextMenu.Visible = true;
		pickupContextMenu.Update();
	}

	public void Popup(string text)
	{
		TextPopup popup = popupScene.Instantiate<TextPopup>();
		AddChild(popup);
		popup.Start(text);
	}
}
