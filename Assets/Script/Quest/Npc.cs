using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcId;
    public Sprite npcImage;
    public List<string> dialogues; // NPC의 대화 리스트
    public Quest quest; // NPC가 제공하는 퀘스트

    public bool IsQuestInProgress()
    {
        if (quest != null)
        {
            return quest.status == QuestStatus.InProgress;
        }
        return false;
    }

    public bool IsQuestCompleted()
    {
        if (quest != null)
        {
            return quest.status == QuestStatus.Completed;
        }
        return false;
    }
    public bool IsNotStarted()
    {
        if (quest != null)
        {
            return quest.status == QuestStatus.NotStarted;
        }
        return false;
    }
}
