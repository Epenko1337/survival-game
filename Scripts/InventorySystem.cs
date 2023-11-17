using Godot;
using System;
using Godot.Collections;

public class Inventory
{
    public Dictionary globalItems = new Dictionary();
    public Dictionary inventoryItems = new Dictionary();
    public float maxCapacity = 15f;

    public enum ItemType
    {
        material,
        food,
        drink,
        weapon,
    }
    public Inventory()
    {
        globalItems.Add("stick", GlobalItemConstructor("Ветка", 0.25f, GD.Load<PackedScene>("res://Scenes/Items/stick.tscn"), ItemType.material));
        globalItems.Add("rock", GlobalItemConstructor("Камень", 0.15f, GD.Load<PackedScene>("res://Scenes/Items/rock.tscn"), ItemType.material));
        globalItems.Add("bigrock", GlobalItemConstructor("Большой камень", 0.2f, GD.Load<PackedScene>("res://Scenes/Items/rock.tscn"), ItemType.material));
        globalItems.Add("flint", GlobalItemConstructor("Кремень", 0.15f, GD.Load<PackedScene>("res://Scenes/Items/flint.tscn"), ItemType.material));
        globalItems.Add("knife", GlobalItemConstructor("Нож", 0.3f, GD.Load<PackedScene>("res://Scenes/Items/knife.tscn"), ItemType.weapon));
        globalItems.Add("water", GlobalItemConstructor("Вода", 1f, null, ItemType.drink));
        globalItems.Add("meat", GlobalItemConstructor("Сырое мясо", 0.5f, GD.Load<PackedScene>("res://Scenes/Items/meat.tscn"), ItemType.material));
        globalItems.Add("cookedmeat", GlobalItemConstructor("Приготовленное мясо", 0.4f, GD.Load<PackedScene>("res://Scenes/Items/cookedmeat.tscn"), ItemType.food));
    }

    public Dictionary GlobalItemConstructor(string name, float weight, PackedScene scene, ItemType type)
    {
        return new Dictionary
        {
            {"name", name},
            {"weight", weight},
            {"scene", scene},
            {"type", (int)type}
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
        return AddItem(item.item_name, globalItem["name"].As<string>(), globalItem["weight"].As<float>(), item.count);
    }

    public uint AddItem(string realName, string name, float weight, uint count)
    {
        Dictionary item = globalItems[realName].As<Dictionary>();
        if (GetFullWeight() + item["weight"].As<float>() > maxCapacity) return 0;
        if (inventoryItems.ContainsKey(realName))
        {
            item = inventoryItems[realName].As<Dictionary>();
            uint newCount = (uint)item["count"] + count;
            item["count"] = newCount;
            return newCount;
        }   
        else
        {
            inventoryItems.Add(realName, ItemConstructor(name, weight, count));
            return count;
        }
    }

    public uint AddItem(string realName, uint count)
    {
        Dictionary item = globalItems[realName].As<Dictionary>();
        if (GetFullWeight() + item["weight"].As<float>() > maxCapacity) return 0;
        if (inventoryItems.ContainsKey(realName))
        {
            item = inventoryItems[realName].As<Dictionary>();
            uint newCount = (uint)item["count"] + count;
            item["count"] = newCount;
            return newCount;
        }   
        else
        {
            inventoryItems.Add(realName, ItemConstructor(item["name"].As<string>(), item["weight"].As<float>(), count));
            return count;
        }
    }

    public string GetItemName(string realName)
    {
        Dictionary item = globalItems[realName].As<Dictionary>();
        return (string)item["name"];
    }

    public float GetFullWeight()
    {
        float weight = 0;
        foreach (string key in inventoryItems.Keys)
		{
            Dictionary item = (Dictionary)inventoryItems[key];
            uint count = (uint)item["count"];
            float perWeight = (float)item["weight"];
            weight += count*perWeight;
		}
        return weight;
    }

    public void RemoveItem(string realName, uint count = 1)
    {
        Dictionary item = (Dictionary)inventoryItems[realName];
        uint currCount = (uint)item["count"];
        item["count"] = Mathf.Clamp(currCount - count, 0, currCount);
    }

    public bool Contains(string realName, uint count = 1)
    {
        if (!inventoryItems.ContainsKey(realName)) return false;
        else
        {
            uint currcount = inventoryItems[realName].As<Dictionary>()["count"].As<uint>();
            if (currcount < count) return false;
            return true; 
        }
    }
}