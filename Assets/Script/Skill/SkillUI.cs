using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// // 스킬 포인트를 찍을 수 있는 ui. 스킬 포인트가 0일 경우 스킬 이미지가 회색처럼 비활성화가 되게 함. 스킬 이미지에 마우스를 hover시키면 툴팁을 띄워 설명
// //스킬이미지 + - 버튼, 캔버스 닫는창 바로아래에 스킬 포인트 위치시키기.

public class SkillUI : MonoBehaviour
{
    public GameObject SkillUIPanel;
    public GameObject DashArea;
    public GameObject HealArea;
    public GameObject UltimateArea;
    public static SkillUI Instance;

    public GameObject skillPrefab; // 스킬 항목 프리팹
    public Transform contentPanel; // 스크롤뷰의 Content
    public TextMeshProUGUI skillPointText;

    private Player currentPlayer;
    private bool activeSkillUI = false;
    
    // 스킬 리스트를 저장할 변수
    public List<Skill> skillList = new List<Skill>(); 

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

    void Start()
    {
        SkillUIPanel.SetActive(false);
        // 스킬 리스트를 스크롤뷰에 출력
        SkillList();
    }

    void Update()
    {
        if (currentPlayer != null)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                activeSkillUI = !activeSkillUI;
                SkillUIPanel.transform.SetAsLastSibling();//해당 ui를 최상단에서 보일 수 있도록 함.
                SkillUIPanel.SetActive(activeSkillUI);
            }
        }
    }


    public void SetPlayer(Player player)
    {
        currentPlayer = player;
        InitializeUI(player.LevelupSkillPoint);
    }

    public void InitializeUI(float skillPoint)
    {
        UpdateSkillPoint(skillPoint);
    }

    // 스크롤뷰에 스킬 리스트 출력
    void SkillList()
    {
        foreach (Skill skill in skillList)
        {
            GameObject skillItem = Instantiate(skillPrefab, contentPanel);

            skillItem.transform.Find("SkillName").GetComponent<TextMeshProUGUI>().text = skill.skillName;
            skillItem.transform.Find("SkillIcon").GetComponent<Image>().sprite = skill.icon;

            skillItem.transform.Find("SkillLevel").GetComponent<TextMeshProUGUI>().text = "Lv: " + skill.level.ToString("F0");
            skill.levelText= skillItem.transform.Find("SkillLevel").GetComponent<TextMeshProUGUI>();

            Button addButton = skillItem.transform.Find("SkillAddBtn").GetComponent<Button>();
            Button removeButton = skillItem.transform.Find("SkillRemoveBtn").GetComponent<Button>();

            addButton.onClick.AddListener(() => ApplySkillPoint(skill));
            removeButton.onClick.AddListener(() => RemoveSkillPoint(skill));
        }
    }
    

    // 스킬 포인트 적용
    public void ApplySkillPoint(Skill skill)
    {
        if (currentPlayer.LevelupSkillPoint > 0 && skill.level < skill.maxLevel)
        {
            currentPlayer.LevelupSkillPoint--;
            skill.IncreaseLevel();
            UpdatePlayerUI();
        }
    }

    // 스킬 포인트 감소
    public void RemoveSkillPoint(Skill skill)
    {
        if (skill.level > 0)
        {
            currentPlayer.LevelupSkillPoint++;
            skill.DecreaseLevel();
            UpdatePlayerUI();
        }
    }

    //스킬 포인트 표시 텍스트
    public void UpdateSkillPoint(float statPoint)
    {
        if (skillPointText != null)
        {
            skillPointText.text = "Player SkillPoint:\t"+statPoint.ToString("F0");
        }
    }

    public void UpdatePlayerUI()
    {
        // 스킬 포인트 텍스트 갱신
        UpdateSkillPoint(currentPlayer.LevelupSkillPoint);
        UpdateSkillAbility();
    }

    private void UpdateSkillAbility()
    {
        foreach (Skill skill in skillList)
        {
            if (skill.skillName == "Dash")
            {
                if (skill.level == 0)
                {
                    currentPlayer.dashSkillActivate = false;
                    DashArea.SetActive(false);
                }
                else
                {
                    currentPlayer.dashSkillActivate = true;
                    DashArea.SetActive(true);
                }
            }
            else if (skill.skillName == "Heal")
            {
                if (skill.level == 0)
                {
                    currentPlayer.healSkillActivate = false;
                    HealArea.SetActive(false);
                }
                else
                {
                    currentPlayer.healSkillActivate = true;
                    HealArea.SetActive(true);
                }
            }
            else if (skill.skillName == "Ultimate")
            {
                if (skill.level == 0)
                {
                    currentPlayer.ultimateSkillActivate = false;
                    UltimateArea.SetActive(false);
                }
                else
                {
                    currentPlayer.ultimateSkillActivate = true;
                    UltimateArea.SetActive(true);
                }
            }
        }
    }
}
