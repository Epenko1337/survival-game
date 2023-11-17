using Godot;
using Godot.Collections;
using System;

public partial class CraftItem : PanelContainer
{
	public string realName;
	Label nameLabel;
	Label requirements;
	PlayerCmd playerCmd;
	Button button;
	bool notEnoughItems = false;
	Color notEnoughColor;
	public override void _Ready()
	{
		nameLabel = GetNode<Label>("VBoxContainer/NameLabel");
		requirements = GetNode<Label>("VBoxContainer/Requirements");
		button = GetNode<Button>("Button");
		playerCmd = GetNode<PlayerCmd>("/root/PlayerCmd");
		button.Pressed += () => playerCmd.RequestCraftItem(realName);
		button.Visible = false;
		notEnoughColor = new Color(0.8f, 0.8f, 0.8f);
	}

	public void Update()
	{
		Dictionary recipe = playerCmd.craftingSystem.Recipes[realName].As<Dictionary>();
		if (recipe["type"].As<int>() == (int)CraftingSystem.CraftType.worldObject)
		{
			nameLabel.Text = "  " + recipe["result"].As<string>();
		}
		else
		{
			Dictionary resultItem = playerCmd.globalInventory.globalItems[recipe["result"].As<string>()].As<Dictionary>();
			nameLabel.Text = "  " + resultItem["name"].As<string>();
		}
		requirements.Text = "  Требуется:";
		Dictionary needed = recipe["needed"].As<Dictionary>();
		foreach (string key in needed.Keys)
		{
			Dictionary neededItem = playerCmd.globalInventory.globalItems[key].As<Dictionary>();
			requirements.Text += $"\n  -{neededItem["name"].As<string>()} ({needed[key]})";
			if (!playerCmd.globalInventory.Contains(key, needed[key].As<uint>())) notEnoughItems = true;
		}
		if (!notEnoughItems) button.Visible = true;
		else
		{
			nameLabel.AddThemeColorOverride("font_color", notEnoughColor);
			requirements.AddThemeColorOverride("font_color", notEnoughColor);
		}
	}
}
