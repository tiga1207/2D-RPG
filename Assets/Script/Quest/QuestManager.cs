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

    public void RewardAndQuestStatusManager(NPC npc)
    {
        List<Quest> removeQuest= new List<Quest>();
        
        foreach(var quest in activeQuest)
        {
            if(quest.status == QuestStatus.Completed && quest.npcId == npc.npcId) // 퀘스트 완료시 
            {
                GetReward(quest);
            }
            
            // if (quest.status == QuestStatus.Rewarded && quest.questType == QuestTypes.Repeat)
            // {
            //     // ResetQuest(quest); // 반복 퀘스트는 초기화해서 매번 퀘스트 완료 조건 만족 시 마다 해당 npc에게 보상 받을 수 있도록. 혹은 추후에 보상버튼을 통해 받을 수 있도록하기.
            //     // removeQuest.Add(quest);
            // }
            if (quest.status == QuestStatus.Rewarded && quest.questType == QuestTypes.Main)
            {
                quest.status = QuestStatus.DonotPlayAgain;
                removeQuest.Add(quest);
            }
        }

        foreach (var quest in removeQuest)
        {
            RemoveQuest(quest);
        }
        UpdateQuestUI();

    }

    // 퀘스트 완료 처리
    public void CompleteQuest(Quest quest)
    {
        quest.status = QuestStatus.Completed;
        Debug.Log($"퀘스트 완료: {quest.questName}");
        UpdateQuestUI();
    }

    //퀘스트 보상
    public void GetReward(Quest quest)
    {
        Player player = Player.LocalPlayerInstance;
        DialogueManager dialogueManager = DialogueManager.Instance;
        //보상이 경험치일 경우
        if(quest.rewardType == QuestRewardTypes.Exp)
        {
            player.AddExp(quest.experienceReward);
            dialogueManager.dialogueText.text ="퀘스트 완료를 확인했습니다. 보상을 지급합니다..";
            quest.status = QuestStatus.Rewarded;//퀘스트 상태를 보상받은 상태로 변경

            Debug.Log("퀘스트 완료 경험치 지급");
        }
        //보상이 아이템일 경우
        else if (quest.rewardType == QuestRewardTypes.Item)
        {
            dialogueManager.dialogueText.text ="퀘스트 완료를 확인했습니다. 보상을 지급합니다..";
            if(quest.rewardItems.Count < player.playerInventory.SlotCnt) // 보상 아이템 수가 슬롯 남은 수보다 적을때 (슬롯 널널)
            {
                foreach (var item in quest.rewardItems)
                {
                    player.playerInventory.AddItem(item);
                }       
            quest.status = QuestStatus.Rewarded;//퀘스트 상태를 보상받은 상태로 변경

            }
            else// 슬롯 부족 문제 시
            {
                dialogueManager.dialogueText.text ="슬롯이 부족합니다. 슬롯을 비우고 다시 시도하세요.";
                return;
            }
        }
        UpdateQuestUI();

    }
    public void ResetQuest(Quest quest)
    {
        quest.status = QuestStatus.NotStarted;
        quest.currentKills = 0;
        quest.itemUsed = false;

    }    
    public void RemoveQuest(Quest quest)
    {
        activeQuest.Remove(quest);
        
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
