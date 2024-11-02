using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MainScene : MonoBehaviourPunCallbacks
{
    public TMP_Text loadingText;
    public TMP_InputField NickNameInput;
    public GameObject MainPanel;
    public GameObject GamePanel;
    public GameObject OptionPanel;
    public Button GoGamePanelBtn;
    public Button GoGameOptionBtn;
    public Button QuitGameBtn;
    public Button Roombtn;
        private Coroutine loadingCoroutine;

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
        if (QuitGameBtn != null)
        {
            QuitGameBtn.onClick.AddListener(QuitGame);
        }
    }

    public void GoGamePanel()
    {
        MainPanel.SetActive(false);
        GamePanel.SetActive(true);
    }
    public void QuitGame()
    {
        Debug.Log("나가기 버튼 눌림");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
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

        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(true);
            if (loadingCoroutine == null)
            {
                loadingCoroutine = StartCoroutine(LoadingAnimation());
            }
        }
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(false);
            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
                loadingCoroutine = null;
            }
        }
        PhotonNetwork.LoadLevel("Cave_1");
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(false);
            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
                loadingCoroutine = null;
            }
        }
    }
    private IEnumerator LoadingAnimation()
    {
        string baseText = "로딩 중";
        int dotCount = 0;

        while (true)
        {
            loadingText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // 0, 1, 2, 3 순으로 반복
            yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 변경
        }
    }
}
