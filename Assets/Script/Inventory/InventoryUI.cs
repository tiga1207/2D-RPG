using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{

    public InventoryUI Instance;
    private Inventory inven;
    public GameObject inventoryPanel;
    bool activeInventory = false;

    [Header("Slot")]
    public Slot[] slots;
    public Transform slotHolder;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inventoryPanel.SetActive(false);
        StartCoroutine(InitInventory());
    }

    private IEnumerator InitInventory()
    {
        // Player 객체가 생성될 때까지 대기
        Player player = null;
        while (player == null)
        {
            player = FindObjectOfType<Player>();
            yield return null;
        }

        inven = player.GetComponent<Inventory>();
        if (inven == null)
        {
            yield break;
        }

        inventoryPanel.SetActive(activeInventory);
        inven.onSlotCountChange += SlotChange;
        slots = slotHolder.GetComponentsInChildren<Slot>();
        inven.onChangeItem += RedrawSlotUI;
        RedrawSlotUI();
        SlotChange(inven.SlotCnt);
    }

    private void SlotChange(int val)
    {
        for (int i = 0; i < slots.Length; ++i)
        {
            slots[i].slotnum = i;
            if (i < inven.SlotCnt)
            {
                slots[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                slots[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    private void Update()
    {
        if (inven == null) return; // Inventory 객체가 없으면 리턴
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !activeInventory;
            inventoryPanel.transform.SetAsLastSibling();//해당 ui를 최상단에서 보일 수 있도록 함.
            inventoryPanel.SetActive(activeInventory);
        }
    }

    public void AddSlot()
    {
        inven.SlotCnt++;
    }

    void RedrawSlotUI()
    {
        // 모든 슬롯을 초기화
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveSlot();
        }

        // 인벤토리에 있는 아이템을 UI 슬롯에 업데이트
        for (int i = 0; i < inven.items.Count; i++)
        {
            string itemID = inven.items[i].itemID; // InventorySlot의 itemID 가져오기
            int itemCount = inven.items[i].ItemCount; // 아이템 수량 가져오기

            // 아이템 ID로 아이템 데이터를 가져옴
            Item itemData = ItemDataBase.instance.GetItemByID(itemID);

            if (itemData != null)
            {
                // 슬롯에 아이템 데이터를 설정
                slots[i].inventorySlot = inven.items[i]; // InventorySlot을 슬롯에 할당
                slots[i].UpdateSlotUI();  // UI 갱신
            }
        }
    }
    
}

