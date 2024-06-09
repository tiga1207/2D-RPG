using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField NickNameInput;
    public GameObject DisconnectPanel;
    public GameObject RespawnPanel;

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        DisconnectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
    }
}
