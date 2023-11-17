using Godot;
using System;

public partial class FirestartContextMenu : ContextMenu
{
	[Signal]
	public delegate void StartFireEventHandler(Campfire argcampfire);
	public Campfire campfire;

	public void GoStartFire()
	{
		EmitSignal(SignalName.StartFire, campfire);
		Bye();
	}
}
