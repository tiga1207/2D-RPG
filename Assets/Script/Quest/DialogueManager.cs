using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject Dialogue;
    private NPC currentNPC; // 현재 대화 중인 NPC
    public Image currentNpcImage;
    public TextMeshProUGUI currentNpcName;
    public Button questAcceptButtons; // 퀘스트 수락 버튼
    public Button questDeclineButtons; // 퀘스트 취소 버튼

    public Button questCancelButtons; //퀘스트 창 닫기 버튼
    public TextMeshProUGUI dialogueText; // 대화 텍스트 UI
    private int dialogueIndex = 0;
    private bool IsDialogueActive = false; // 대화 활성화 상태 -> 대화 완료 후에도 대화가 다시 시작되는 것을 방지

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
        if (IsDialogueActive && Input.GetMouseButtonDown(0)) // 왼쪽 마우스 버튼 클릭 감지
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
        currentNpcName.text= currentNPC.npcId;
        currentNpcImage.sprite= currentNPC.npcImage;
        dialogueIndex = 0;
        IsDialogueActive = true;
        ShowNextDialogue();
    }

    public void ShowNextDialogue()
    {
        if(currentNPC != null)
        {
            if (!IsDialogueActive)
            {
                return;
            }

            if (currentNPC.IsQuestInProgress())
            {
                Dialogue.SetActive(true);
                dialogueText.text = "퀘스트 진행 중입니다.";
                questAcceptButtons.gameObject.SetActive(false);
                questDeclineButtons.gameObject.SetActive(false);
                questCancelButtons.gameObject.SetActive(true);
                return;
            }
        
            else if(currentNPC.IsQuestCompleted())
            {
                QuestManager questManager= QuestManager.instance;
                Dialogue.SetActive(true);
                questManager.RewardAndQuestStatusManager(currentNPC);
                // dialogueText.text = "퀘스트 완료를 확인했습니다. 보상을 지급합니다..";
                questAcceptButtons.gameObject.SetActive(false);
                questDeclineButtons.gameObject.SetActive(false);
                questCancelButtons.gameObject.SetActive(true);
                return;
            }

            else if (currentNPC.IsNotStarted() && dialogueIndex < currentNPC.dialogues.Count)
            {
                Dialogue.SetActive(true);
                questCancelButtons.gameObject.SetActive(false);
                questAcceptButtons.gameObject.SetActive(false);
                questDeclineButtons.gameObject.SetActive(false);
                dialogueText.text = currentNPC.dialogues[dialogueIndex]; // 대화 텍스트 표시
                dialogueIndex++;
            }
            else if(currentNPC.IsNotStarted() && dialogueIndex >= currentNPC.dialogues.Count)
            {
                // 대화가 끝났을 때 퀘스트 수락/취소 버튼 표시
                questAcceptButtons.gameObject.SetActive(true);
                questDeclineButtons.gameObject.SetActive(true);
            }

        }   
    }

    public void AcceptQuest()
    {
        if (currentNPC != null && currentNPC.quest != null)
        {
            IsDialogueActive = false;
            QuestManager.instance.StartQuest(currentNPC.quest);
            questAcceptButtons.gameObject.SetActive(false);
            questDeclineButtons.gameObject.SetActive(false);
            Dialogue.SetActive(false);

        }
    }

    public void DeclineQuest()
    {
        IsDialogueActive = false;
        Debug.Log("퀘스트를 거절했습니다.");
        questAcceptButtons.gameObject.SetActive(false);
        questDeclineButtons.gameObject.SetActive(false);
        Dialogue.SetActive(false);

    }
    public void CancelQuest()
    {
        IsDialogueActive = false;
        Debug.Log("퀘스트 창을 닫습니다.");
        questCancelButtons.gameObject.SetActive(false);
        Dialogue.SetActive(false);
    }

}
