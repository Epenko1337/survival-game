using Godot;
using System;

public partial class ObjectFade : Area3D
{
	bool playerInside = false;
	bool cameraInside = false;
	StandardMaterial3D[] materials;
	int surfaceCount;
	float alpha = 1f;
	Node3D thisParent;
    public override void _Ready()
    {
		MeshInstance3D mesh = GetParent().GetChild<MeshInstance3D>(0);
		surfaceCount = mesh.Mesh.GetSurfaceCount();
		materials = new StandardMaterial3D[surfaceCount];
		for (int i = 0; i < surfaceCount; i++)
		{
			materials[i] = (StandardMaterial3D)mesh.Mesh.SurfaceGetMaterial(i).Duplicate(false);
			materials[i].Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
			materials[i].CullMode = BaseMaterial3D.CullModeEnum.Back;
			mesh.SetSurfaceOverrideMaterial(i, materials[i]);
		}
		this.AreaEntered += (e) => OnAreaEnter(e);
		this.AreaExited += (e) => OnAreaExit(e);
		thisParent = GetParent<Node3D>();
    }

    public override void _PhysicsProcess(double delta)
    {
		thisParent.Visible = !(cameraInside && !playerInside);
		alpha = thisParent.Visible ? alpha : 0;
        if (playerInside)
		{
			ApplyAlpha(Mathf.Lerp(alpha, 0.2f, 8f*(float)delta));
		}
		else if (alpha != 1f)
		{
			ApplyAlpha(Mathf.Lerp(alpha, 1f, 8f*(float)delta));
		}
    }

	private void OnAreaEnter(Area3D area)
	{
		Node parent = area.GetParent();
		if (parent is Camera3D)
		{
			cameraInside = true;
		}
		else if (parent is CharacterBody3D)
		{
			playerInside = true;
		}
	}

	private void OnAreaExit(Area3D area)
	{
		Node parent = area.GetParent();
		if (parent is Camera3D)
		{
			cameraInside = false;
		}
		else if (parent is CharacterBody3D)
		{
			playerInside = false;
		}
	}

    private void OnBodyEnter(PhysicsBody3D body)
	{
		if (body.GetParent().GetParent() is CharacterBody3D)
		{
			//inside = true;
		}
	}

	private void OnBodyExit(PhysicsBody3D body)
	{
		if (body is CharacterBody3D)
		{
			//inside = false;
		}
	}

	private void ApplyAlpha(float newAlpha)
	{
		alpha = newAlpha;
		for (int i = 0; i < surfaceCount; i++)
		{
			StandardMaterial3D material = materials[i];
			Color newColor = material.AlbedoColor;
			newColor.A = alpha;
			material.AlbedoColor = newColor;
		}
	}
}
