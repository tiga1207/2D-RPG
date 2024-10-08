using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public int itemCntMax = 99;
    public string itemID;
    public bool canOverlap;

    [SerializeField]private int _itemCount;

    // 아이템 개수 프로퍼티
    public int ItemCount
    {
        get { return _itemCount; }
        set
        {
            // 값이 변경될 때만 실행
            if (_itemCount != value)
            {
                _itemCount = Mathf.Clamp(value, 0, itemCntMax); // 최소 0, 최대 itemCntMax로 제한
            }
        }
    }

    public InventorySlot(string id, int count)
    {
        itemID = id;
        ItemCount = count;
    }

    public void RemoveItemCount(int count)
    {
        ItemCount -= count;
    }
}
