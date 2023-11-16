using Godot;
using Godot.Collections;
using System;

public class CraftingSystem
{
    public enum CraftType : int
	{
		item,
		worldObject,
	}
    public Dictionary Recipes = new Dictionary();

    public CraftingSystem()
    {
        RecipeConstructor("bigrock", "bigrock", 
        new Dictionary
        {
            {"rock", 2}
        },
        new Dictionary
        {
            {"rock", 2}
        },
        5, (int)CraftType.item);
    }

    public void RecipeConstructor(string craftName, string resultName, Dictionary neededItems, Dictionary takedItems, float craftTime, int type)
    {
        Recipes.Add(craftName, new Dictionary
        {
            {"result", resultName},
            {"needed", neededItems},
            {"taked", takedItems},
            {"time", craftTime},
            {"type", type},
        });
    }
}