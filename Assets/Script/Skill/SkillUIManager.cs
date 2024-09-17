using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager Instance;
    private Player currentPlayer; // 현재 플레이어

    //대쉬 스킬
    public Image dashImage; // 대쉬 스킬을 표시할 UI 이미지
    public TextMeshProUGUI dashText;

    //힐 스킬
    public Image healImage; // 대쉬 스킬을 표시할 UI 이미지
    public TextMeshProUGUI healText;

    void Awake()
    {
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
        InitializeUI(player.dashCooldownTimer,player.healCoolTimer);
    }

    public void UpdateDash(float dashCooldownTimer)
    {
        if (dashImage != null)
        {
            dashImage.fillAmount = dashCooldownTimer/currentPlayer.dashCooldown;
        }
        if (dashText != null)
        {          
            if(dashCooldownTimer == currentPlayer.dashCooldown || dashCooldownTimer<=0)
            {
                dashText.text = null;
            }
            else{
                dashText.text = dashCooldownTimer.ToString("F0");
            }
        }
    }

    public void UpdateHeal(float healCoolTimer)
    {
        if (healImage != null)
        {
            healImage.fillAmount = healCoolTimer/currentPlayer.healCooldown;
        }
        if (healText != null)
        {          
            if(healCoolTimer == currentPlayer.dashCooldown || healCoolTimer<=0)
            {
                healText.text = null;
            }
            else{
                healText.text = healCoolTimer.ToString("F0");
            }
        }
    }

    public void InitializeUI(float dashCooldownTimer,float healCoolTimer)
    {
        UpdateDash(dashCooldownTimer);
        UpdateHeal(healCoolTimer);

    }

    public void UpdatePlayerUI()
    {
        if (currentPlayer != null)
        {
            UpdateDash(currentPlayer.dashCooldownTimer);
            UpdateHeal(currentPlayer.healCoolTimer);
        }
    }
}
