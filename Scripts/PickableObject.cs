using Godot;
using System;

public partial class PickableObject : InteractableObject
{
    [Export]
    public string item_name;

    [Export]
    public uint count;
}