using Godot;
using System;

public partial class ObjectFade : Area3D
{
	bool playerInside = false;
	bool cameraInside = false;
	StandardMaterial3D[] alphaMaterials;
	StandardMaterial3D[] origMaterials;
	int surfaceCount;
	float alpha = 1f;
	Node3D thisParent;
	MeshInstance3D meshInstance;
    public override void _Ready()
    {
		meshInstance = GetParent().GetChild<MeshInstance3D>(0);
		surfaceCount = meshInstance.Mesh.GetSurfaceCount();
		alphaMaterials = new StandardMaterial3D[surfaceCount];
		origMaterials = new StandardMaterial3D[surfaceCount];
		for (int i = 0; i < surfaceCount; i++)
		{
			origMaterials[i] = (StandardMaterial3D)meshInstance.Mesh.SurfaceGetMaterial(i);
			alphaMaterials[i] = (StandardMaterial3D)meshInstance.Mesh.SurfaceGetMaterial(i).Duplicate(false);
			alphaMaterials[i].Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
			alphaMaterials[i].CullMode = BaseMaterial3D.CullModeEnum.Back;
		}
		this.AreaEntered += (e) => OnAreaEnter(e);
		this.AreaExited += (e) => OnAreaExit(e);
		thisParent = GetParent<Node3D>();
    }

    public override void _PhysicsProcess(double delta)
    {
		meshInstance.Visible = !(cameraInside && !playerInside);
		alpha = meshInstance.Visible ? alpha : 0;
        if (playerInside)
		{
			ApplyAlpha(Mathf.Lerp(alpha, 0.2f, 8f*(float)delta));
		}
		else if (alpha != 1f)
		{
			ApplyAlpha(Mathf.Lerp(alpha, 1f, 8f*(float)delta));
		}
		else if (!playerInside && alpha == 1f)
		{
			SwitchMaterials(false);
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
			SwitchMaterials(true);
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

	private void ApplyAlpha(float newAlpha)
	{
		alpha = newAlpha;
		for (int i = 0; i < surfaceCount; i++)
		{
			StandardMaterial3D material = alphaMaterials[i];
			Color newColor = material.AlbedoColor;
			newColor.A = alpha;
			material.AlbedoColor = newColor;
		}
	}

	private void SwitchMaterials(bool alpha)
	{
		for (int i = 0; i < surfaceCount; i++)
		{
			meshInstance.SetSurfaceOverrideMaterial(i, alpha ? alphaMaterials[i] : origMaterials[i]);
		}
	}
}
