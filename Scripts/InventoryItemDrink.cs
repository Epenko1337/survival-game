using Godot;
using System;

public partial class InventoryItemDrink : InventoryItem
{
	Button drinkButton;
	public override void _Ready()
	{
		base._Ready();
		drinkButton = GetNode<Button>("HBoxContainer/DrinkButton");
		drinkButton.Pressed += () => playerCmd.RequestDrink(realName);
	}
}
