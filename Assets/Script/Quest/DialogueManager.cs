using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject Dialogue;
    private NPC currentNPC; // 현재 대화 중인 NPC
    public Image currentNpcImage;
    public TextMeshProUGUI currentNpcName;
    public Button questAcceptButtons; // 퀘스트 수락 버튼
    public Button questDeclineButtons; // 퀘스트 취소 버튼

    public Button questCancelButtons; //퀘스트 창 닫기 버튼
    public TextMeshProUGUI dialogueText; // 대화 텍스트 UI
    private int dialogueIndex = 0;
    private void Start()
    {
        if (questAcceptButtons != null)
        {
            questAcceptButtons.onClick.AddListener(AcceptQuest);
        }
        if (questDeclineButtons != null)
        {
            questDeclineButtons.onClick.AddListener(DeclineQuest);
        }
        if (questCancelButtons != null)
        {
            questCancelButtons.onClick.AddListener(CancelQuest);
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 버튼 클릭 감지
        {
            // 화면을 클릭했을 때 대화창의 범위 안인지 확인
            if (Dialogue.activeSelf)
            {
                ShowNextDialogue();
            }
        }
    }
    // NPC 대화를 시작할 때 호출
    public void StartDialogue(NPC npc)
    {
        currentNPC = npc;
        currentNpcName.text= currentNPC.npcName;
        currentNpcImage.sprite= currentNPC.npcImage;
        dialogueIndex = 0;
        ShowNextDialogue();
    }

    public void ShowNextDialogue()
    {
        if (currentNPC.IsQuestInProgress())
        {
            Dialogue.SetActive(true);
            dialogueText.text = "퀘스트 진행 중입니다.";
            questAcceptButtons.gameObject.SetActive(false);
            questDeclineButtons.gameObject.SetActive(false);
            questCancelButtons.gameObject.SetActive(true);
            return;
        }

        if (currentNPC != null && dialogueIndex < currentNPC.dialogues.Count)
        {
            Dialogue.SetActive(true);
            questCancelButtons.gameObject.SetActive(false);
            questAcceptButtons.gameObject.SetActive(false);
            questDeclineButtons.gameObject.SetActive(false);
            dialogueText.text = currentNPC.dialogues[dialogueIndex]; // 대화 텍스트 표시
            dialogueIndex++;
        }
        else
        {
            // 대화가 끝났을 때 퀘스트 수락/취소 버튼 표시
            questAcceptButtons.gameObject.SetActive(true);
            questDeclineButtons.gameObject.SetActive(true);
        }
    }

    public void AcceptQuest()
    {
        if (currentNPC != null && currentNPC.quest != null)
        {
            Debug.Log($"{currentNPC.npcName}의 퀘스트 수락: {currentNPC.quest.questName}");
            QuestManager.instance.StartQuest(currentNPC.quest);
            questAcceptButtons.gameObject.SetActive(false);
            questDeclineButtons.gameObject.SetActive(false);
            Dialogue.SetActive(false);
        }
    }

    public void DeclineQuest()
    {
        Debug.Log("퀘스트를 거절했습니다.");
        questAcceptButtons.gameObject.SetActive(false);
        questDeclineButtons.gameObject.SetActive(false);
        Dialogue.SetActive(false);

    }
    public void CancelQuest()
    {
        Debug.Log("퀘스트 창을 닫습니다.");
        questCancelButtons.gameObject.SetActive(false);
        Dialogue.SetActive(false);
    }

}
