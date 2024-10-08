using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Weapon,
    Consumables,
    Etc
}

[System.Serializable]
public class Item
{
    public string itemID;
    public ItemType itemType;
    public string itemName;
    public Sprite itemImage;
    public List<ItemEffect> effects;
    public bool canOverlap;

    public bool Use(Player player)
    {
        bool isUsed = false;
        // itemCnt--;
        foreach (ItemEffect effect in effects)
        {
            isUsed = effect.ExecuteRole(player);
        }

        return isUsed;
    }
}


