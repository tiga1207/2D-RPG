using System.Collections.Generic;

public enum QuestStatus { NotStarted, InProgress, Completed, Rewarded,DonotPlayAgain } // 퀘스트 진행도
// public enum QuestTypes {Repeat, Main} // 퀘스트 종류 (반복 퀘스트, 메인퀘스트)
public enum QuestTypes {Main} // 퀘스트 종류 (반복 퀘스트, 메인퀘스트)


public enum QuestRewardTypes {Exp, Item} // 퀘스트 보상

[System.Serializable]
public class Quest
{
    public string questName;
    public string description;
    public NPC npc;
    public string npcId;
    public QuestTypes questType;
    public QuestStatus status = QuestStatus.NotStarted;
    public QuestRewardTypes rewardType;
    // 보상으로 지급될 경험치나 아이템
    public int experienceReward;
    public List<Item> rewardItems = new List<Item>();
    public int requiredKills; // 적 처치가 필요한 경우
    public int currentKills;  // 현재 처치한 적 수

    public bool requiresItemUsage; // 아이템 사용이 필요한 경우
    public bool itemUsed; // 아이템이 사용되었는지 여부

    // 퀘스트 완료 여부 체크
    public bool IsQuestCompleted()
    {
        if (requiresItemUsage)
        {
            return itemUsed;
        }
        else if (requiredKills > 0)
        {
            return currentKills >= requiredKills;
        }
        return false;
    }
    
}
