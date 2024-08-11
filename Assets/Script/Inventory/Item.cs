using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Equipment,
    Consumables,
    Etc
}

[System.Serializable]
public class Item
{
    public int itemID;
    public ItemType itemType;
    public string itemName;
    public Sprite itemImage;
    public List<ItemEffect> effects;

    public bool Use(Player player)
    {
        bool isUsed = false;
        
        foreach (ItemEffect effect in effects)
        {
            isUsed = effect.ExecuteRole(player);
        }

        return isUsed;
    }   
}

