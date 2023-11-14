using Godot;
using System;
using Godot.Collections;

public class Inventory
{
    public Dictionary globalItems = new Dictionary();
    public Dictionary inventoryItems = new Dictionary();
    public Inventory()
    {
        globalItems.Add("stick", GlobalItemConstructor("Палка", 0.1f));
    }

    public Dictionary GlobalItemConstructor(string name, float weight)
    {
        return new Dictionary
        {
            {"name", name},
            {"weight", weight},
        };
    }

    public Dictionary ItemConstructor(string name, float weight, uint count = 1)
    {
        return new Dictionary
        {
            {"name", name},
            {"weight", weight},
            {"count", count},
        };
    }

    public uint AddItem(PickableObject item)
    {
        Dictionary globalItem = globalItems[item.item_name].As<Dictionary>();
        return AddItem(item.item_name, globalItem["name"].As<string>(), globalItem["weight"].As<float>(), 1);
    }

    public uint AddItem(string realName, string name, float weight, uint count)
    {
        if (inventoryItems.ContainsKey(realName))
        {
            Dictionary item = inventoryItems[realName].As<Dictionary>();
            uint newCount = (uint)item["count"] + count;
            item["count"] = newCount;
            return newCount;
        }
        else
        {
            inventoryItems.Add(realName, ItemConstructor(name, weight, count));
        }
        return 1;
    }

    public string GetItemName(string realName)
    {
        Dictionary item = globalItems[realName].As<Dictionary>();
        return (string)item["name"];
    }
}