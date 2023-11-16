using Godot;
using System;

public partial class InventoryItem : PanelContainer
{
	public string realName;
	public string name;
	public uint count;
	public float weight;
	Label valueLabel;
	Label nameLabel;
	Button dropButton;
	PlayerCmd playerCmd;
	public override void _Ready()
	{
		valueLabel = GetNode<Label>("HBoxContainer/ValueLabel");
		nameLabel = GetNode<Label>("NameLabel");
		dropButton = GetNode<Button>("HBoxContainer/DropButton");
		playerCmd = GetNode<PlayerCmd>("/root/PlayerCmd");
		dropButton.Pressed += () => playerCmd.RequestDropItem(realName);
	}

	public void Update()
	{
		valueLabel.Text = count.ToString() + "  ";
		nameLabel.Text = "  " + name;
		this.TooltipText = $"Вес одного предмета: {weight}\nВес всех предметов: {weight*count}";
	}
}
