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
        StartCoroutine(InitializeInventory());
    }

    private IEnumerator InitializeInventory()
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
            inventoryPanel.SetActive(activeInventory);
        }
    }

    public void AddSlot()
    {
        inven.SlotCnt++;
    }

    void RedrawSlotUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveSlot();
        }
        for (int i = 0; i < inven.items.Count; i++)
        {
            slots[i].item = inven.items[i];
            slots[i].UpdateSlotUI();
        }
    }
    
}

