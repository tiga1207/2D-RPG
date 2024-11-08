using TMPro;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public NPC npcData; // NPC별 고유 데이터
    public TMP_Text NickNameText;
    public TMP_Text questProgress;
    public GameObject interactionDesc;
    public GameObject squre;
    public DialogueManager dialogueManager;
    private bool isPlayerNearby = false; // 플레이어가 NPC 근처에 있는지 여부

    // 플레이어가 NPC 근처에 있을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            interactionDesc.SetActive(true);
        }
    }

    // 플레이어가 NPC 근처를 벗어났을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactionDesc.SetActive(false);
        }
    }
    private void QuestProgress()
    {
        if (npcData.quest.status == QuestStatus.NotStarted)
        {
            questProgress.text = "!";
        }
        else if (npcData.quest.status == QuestStatus.InProgress)
        {
            questProgress.text = "···";
        }
        else if (npcData.quest.status == QuestStatus.Completed)
        {
            questProgress.text = "O";
        }
        else
        {
            questProgress.text = "";
            squre.SetActive(false);
        }
    }

    private void Start()
    {
        NickNameText.text = npcData.npcName;
    }

    void Update()
    {
        QuestProgress();
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F)) // F키 눌렀을 때
        {
            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(npcData); // 클릭한 NPC의 대화 시작
            }
        }
        
    }
}
