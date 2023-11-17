using Godot;
using System;

public partial class CutAnimalContextMenu : ContextMenu
{
    VBoxContainer container;
    Label titleLabel;
    public Animal animal;

	[Signal]
	public delegate void CutAnimalEventHandler(Animal animal);

    public override void _Ready()
    {
        base._Ready();
		GetNode<Button>("VBoxContainer/Button").Pressed += () => EmitNClose(SignalName.CutAnimal, animal);
    }
}
