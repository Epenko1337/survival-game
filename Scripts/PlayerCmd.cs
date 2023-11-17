using Godot;
using System;

public partial class PlayerCmd : Node
{
    [Signal]
	public delegate void DropItemEventHandler(string realName);

    [Signal]
    public delegate void CraftItemEventHandler(string craftName);

    [Signal]
    public delegate void CampfireSwitchEventHandler(bool entered);

    [Signal]
    public delegate void DrinkEventHandler(string realName);

    [Signal]
    public delegate void EatEventHandler(string realName);

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

    public void RequestCampfireEnter(bool entered)
    {
        EmitSignal(SignalName.CampfireSwitch, entered);
    }

    public void RequestDrink(string realName)
    {
        EmitSignal(SignalName.Drink, realName);
    }

    public void RequestEat(string realName)
    {
        EmitSignal(SignalName.Eat, realName);
    }
}