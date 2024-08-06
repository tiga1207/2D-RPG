using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager Instance;

    public Image dashImage; // HP를 표시할 UI 이미지
    public TextMeshProUGUI dashText;
    private Player currentPlayer; // 현재 플레이어

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
    }

    public void SetPlayer(Player player)
    {
        currentPlayer = player;
        InitializeUI(player.Hp, player.MaxHp, player.Mp, player.MaxMp, player.Exp, player.MaxExp,player.Level);
    }

    public void UpdateHP(float hp, float maxHp)
    {
        if (dashImage != null)
        {
            dashImage.fillAmount = hp / maxHp;
        }
        if (dashText != null)
        {
            dashText.text = hp.ToString("F0");
        }
    }

    public void InitializeUI(float hp, float maxHp, float mp, float maxMp, float exp, float maxExp, float level)
    {
        UpdateHP(hp, maxHp);

    }

    public void UpdatePlayerUI()
    {
        if (currentPlayer != null)
        {
            UpdateHP(currentPlayer.Hp, currentPlayer.MaxHp);
        }
    }
}
