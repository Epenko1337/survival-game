using Godot;
using System;

public partial class PlayerCmd : Node
{
    [Signal]
	public delegate void DropItemEventHandler(string realName);

    [Signal]
    public delegate void CraftItemEventHandler(string craftName);

    public Inventory globalInventory = new Inventory();
    public CraftingSystem craftingSystem = new CraftingSystem();

    public void RequestDropItem(string realName)
    {
        EmitSignal(SignalName.DropItem, realName);
    }

    public void RequestCraftItem(string craftName)
    {
        EmitSignal(SignalName.CraftItem, craftName);
    }
}