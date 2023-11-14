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
	
	public override void _Ready()
	{
		valueLabel = GetNode<Label>("ValueLabel");
		nameLabel = GetNode<Label>("NameLabel");
	}

	public void Update()
	{
		valueLabel.Text = count.ToString() + "  ";
		nameLabel.Text = "  " + name;
	}
}
