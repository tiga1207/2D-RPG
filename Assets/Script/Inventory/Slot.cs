using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Slot : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public InventorySlot inventorySlot;  // InventorySlot 참조
    public Image itemIcon;
    public int slotnum;
    public TextMeshProUGUI itemCntText;
    public GameObject toolTip;
    private GameObject currentToolTip;  // 현재 활성화된 툴팁
    public RectTransform toolTipRectTransform;
    public TextMeshProUGUI tooTipItemName;
    public TextMeshProUGUI tooTipItemDesc;


    private void Start()
    {
        if (toolTip != null)
        {
            toolTipRectTransform = toolTip.GetComponent<RectTransform>();
            toolTip.SetActive(false); // 처음에는 툴팁 비활성화
        }
    }

    private void Update()
    {
        if (currentToolTip != null && currentToolTip.activeSelf)
        {
            // 마우스 위치를 화면 좌표로 변환
            Vector2 mousePos = Input.mousePosition;
            //툴팁 위치 조정(마우스 기준).
            currentToolTip.GetComponent<RectTransform>().position = new Vector2(mousePos.x+252f, mousePos.y-112f);
        }
    }

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
    
    public void OnPointerUp(PointerEventData eventData)//아이템 클릭 시
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
                        UpdateSlotUI(); // UI 갱신

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

    public void OnPointerEnter(PointerEventData eventData)
    {   
        if (inventorySlot != null && toolTip != null)
        {
            // 툴팁 프리팹을 인스턴스화
            currentToolTip = Instantiate(toolTip, transform.parent);

            // 툴팁 텍스트 컴포넌트를 동적으로 찾음
            TextMeshProUGUI toolTipNameText = currentToolTip.transform.Find("ToolTipName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI toolTipDescriptionText = currentToolTip.transform.Find("ToolTipDesc").GetComponent<TextMeshProUGUI>();

            // 아이템 데이터 가져오기
            Item itemData = ItemDataBase.instance.GetItemByID(inventorySlot.itemID);
            if (itemData != null)
            {
                // 아이템 이름과 설명을 툴팁에 표시
                toolTipNameText.text = itemData.itemName;
                toolTipDescriptionText.text = itemData.itemDescription; // GetDescription()은 아이템 설명을 반환하는 메서드
            }

            // 툴팁 활성화
            currentToolTip.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventorySlot != null)
        {
            Item itemData = ItemDataBase.instance.GetItemByID(inventorySlot.itemID);
            if (itemData != null && toolTip != null)
            {
                // Destroy(currentToolTip);
                currentToolTip.SetActive(false);
                Debug.Log("마우스 떠남.");
            }
        }
    }
}
