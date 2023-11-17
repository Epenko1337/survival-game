using Godot;
using System;

public partial class soundsettings : Node2D
{
	HSlider effectSlider;
	HSlider ambientSlider;
	float effectsVolume;
	float ambientVolume;
	public override void _Ready()
	{
		effectsVolume = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Effects"));
		ambientVolume = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Ambience"));
		effectSlider = GetNode<HSlider>("ObjectsHolder/VBoxContainer/Label/EffectSlider");
		ambientSlider = GetNode<HSlider>("ObjectsHolder/VBoxContainer/Label2/AmbentSlider");
		effectSlider.Value = effectsVolume;
		ambientSlider.Value = ambientVolume;
		effectSlider.DragEnded += (e) => SetValue("Effects", (float)effectSlider.Value);
		ambientSlider.DragEnded += (e) => SetValue("Ambience", (float)ambientSlider.Value);
	}
	
	public void GotoMenu()
	{
		GetTree().ChangeSceneToFile("res://Scenes/menu.tscn");
	}

	public void SetValue(StringName busName, float value)
	{
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(busName), value);
	}
}
