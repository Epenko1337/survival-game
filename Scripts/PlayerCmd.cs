using Godot;
using System;

public partial class PlayerCmd : Node
{
    [Signal]
	public delegate void DropItemEventHandler(string realName);

    public Inventory globalInventory = new Inventory();

    public void RequestDropItem(string realName)
    {
        EmitSignal(SignalName.DropItem, realName);
    }

    public void Reset()
    {
        DisconnectSignals(SignalName.DropItem);
    }

    public void DisconnectSignals(StringName signalName)
    {
        foreach(var dict in this.GetSignalConnectionList(signalName))
        {
            this.Disconnect(signalName, (Callable)dict["callable"]);
        }
    }
}