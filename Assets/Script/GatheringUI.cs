using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatheringUI : MonoBehaviour
{
    private InventoryUI inventoryUI;
    bool activeInventory = false;
    public Button invenBtn;

    private SkillUI skillUI;
    bool activeSkillUI = false;
    public Button skillBtn;

    private StatUI statUI;
    bool activeStatUI = false;
    public Button statBtn;

    private QuestUI questUI;
    bool activeQuestUI = false;
    public Button questBtn;

    // Start is called before the first frame update
    void Start()
    {
        inventoryUI = InventoryUI.Instance;
        skillUI = SkillUI.Instance;
        statUI=StatUI.Instance;
        questUI=QuestUI.Instance;

        if (invenBtn != null)
        {
            invenBtn.onClick.AddListener(SetOnInvenUI);
        }

        if (skillBtn != null)
        {
            skillBtn.onClick.AddListener(SetOnSkillUI);
        }

        if (statBtn != null)
        {
            statBtn.onClick.AddListener(SetOnStatUI);
        }

        if (questBtn != null)
        {
            questBtn.onClick.AddListener(SetOnQuestUI);
        }
    }

    private void SetOnInvenUI()
    {
        activeInventory = !activeInventory;
        inventoryUI.inventoryPanel.SetActive(activeInventory);
        if (activeInventory)
        {
            inventoryUI.inventoryPanel.transform.SetAsLastSibling();

        }
    }

    private void SetOnSkillUI()
    {
        activeSkillUI = !activeSkillUI;
        inventoryUI.inventoryPanel.SetActive(activeSkillUI);
        if (activeSkillUI)
        {
            skillUI.SkillUIPanel.transform.SetAsLastSibling();

        }
    }

    private void SetOnStatUI()
    {
        activeStatUI = !activeStatUI;
        statUI.StatUIPanel.SetActive(activeStatUI);
        if (activeStatUI)
        {
            statUI.StatUIPanel.transform.SetAsLastSibling();

        }
    }

    private void SetOnQuestUI()
    {
        activeQuestUI = !activeQuestUI;
        questUI.QuestUIPanel.SetActive(activeQuestUI);
        if (activeQuestUI)
        {
            questUI.QuestUIPanel.transform.SetAsLastSibling();

        }
    }
}
