using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField NickNameInput;
    public GameObject DisconnectPanel;
    public GameObject RespawnPanel;

    public Button RespawnButton;

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        // RespawnButton.onClick.AddListener(onRespawnButtonClicked);
        // RespawnButton.onClick.AddListener(Connect);
    }
    private void Start()
    {
        if(RespawnButton != null)
        {
            RespawnButton.onClick.AddListener(Connect);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    // public void Connect() => PhotonNetwork.ConnectUsingSettings();

    // public void Connect()
    // {
    //     if(PhotonNetwork.IsConnected)
    //     {
    //         PhotonNetwork.LeaveRoom();
    //     }
    //     else
    //     {
    //         PhotonNetwork.ConnectUsingSettings();
    //     }
    // }
        public void Connect()
    {
        if(PhotonNetwork.IsConnected)
        {
            if(PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                PhotonNetwork.ReconnectAndRejoin();
            }
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
            
    }

    public override void OnConnectedToMaster()
    {
        string nickName = NickNameInput.text;
        PhotonNetwork.LocalPlayer.NickName = nickName;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {
        DisconnectPanel.SetActive(false);
        RespawnPanel.SetActive(false);
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f, 19f), 4, 0), Quaternion.identity);
        GameObject enemy =  PhotonNetwork.Instantiate("Enemy_Skeleton", new Vector3(Random.Range(-6f, 19f), 4, 0), Quaternion.identity);
        // if (player != null)
        // {
        //     Debug.Log("Player object instantiated successfully.");
        // }
        // else
        // {
        //     Debug.LogError("Player object not instantiated.");
        // }
    }

    public override void OnLeftRoom()
    {
        // Connect();
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
        else{
            PhotonNetwork.ConnectUsingSettings();
        }

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        if(DisconnectPanel !=null)
        {
            DisconnectPanel.SetActive(true);
        }
        if(RespawnPanel != null)
        {
            RespawnPanel.SetActive(false);
        }
    }

    // public void onRespawnButtonClicked(){
    //     Debug.Log("리스폰 버튼 클릭됨");

    //     Connect();
    // }
}
