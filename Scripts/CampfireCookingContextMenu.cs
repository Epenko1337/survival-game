using Godot;
using System;

public partial class CampfireCookingContextMenu : ContextMenu
{
	PlayerCmd playerCmd;
	[Signal]
	public delegate void BoilWaterEventHandler();
	[Signal]
	public delegate void CookFoodEventHandler();

	Button cookbtn;
	Button boilbtn;
	public override void _Ready()
	{
		playerCmd = GetNode<PlayerCmd>("/root/PlayerCmd");
		cookbtn = GetNode<Button>("VBoxContainer/CookBtn");
		boilbtn = GetNode<Button>("VBoxContainer/BoilBtn");
		cookbtn.Disabled = !playerCmd.globalInventory.Contains("meat");
		cookbtn.Pressed += () => EmitNClose(SignalName.CookFood);
		boilbtn.Pressed += () => EmitNClose(SignalName.BoilWater);
	}

	public void Update()
	{
		cookbtn.Disabled = !playerCmd.globalInventory.Contains("meat");
	}
}
