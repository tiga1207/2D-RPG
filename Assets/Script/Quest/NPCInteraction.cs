using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public NPC npcData; // NPC별 고유 데이터
    public DialogueManager dialogueManager;
    private bool isPlayerNearby = false; // 플레이어가 NPC 근처에 있는지 여부

    // 플레이어가 NPC 근처에 있을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    // 플레이어가 NPC 근처를 벗어났을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F)) // F키 눌렀을 때
        {
            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(npcData); // 클릭한 NPC의 대화 시작
            }
        }
    }
}
