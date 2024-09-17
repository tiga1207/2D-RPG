public enum QuestStatus { NotStarted, InProgress, Completed }

[System.Serializable]
public class Quest
{
    public string questName;
    public string description;
    public QuestStatus status = QuestStatus.NotStarted;

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
