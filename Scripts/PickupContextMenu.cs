using Godot;
using System;

public partial class PickupContextMenu : ContextMenu
{
    VBoxContainer container;
    Label titleLabel;
    public PickableObject pickableObject;

    [Signal]
    public delegate void PickupEventHandler(PickableObject pickableObject);

    public override void _Ready()
    {
        base._Ready();
        container = GetNode<VBoxContainer>("VBoxContainer");
        titleLabel = container.GetNode<Label>("TitleLabel");
    }

    public void Update()
    {
        titleLabel.Text = title;
    }

    public void GoPickup()
    {
        EmitSignal(SignalName.Pickup, pickableObject);
        Bye();
    }
}