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

    public Image UltimateImage; // 대쉬 스킬을 표시할 UI 이미지
    public TextMeshProUGUI UltimateText;

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
        InitializeUI(player.dashCooldownTimer,player.healCoolTimer,player.ultimateAttackCooldownTimer);
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
    public void UpdateUltimate(float ultimateAttackCooldownTimer)
    {
        if (UltimateImage != null)
        {
            UltimateImage.fillAmount = ultimateAttackCooldownTimer/currentPlayer.ultimateAttackCooldown;
        }
        if (UltimateText != null)
        {          
            if(ultimateAttackCooldownTimer == currentPlayer.dashCooldown || ultimateAttackCooldownTimer<=0)
            {
                UltimateText.text = null;
            }
            else{
                UltimateText.text = ultimateAttackCooldownTimer.ToString("F0");
            }
        }
    }

    public void InitializeUI(float dashCooldownTimer,float healCoolTimer, float ultimateAttackCooldownTimer)
    {
        UpdateDash(dashCooldownTimer);
        UpdateHeal(healCoolTimer);
        UpdateUltimate(ultimateAttackCooldownTimer);

    }

    public void UpdatePlayerUI()
    {
        if (currentPlayer != null)
        {
            UpdateDash(currentPlayer.dashCooldownTimer);
            UpdateHeal(currentPlayer.healCoolTimer);
            UpdateUltimate(currentPlayer.ultimateAttackCooldownTimer);
        }
    }
}
