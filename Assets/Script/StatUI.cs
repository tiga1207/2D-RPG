using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    public GameObject StatUIPanel;
    public static StatUI Instance;
    private Player currentPlayer;
    public TextMeshProUGUI hpText;

    public TextMeshProUGUI mpText;

    public TextMeshProUGUI expText;
    public TextMeshProUGUI levelText;

    public TextMeshProUGUI damageText;
    public TextMeshProUGUI statPointText;

    bool activeStat = false;

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
        StatUIPanel.SetActive(false);
    }

    public void SetPlayer(Player player)
    {
        currentPlayer = player;
        InitializeUI(player.MaxHp, player.MaxMp, player.Level, player.Damage, player.LevelupStatPoint);
    }

    public void UpdateHP(float maxHp)
    {
        if (hpText != null)
        {
            hpText.text = "Player Max Hp:\t"+maxHp.ToString("F0");
        }
    }

    public void UpdateMP(float maxMp)
    {
        if (mpText != null)
        {
            mpText.text = "Player Max Mp:\t"+maxMp.ToString("F0");
        }
    }
    public void UpdateLEVEL(float level)
    {

        if (levelText != null)
        {
            levelText.text = "Player Level:\t"+level.ToString("F0");
        }
    }
    public void UpdateDamage(float damage)
    {

        if (damageText != null)
        {
            damageText.text = "Player Damage:\t"+damage.ToString("F0");
        }
    }

    public void UpdateStatPoint(float statPoint)
    {

        if (statPointText != null)
        {
            statPointText.text = "Player StatPoint:\t"+statPoint.ToString("F0");
        }
    }

    public void InitializeUI(float maxHp, float maxMp, float level, float damage, float statPoint)
    {
        UpdateHP(maxHp);
        UpdateMP(maxMp);
        UpdateLEVEL(level);
        UpdateDamage(damage);
        UpdateStatPoint(statPoint);
    }

    public void UpdatePlayerUI()
    {
        if (currentPlayer != null)
        {
            UpdateHP(currentPlayer.MaxHp);
            UpdateMP(currentPlayer.MaxMp);
            UpdateLEVEL(currentPlayer.Level);
            UpdateDamage(currentPlayer.Damage);
            UpdateStatPoint(currentPlayer.LevelupStatPoint);
        }
    }
    private void Update()
    {
        if (currentPlayer != null)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                activeStat = !activeStat;
                StatUIPanel.SetActive(activeStat);
            }
        }
    }

    public void ApplyStatToDamage()
    {
        if(currentPlayer.LevelupStatPoint>0)
        {
            currentPlayer.Damage++;
            currentPlayer.LevelupStatPoint--;
        }
    }
    
}

