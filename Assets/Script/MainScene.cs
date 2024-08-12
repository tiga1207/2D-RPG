using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MainScene : MonoBehaviourPunCallbacks
{
    public TMP_InputField NickNameInput;
    public GameObject MainPanel;
    public GameObject GamePanel;
    public GameObject OptionPanel;
    public Button GoGamePanelBtn;
    public Button GoGameOptionBtn;
    public Button Roombtn;

    private void Start()
    {
        if (GoGamePanelBtn != null)
        {
            GoGamePanelBtn.onClick.AddListener(GoGamePanel);
        }
        if (GoGameOptionBtn != null)
        {
            GoGameOptionBtn.onClick.AddListener(GoOptionPanel);
        }
        if (Roombtn != null)
        {
            Roombtn.onClick.AddListener(OnRoomButtonClicked);
        }
    }

    public void GoGamePanel()
    {
        MainPanel.SetActive(false);
        GamePanel.SetActive(true);
    }

    public void GoOptionPanel()
    {
        MainPanel.SetActive(false);
        OptionPanel.SetActive(true);
    }

    public void BackToMain()
    {
        if (GamePanel.activeSelf)
        {
            GamePanel.SetActive(false);
        }
        if (OptionPanel.activeSelf)
        {
            OptionPanel.SetActive(false);
        }
        MainPanel.SetActive(true);
    }

    private void OnRoomButtonClicked()
    {
        string nickName = NickNameInput.text;
        PhotonNetwork.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Cave_1");
    }
}
