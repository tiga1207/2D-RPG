using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainScene : MonoBehaviour
{

    public TMP_InputField NickNameInput;
    public GameObject MainPanel;
    public GameObject GamePanel;
    public GameObject OptionPanel;
    public Button GameStartBtn;
    public Button GameOptionBtn;

    void Start()
    {
        if(GameStartBtn!=null)
        {
            GameStartBtn.onClick.AddListener(GameStart);
        }
        if(GameOptionBtn!=null)
        {
            GameOptionBtn.onClick.AddListener(Option);
        }
    }

    public void GameStart()
    {
        MainPanel.SetActive(false);
        GamePanel.SetActive(true);
    }
    public void Option()
    {
        MainPanel.SetActive(false);
        OptionPanel.SetActive(true);
    }

    public void BackToMain()
    {
        if(GamePanel.activeSelf)
        {
            GamePanel.SetActive(false);
        }
        if(OptionPanel.activeSelf)
        {
            OptionPanel.SetActive(false);
        }
        MainPanel.SetActive(true);
    }
}
