using Godot;
using Godot.Collections;
using System;

public partial class WorldCmd : Node
{
    [Signal]
	public delegate void WorldTimeUpdateEventHandler(float WorldTime);

    [Signal]
    public delegate void StopWorldEventHandler();

    [Signal]
    public delegate void DestroyWorldEventHandler();

    public void RequestWorldTimeUpdate(float newWorldTime)
    {
        EmitSignal(SignalName.WorldTimeUpdate, newWorldTime);
    }

    public void RequestStopWorld()
    {
        EmitSignal(SignalName.StopWorld);
    }

    public void RequestDestroyWorld()
    {
        EmitSignal(SignalName.DestroyWorld);
    }

    public void Reset()
    {
        foreach (Dictionary signalInstance in this.GetIncomingConnections())
        {
            Signal signal = (Signal)signalInstance["signal"];
            signal.Owner.Disconnect(signal.Name, (Callable)signalInstance["callable"]);
        }
    }
}