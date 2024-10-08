using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Slot : MonoBehaviour, IPointerUpHandler
{
    public InventorySlot inventorySlot;  // InventorySlot 참조
    public Image itemIcon;
    public int slotnum;
    public TextMeshProUGUI itemCntText;

    public void UpdateSlotUI()
    {
        // 슬롯 데이터가 유효한지 확인
        if (inventorySlot != null)
        {
            // 아이템 데이터 가져오기
            Item itemData = ItemDataBase.instance.GetItemByID(inventorySlot.itemID);
            
            if (itemData != null)
            {
                // 아이템 아이콘과 개수 업데이트
                itemIcon.sprite = itemData.itemImage;
                itemIcon.gameObject.SetActive(true);
                itemCntText.text = inventorySlot.ItemCount.ToString(); // 슬롯에서 아이템 개수 가져오기
                itemCntText.gameObject.SetActive(true);
            }
        }
        else
        {
            // 슬롯이 비었으면 UI 숨기기
            itemIcon.gameObject.SetActive(false);
            itemCntText.gameObject.SetActive(false);
        }
    }

    public void RemoveSlot()
    {
        inventorySlot = null;
        itemIcon.gameObject.SetActive(false);
        itemCntText.gameObject.SetActive(false);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (inventorySlot != null)
        {
            Player player = Player.LocalPlayerInstance;
            if (player != null && player.GetComponent<PhotonView>().IsMine)
            {
                Item itemData = ItemDataBase.instance.GetItemByID(inventorySlot.itemID);
                if (itemData != null && inventorySlot.ItemCount > 0)
                {
                    bool isUse = itemData.Use(player);
                    if (isUse)
                    {
                        inventorySlot.ItemCount--;  // 슬롯의 아이템 개수 감소
                        UpdateSlotUI();     // UI 갱신

                        Inventory inven = player.GetComponent<Inventory>();
                        if (inven != null && inventorySlot.ItemCount <= 0)
                        {
                            inven.RemoveItem(slotnum, 1);  // 아이템 제거 처리
                        }
                    }
                    else
                    {
                        Debug.Log("클릭 안됨");
                    }
                }
            }
        }
    }
}
