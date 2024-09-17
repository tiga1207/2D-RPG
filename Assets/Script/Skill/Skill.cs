using TMPro;
using UnityEngine;

[System.Serializable]
public class Skill
{
    private Player currentPlayer;
    public string skillName;      // 스킬 이름
    public string description;    // 스킬 설명
    public int level;             // 스킬 레벨
    public Sprite icon;           // 스킬 아이콘
    public int maxLevel;          // 스킬 최대 레벨
    public TextMeshProUGUI levelText;


    public Skill(string skillName, string description, Sprite icon, int maxLevel)
    {
        this.skillName = skillName;
        this.description = description;
        this.level = 0; // 기본적으로 0레벨
        this.icon = icon;
        this.maxLevel = maxLevel;
    }

    public bool IsActive()
    {
        return level>0;
    }

    // 레벨 증가
    public void IncreaseLevel()
    {
        if (level < maxLevel)
        {
            level++;
            UpdateLevelText();
        }
    }

    // 레벨 감소
    public void DecreaseLevel()
    {
        if (level > 0)
        {
            level--;
            UpdateLevelText();
        }
    }

    // 레벨 텍스트 업데이트
    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Lv: " + level.ToString("F0");
        }
    }
}
