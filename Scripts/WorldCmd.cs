using Godot;
using Godot.Collections;
using System;

public partial class WorldCmd : Node
{
    [Signal]
	public delegate void WorldTimeUpdateEventHandler(float WorldTime) ;

    [Signal]
    public delegate void StopWorldEventHandler();

    [Signal]
    public delegate void DestroyWorldEventHandler();

    [Signal]
    public delegate void SkipWorldTimeEventHandler(float value);

    [Signal]
    public delegate void AddWorldChildEventHandler(Node node);

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

    public void RequestSkipWorldTime(float value)
    {
        EmitSignal(SignalName.SkipWorldTime, value);
    }

    public void RequestAddChild(Node node)
    {
        EmitSignal(SignalName.AddWorldChild, node);
    }
}