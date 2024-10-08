using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Inventory : MonoBehaviourPun
{
    private int slotCnt;

    public delegate void OnSlotCountChange(int val);
    public OnSlotCountChange onSlotCountChange;

    public delegate void OnChangeItem();
    public OnChangeItem onChangeItem;

    // InventorySlot에서 itemID와 itemCount만 관리
    public List<InventorySlot> items = new List<InventorySlot>();
    public static Inventory Instance;
    private Player currentPlayer;

    public int SlotCnt
    {
        get => slotCnt;
        set
        {
            slotCnt = value;
            onSlotCountChange?.Invoke(slotCnt); // Null 체크 후 호출
        }
    }

    // 초기화
    public void Initialize()
    {
        if (photonView.IsMine)
        {
            slotCnt = 2; // 슬롯 초기값
            onSlotCountChange?.Invoke(slotCnt);
        }
    }

    // 아이템 추가: itemID와 획득한 수량만 관리
    public bool AddItem(string itemID, int count = 1)
    {
        if (!photonView.IsMine) return false;

        // 이미 해당 아이템이 인벤토리에 있는지 확인
        InventorySlot existingSlot = items.Find(slot => slot.itemID == itemID);
        if (existingSlot != null)
        {
            // 이미 있으면 개수만 증가
            existingSlot.ItemCount += count;
            onChangeItem?.Invoke();
            return true;
        }

        // 슬롯이 남아있을 때 새로운 슬롯에 아이템 추가
        if (items.Count < slotCnt)
        {
            items.Add(new InventorySlot(itemID, count));
            onChangeItem?.Invoke();
            return true;
        }

        return false;  // 슬롯이 꽉 찼을 때
    }

    // 아이템 사용/제거
    public void RemoveItem(int _index, int count)
    {
        if (!photonView.IsMine) 
        {
            Debug.Log("can't remove because it's not mine");
            return;
        }

        if (_index >= 0 && _index < items.Count)
        {
            // 슬롯에 있는 아이템의 개수 감소
            items[_index].RemoveItemCount(count);
            
            // 아이템 개수가 0이면 해당 슬롯 삭제
            if (items[_index].ItemCount <= 0)
            {
                items.RemoveAt(_index);
            }

            onChangeItem?.Invoke();
        }
    }

    // 필드 아이템을 획득했을 때 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FieldItem") && photonView.IsMine)
        {
            FieldItems fieldItems = other.GetComponent<FieldItems>();
            if (fieldItems != null && AddItem(fieldItems.item.itemID, 1))
            {
                fieldItems.DestroyItem(); // 아이템 획득 후 필드에서 제거
            }
        }
    }
}
