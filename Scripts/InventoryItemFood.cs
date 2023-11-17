using Godot;
using System;

public partial class InventoryItemFood : InventoryItem
{
	public override void _Ready()
	{
		base._Ready();
		Button eatButton = GetNode<Button>("HBoxContainer/EatButton");
		eatButton.Pressed += () => playerCmd.RequestEat(realName);
	}

}
