using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Image hpImage; // HP를 표시할 UI 이미지
    public Image mpImage; // MP를 표시할 UI 이미지
    public Image expImage; // EXP를 표시할 UI 이미지
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI levelText;

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
        if (hpImage != null)
        {
            hpImage.fillAmount = hp / maxHp;
        }
        if (hpText != null)
        {
            hpText.text = hp.ToString("F0");
        }
    }

    public void UpdateMP(float mp, float maxMp)
    {
        if (mpImage != null)
        {
            mpImage.fillAmount = mp / maxMp;
        }
        if (mpText != null)
        {
            mpText.text = mp.ToString("F0");
        }
    }

    public void UpdateEXP(float exp, float maxExp)
    {
        if (expImage != null)
        {
            expImage.fillAmount = exp / maxExp;
        }
        if (expText != null)
        {
            expText.text = exp.ToString("F0");
        }
    }

    public void UpdateLEVEL(float level)
    {

        if (levelText != null)
        {
            levelText.text = "레벨: "+level.ToString("F0");
        }
    }

    public void InitializeUI(float hp, float maxHp, float mp, float maxMp, float exp, float maxExp, float level)
    {
        UpdateHP(hp, maxHp);
        UpdateMP(mp, maxMp);
        UpdateEXP(exp, maxExp);
        UpdateLEVEL(level);
    }

    public void UpdatePlayerUI()
    {
        if (currentPlayer != null)
        {
            UpdateHP(currentPlayer.Hp, currentPlayer.MaxHp);
            UpdateMP(currentPlayer.Mp, currentPlayer.MaxMp);
            UpdateEXP(currentPlayer.Exp, currentPlayer.MaxExp);
            UpdateLEVEL(currentPlayer.Level);
        }
    }
}
