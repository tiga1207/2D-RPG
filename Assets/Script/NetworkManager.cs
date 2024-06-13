// using System.Collections;
// using Photon.Pun;
// using Photon.Realtime;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// public class NetworkManager : MonoBehaviourPunCallbacks
// {
//     public TMP_InputField NickNameInput;
//     public GameObject DisconnectPanel;
//     public GameObject RespawnPanel;
//     public Button RespawnButton;
//     public GameObject enemyManagerPrefab;
//     public TMP_Text loadingText;
//     private EnemyManager enemyManagerInstance;
//     private Coroutine loadingCoroutine;

//     void Awake()
//     {
//         Screen.SetResolution(960, 540, false);
//         PhotonNetwork.SendRate = 60;
//         PhotonNetwork.SerializationRate = 30;

//         // RespawnButton.onClick.AddListener(onRespawnButtonClicked);
//         // RespawnButton.onClick.AddListener(Connect);
//         enemyManagerInstance = FindObjectOfType<EnemyManager>();
//     }
//     private void Start()
//     {
//         if(RespawnButton != null)
//         {
//             RespawnButton.onClick.AddListener(Connect);
//         }

//         if(loadingText != null)
//         {
//             loadingText.gameObject.SetActive(false);
//         }
//     }

//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
//         {
//             PhotonNetwork.Disconnect();
//         }
//     }

//     // public void Connect() => PhotonNetwork.ConnectUsingSettings();

//     // public void Connect()
//     // {
//     //     if(PhotonNetwork.IsConnected)
//     //     {
//     //         PhotonNetwork.LeaveRoom();
//     //     }
//     //     else
//     //     {
//     //         PhotonNetwork.ConnectUsingSettings();
//     //     }
//     // }
//         public void Connect()
//     {
//         if(loadingText != null)
//         {
//             loadingText.gameObject.SetActive(true);
//             if(loadingCoroutine == null)
//             {
//                 loadingCoroutine = StartCoroutine(loadingAnimation());
//             }
//         }
//         if(PhotonNetwork.IsConnected)
//         {
//             if(PhotonNetwork.InRoom)
//             {
//                 PhotonNetwork.LeaveRoom();
//             }
//             else
//             {
//                 PhotonNetwork.ReconnectAndRejoin();
//             }
//         }
//         else
//         {
//             PhotonNetwork.ConnectUsingSettings();
//         }
            
//     }

//     public override void OnConnectedToMaster()
//     {
//         string nickName = NickNameInput.text;
//         PhotonNetwork.LocalPlayer.NickName = nickName;
//         PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
//     }

//     public override void OnJoinedRoom()
//     {
//         DisconnectPanel.SetActive(false);
//         RespawnPanel.SetActive(false);
//         if (loadingText != null)
//         {
//             loadingText.gameObject.SetActive(false);
//         }
//         SpawnPlayer();
//         if (PhotonNetwork.IsMasterClient && enemyManagerInstance != null)
//         {
//             // enemyManagerInstance = PhotonNetwork.Instantiate(enemyManagerPrefab.name,Vector3.zero, Quaternion.identity);
//             // enemyManagerInstance = Instantiate(enemyManagerPrefab, Vector3.zero, Quaternion.identity);
//             enemyManagerInstance.SpawnEnemies();

//         }
//     }

//     private void SpawnPlayer()
//     {
//         GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f, 19f), 4, 0), Quaternion.identity);
//         // GameObject enemy =  PhotonNetwork.Instantiate("Enemy_Skeleton", new Vector3(Random.Range(-6f, 19f), 4, 0), Quaternion.identity);
//         // if (player != null)
//         // {
//         //     Debug.Log("Player object instantiated successfully.");
//         // }
//         // else
//         // {
//         //     Debug.LogError("Player object not instantiated.");
//         // }
//     }

//     public override void OnLeftRoom()
//     {
//         // Connect();
//         if(PhotonNetwork.IsConnected)
//         {
//             PhotonNetwork.JoinLobby();
//         }
//         else{
//             PhotonNetwork.ConnectUsingSettings();
//         }

//     }
//     public override void OnDisconnected(DisconnectCause cause)
//     {
//         base.OnDisconnected(cause);
//         if(DisconnectPanel !=null)
//         {
//             DisconnectPanel.SetActive(true);
//         }
//         if(RespawnPanel != null)
//         {
//             RespawnPanel.SetActive(false);
//         }
//         if(loadingText != null)
//         {
//             loadingText.gameObject.SetActive(false);
//         }
//     }

//     private IEnumerator loadingAnimation()
//     {
//         string baseText = "Loading";
//         int dotCount = 0;

//         while (true)
//         {
//             loadingText.text = baseText + new string('.', dotCount);
//             dotCount = (dotCount + 1) % 4; // 0, 1, 2, 3 순으로 반복
//             yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 변경
//         }
//     }

//     // public void onRespawnButtonClicked(){
//     //     Debug.Log("리스폰 버튼 클릭됨");

//     //     Connect();
//     // }
// }

using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField NickNameInput;
    public GameObject DisconnectPanel;
    public GameObject RespawnPanel;
    public Button RespawnButton;
    public TMP_Text loadingText;

    private EnemyManager enemyManagerInstance; // EnemyManager 인스턴스 참조
    private Coroutine loadingCoroutine;

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        // 씬에서 EnemyManager 오브젝트 찾기
        enemyManagerInstance = FindObjectOfType<EnemyManager>();
    }

    private void Start()
    {
        if (RespawnButton != null)
        {
            RespawnButton.onClick.AddListener(OnRespawnButtonClicked);
        }

        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void Connect()
    {
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(true);
            if (loadingCoroutine == null)
            {
                loadingCoroutine = StartCoroutine(LoadingAnimation());
            }
        }

        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.InRoom)
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
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(false);
            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
                loadingCoroutine = null;
            }
        }
        SpawnPlayer();
        if (PhotonNetwork.IsMasterClient && enemyManagerInstance != null)
        {
            // 마스터 클라이언트가 적 스폰 관리
            enemyManagerInstance.SpawnEnemies();
        }
    }

    private void SpawnPlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f, 19f), 4, 0), Quaternion.identity);
    }

    public override void OnLeftRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        if (DisconnectPanel != null)
        {
            DisconnectPanel.SetActive(true);
        }

        if (RespawnPanel != null)
        {
            RespawnPanel.SetActive(false);
        }

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

    private void OnRespawnButtonClicked()
    {
        Connect();
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

