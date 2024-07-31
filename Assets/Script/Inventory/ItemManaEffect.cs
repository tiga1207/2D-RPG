using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ItemEffect/Comsumable/Mana")]
public class ItemManaEffect : ItemEffect 
{
    public int ManaAmount =0;
    public override bool ExecuteRole(Player player)
    {

        if (player == null)
        {
            return false;
        }

        player.MpPotion(ManaAmount);//mp회복
        return true;
    }
}
