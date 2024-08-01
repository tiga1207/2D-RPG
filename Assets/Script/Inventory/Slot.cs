using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerUpHandler
{
    public Item item;
    public Image itemIcon;
    public int slotnum;

    public void UpdateSlotUI()
    {
        if(item != null)
        {
            itemIcon.sprite = item.itemImage;
            itemIcon.gameObject.SetActive(true);
        }
        else
        {
            itemIcon.gameObject.SetActive(false);
        }
    }

    public void RemoveSlot()
    {
        item = null;
        itemIcon.gameObject.SetActive(false);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if(item !=null)
        {

            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                bool isUse = item.Use(player);
                if (isUse)
                {
                    Inventory inven = player.GetComponent<Inventory>();
                    if (inven != null)
                    {
                        inven.RemoveItem(slotnum);
                    }
                }
                else{
                    Debug.Log("클릭 안됨");
                }
            }
        }
    }
}
