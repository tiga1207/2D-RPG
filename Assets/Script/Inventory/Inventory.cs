using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private int slotCnt;

    public delegate void OnSlotCountChange(int val);
    public OnSlotCountChange onSlotCountChange;

    public delegate void OnChangeItem();
    public OnChangeItem onChangeItem;
    public List<Item> items = new List<Item>(); 

    public int SlotCnt
    {
        get => slotCnt;
        set
        {
            slotCnt = value;
            onSlotCountChange?.Invoke(slotCnt); // Null 체크 후 호출
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
    }

    public void Initialize()
    {
        slotCnt = 4;
        onSlotCountChange?.Invoke(slotCnt);
    }

    public bool AddItem(Item _item)
    {
        if(items.Count < slotCnt)
        {
            items.Add(_item);
            if(onChangeItem!=null)
            onChangeItem?.Invoke();
            return true;
        }
        return false;
    }

    public void RemoveItem(int _index)
    {
        items.RemoveAt(_index);
        onChangeItem?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("FieldItem"))
        {
            FieldItems fieldItems = other.GetComponent<FieldItems>();
            if(AddItem(fieldItems.GetItem()))
            {
                fieldItems.DestroyItem();
            }
        }
    }
}

