using Godot;
using System;

public partial class ObjectFade : Area3D
{
	bool inside = false;
	StandardMaterial3D[] materials;
	int surfaceCount;
	float alpha = 1f;
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
			materials[i].DistanceFadeMode = BaseMaterial3D.DistanceFadeModeEnum.PixelAlpha;
			materials[i].DistanceFadeMinDistance = 3f;
			materials[i].DistanceFadeMaxDistance = 20f;
			mesh.SetSurfaceOverrideMaterial(i, materials[i]);
		}
		this.BodyEntered += (e) => OnBodyEnter((PhysicsBody3D)e);
		this.BodyExited += (e) => OnBodyExit((PhysicsBody3D)e);
    }

    public override void _Process(double delta)
    {
        if (inside)
		{
			ApplyAlpha(Mathf.Lerp(alpha, 0.3f, 8f*(float)delta));
		}
		else if (alpha != 1f)
		{
			ApplyAlpha(Mathf.Lerp(alpha, 1f, 8f*(float)delta));
		}
    }

    private void OnBodyEnter(PhysicsBody3D body)
	{
		if (body is CharacterBody3D)
		{
			inside = true;
		}
	}

	private void OnBodyExit(PhysicsBody3D body)
	{
		if (body is CharacterBody3D)
		{
			inside = false;
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
