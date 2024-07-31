using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour,IPointerUpHandler
{
    public Item item;
    public Image itemIcon;
    public int slotnum;

    public void UpdateSlotUI()
    {
        itemIcon.sprite = item.itemImage;
        itemIcon.gameObject.SetActive(true);
    }

    public void RemoveSlot()
    {
        item = null;
        itemIcon.gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            bool isUse=item.Use(player);
            if(isUse)
            {
                Inventory.instance.RemoveItem(slotnum);
            }

        }
    }
}
