using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;
    public GameObject questItemPrefab; // 퀘스트 항목 프리팹
    public Transform content; // Scroll View의 Content
    private List<Quest> quests = new List<Quest>();
    private Dictionary<string, GameObject> questItemMap = new Dictionary<string, GameObject>(); // 퀘스트 이름과 프리팹을 매핑
    private Player currentPlayer;

    public GameObject QuestUIPanel;
    private bool activeQuestUI = false;

    void Awake()
    {
        // Singleton 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (currentPlayer != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                activeQuestUI = !activeQuestUI;
                QuestUIPanel.SetActive(activeQuestUI);
            }
        }
    }

    public void SetPlayer(Player player)
    {
        currentPlayer = player;
    }

    // 퀘스트 리스트를 업데이트하는 함수
    public void UpdateQuestList(List<Quest> questList)
    {
        quests = questList;

        foreach (var quest in quests)
        {
            // 이미 존재하는 퀘스트 항목인지 확인
            if (questItemMap.TryGetValue(quest.questName, out GameObject questItem))
            {
                // 이미 존재하면 텍스트만 업데이트
                UpdateQuestUI(questItem, quest);
            }
            else
            {
                // 존재하지 않으면 새로 생성하고 딕셔너리에 추가
                questItem = Instantiate(questItemPrefab, content);
                questItemMap[quest.questName] = questItem;
                UpdateQuestUI(questItem, quest);
            }
        }
    }

    // 퀘스트 UI 업데이트 함수
    private void UpdateQuestUI(GameObject questItem, Quest quest)
    {
        TextMeshProUGUI questNameText = questItem.transform.Find("QuestName").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI questStatusText = questItem.transform.Find("QuestText").GetComponent<TextMeshProUGUI>();

        // 퀘스트 이름 설정
        if (questNameText != null)
        {
            questNameText.text = quest.questName;
        }

        // 퀘스트 상태 업데이트
        if (questStatusText != null)
        {
            if (quest.status == QuestStatus.NotStarted)
            {
                questStatusText.text = $"퀘스트 진행 전: {quest.questName}";
            }
            else if (quest.status == QuestStatus.InProgress)
            {
                if (quest.requiredKills > 0)
                    questStatusText.text = $"적 처치 {quest.currentKills}/{quest.requiredKills}";
                else if (quest.requiresItemUsage)
                    questStatusText.text = $"아이템 사용 대기 중";
            }
            else if (quest.status == QuestStatus.Completed)
            {
                questStatusText.text = $"퀘스트 완료!";
            }
        }
    }

}
