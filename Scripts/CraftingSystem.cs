using Godot;
using Godot.Collections;
using System;

public class CraftingSystem
{
    public enum CraftType
	{
		item,
		worldObject,
	}
    public Dictionary Recipes = new Dictionary();

    public CraftingSystem()
    {
        RecipeConstructor("campfire", "Костер", GD.Load<PackedScene>("res://Scenes/Items/campfire.tscn"), 
        new Dictionary
        {
            {"rock", 4},
            {"stick", 4},
        },
        new Dictionary
        {
            {"rock", 4},
            {"stick", 4},   
        });
    }

    public void RecipeConstructor(string craftName, string resultName, uint count, Dictionary neededItems, Dictionary takedItems, float craftTime)
    {
        Recipes.Add(craftName, new Dictionary
        {
            {"result", resultName},
            {"count", count},
            {"needed", neededItems},
            {"taked", takedItems},
            {"time", craftTime},
            {"type", (int)CraftType.item},
        });
    }

    public void RecipeConstructor(string craftName, string resultName, PackedScene resultScene, Dictionary neededItems, Dictionary takedItems)
    {
        Recipes.Add(craftName, new Dictionary
        {
            {"result", resultName},
            {"scene", resultScene},
            {"needed", neededItems},
            {"taked", takedItems},
            {"type", (int)CraftType.worldObject},
        });
    }
}