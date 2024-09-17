using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    // private Quest activeQuest;
    private List<Quest> activeQuest;

    private QuestUI questUI;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        // QuestUI 인스턴스 초기화
        questUI = QuestUI.Instance;

        // activeQuest 리스트 초기화
        activeQuest = new List<Quest>();
    }

    public void StartQuest(Quest quest)
    {
        if (!activeQuest.Contains(quest))
        {
            quest.status = QuestStatus.InProgress;
            activeQuest.Add(quest);
            UpdateQuestUI();
        }
    }
     public void UpdateKillCount()
    {
        foreach (var quest in activeQuest)
        {
            if (quest.status == QuestStatus.InProgress && quest.requiredKills > 0)
            {
                quest.currentKills++;
                Debug.Log($"적 처치 수: {quest.questName} - {quest.currentKills}/{quest.requiredKills}");

                if (quest.IsQuestCompleted())
                {
                    CompleteQuest(quest);
                }
                else{
                    UpdateQuestUI();
                }
            }
        }
    }

    public void UseItem()
    {
        foreach (var quest in activeQuest)
        {
            if (quest.status == QuestStatus.InProgress && quest.requiresItemUsage)
            {
                quest.itemUsed = true;
                Debug.Log($"{quest.questName} - 아이템을 사용했습니다.");

                if (quest.IsQuestCompleted())
                {
                    CompleteQuest(quest);
                }
                else{
                    UpdateQuestUI();
                }
            }
        }
    }

    // 퀘스트 완료 처리
    public void CompleteQuest(Quest quest)
    {
        quest.status = QuestStatus.Completed;
        Debug.Log($"퀘스트 완료: {quest.questName}");
        // 완료된 퀘스트는 활성 리스트에서 제거하거나 필요 시 유지
        UpdateQuestUI();
        // activeQuest.Remove(quest);
    }
    
    private void UpdateQuestUI()
    {
        if (questUI != null)
        {
            // 활성 퀘스트 리스트 전체를 넘겨서 UI를 업데이트
            questUI.UpdateQuestList(activeQuest);
        }
    }
}
