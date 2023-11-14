using Godot;
using System;

public partial class ContextMenu : Control
{
    public string title;
    public bool mouseIn = false;

    public override void _Ready()
    {
        this.MouseExited += () => OnMouseExit();
        this.MouseEntered += () => OnMosueEnter();
    }

    public void Bye()
    {
        this.Visible = false;
    }

    public void OnMouseExit()
    {
        mouseIn = false;
    }

    public void OnMosueEnter()
    {
        mouseIn = true;
    }

    public bool InControl(Vector2 mousePos)
    {
        Vector2 globalPos = this.GlobalPosition;
        float minX = globalPos.X;
        float minY = globalPos.Y;
        float maxX = minX + this.Size.X;
        float maxY = minY + this.Size.Y;
        return mousePos.X > minX && mousePos.Y > minY && mousePos.X < maxX && mousePos.Y < maxY;
    }

    public override void _Input(InputEvent @event)
    {
        if (!Visible) return;
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.ButtonMask == MouseButtonMask.Left && eventMouseButton.IsPressed()) 
            {
                if (!InControl(eventMouseButton.Position)) Bye();
            }
        }
    }
}