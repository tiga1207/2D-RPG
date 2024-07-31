using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ItemEffect/Comsumable/Health")]
public class ItemHealingEffect : ItemEffect 
{
    public int healingAmount =0;
    public override bool ExecuteRole(Player player)
    {

        if (player == null)
        {
            return false;
        }

        player.HpPotion(healingAmount);//hp회복
        return true;
    }
}
