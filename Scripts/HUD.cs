using Godot;
using Godot.Collections;
using System;
using System.Linq;

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
	public VBoxContainer inventoryVbox;
	public PackedScene inventoryItemScene;
	public bool inventoryOpen = false;
	public Control inventoryControl;
	public Label weightLabel;
	public bool craftmenuOpen = false;
	public Control craftmenuControl;
	public VBoxContainer craftmenuVbox;
	public PackedScene craftItemScene;
	public PlayerCmd playerCmd;
	public FirestartContextMenu firestartContextMenu;
	public CampfireCookingContextMenu campfireCookingContextMenu;
	public PackedScene inventoryItemDrinkScene;
	public PackedScene inventoryItemFoodScene;
	public CutAnimalContextMenu cutAnimalContextMenu;
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
		inventoryControl = GetNode<Control>("Inventory");
		inventoryControl.Visible = false;
		inventoryVbox = GetNode<VBoxContainer>("Inventory/ScrollContainer/VBoxContainer");
		inventoryItemScene = GD.Load<PackedScene>("res://Scenes/InventoryItem.tscn");
		weightLabel = GetNode<Label>("Inventory/WeightLabel");
		craftmenuControl = GetNode<Control>("CraftMenu");
		craftmenuControl.Visible = false;
		craftmenuVbox = GetNode<VBoxContainer>("CraftMenu/ScrollContainer/VBoxContainer");
		craftItemScene = GD.Load<PackedScene>("res://Scenes/CraftItem.tscn");
		playerCmd = GetNode<PlayerCmd>("/root/PlayerCmd");
		firestartContextMenu = GetNode<FirestartContextMenu>("FirestartContextMenu");
		campfireCookingContextMenu = GetNode<CampfireCookingContextMenu>("CampfireCooking");
		inventoryItemDrinkScene = GD.Load<PackedScene>("res://Scenes/InventoryItemDrink.tscn");
		inventoryItemFoodScene = GD.Load<PackedScene>("res://Scenes/InventoryItemFood.tscn");
		cutAnimalContextMenu = GetNode<CutAnimalContextMenu>("cutAnimalContextMenu");
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
		Dictionary item = (Dictionary)playerInstance.inventory.globalItems[pickableObject.item_name];
		pickupContextMenu.title = (string) item["name"];
		pickupContextMenu.pickableObject = pickableObject;
		pickupContextMenu.Visible = true;
		pickupContextMenu.Update();
	}

	public void ShowFireStartContext(Campfire campfire, Vector2 screenPos)
	{
		firestartContextMenu.Position = screenPos;
		firestartContextMenu.campfire = campfire;
		firestartContextMenu.Visible = true;
	}

	public void Popup(string text, bool sound)
	{
		TextPopup popup = popupScene.Instantiate<TextPopup>();
		AddChild(popup);
		popup.Start(text, sound);
	}

	public void SwitchInventory()
	{
		craftmenuOpen = false;
		craftmenuControl.Visible = false;
		inventoryOpen = !inventoryOpen;
		if (inventoryOpen) UpdateInventory();
		inventoryControl.Visible = !inventoryControl.Visible;
	}

	public void SwitchCraftmenu()
	{
		inventoryOpen = false;
		inventoryControl.Visible = false;
		craftmenuOpen = !craftmenuOpen;
		craftmenuControl.Visible = !craftmenuControl.Visible;
		if (craftmenuControl.Visible) UpdateCraftMenu();
	}

	public void UpdateInventory()
	{
		weightLabel.Text = $"Занято {Mathf.Round(playerInstance.inventory.GetFullWeight())}/{playerInstance.maxCapacity} кг";
		foreach (Node item in inventoryVbox.GetChildren())
		{
			item.QueueFree();
		}

		foreach (string key in playerInstance.inventory.inventoryItems.Keys)
		{
			Dictionary inventoryItem = (Dictionary)playerInstance.inventory.inventoryItems[key];
			uint itemCount = (uint)inventoryItem["count"];
			if (itemCount == 0) continue;
			InventoryItem item = null;
			switch (playerCmd.globalInventory.globalItems[key].As<Dictionary>()["type"].As<int>())
			{
				case (int)Inventory.ItemType.material:
					item = inventoryItemScene.Instantiate<InventoryItem>();
					break;
				case (int)Inventory.ItemType.drink:
					item = inventoryItemDrinkScene.Instantiate<InventoryItemDrink>();
					break;
				case (int)Inventory.ItemType.food:
					item = inventoryItemFoodScene.Instantiate<InventoryItemFood>();
					break;
				default:
					item = inventoryItemScene.Instantiate<InventoryItem>();
					break;
			}
			item.realName = key;
			item.name = (string)inventoryItem["name"];
			item.count = itemCount;
			item.weight = (float)inventoryItem["weight"];
			inventoryVbox.AddChild(item);
			item.Update();
		}
	}

	public void UpdateCraftMenu()
	{
		foreach (Node item in craftmenuVbox.GetChildren())
		{
			item.QueueFree();
		}

		foreach (string key in playerCmd.craftingSystem.Recipes.Keys)
		{
			CraftItem item = craftItemScene.Instantiate<CraftItem>();
			item.realName = key;
			craftmenuVbox.AddChild(item);
			item.Update();
		}
	}
}
