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
    public GameObject UserUI;
    // public GameObject UserStatUI;

    public GameObject UserSkillUi;
    public GameObject userRespawn;
    public Button RespawnButton;
    public TMP_Text loadingText;
    private bool isSceneTransitioning = false;

    private EnemyManager enemyManagerInstance; // EnemyManager 인스턴스 참조

    private ItemDataBase itemDataBaseInstance;
    private Coroutine loadingCoroutine;

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.AutomaticallySyncScene = true; // 자동 씬 동기화 설정


        // 씬에서 EnemyManager 오브젝트 찾기
        enemyManagerInstance = FindObjectOfType<EnemyManager>();
        itemDataBaseInstance= FindObjectOfType<ItemDataBase>();
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
        UserUI.SetActive(true);
        UserSkillUi.SetActive(true);
        

        if(PhotonNetwork.IsMasterClient)  // 마스터 클라이언트가 적 스폰 및 아이템데이터베이스 관리
        {
            if(enemyManagerInstance != null)
            {
                enemyManagerInstance.SpawnEnemies();
            }
            if (itemDataBaseInstance != null)
            {
                // 마스터 클라이언트가 아이템 생성 관리
                itemDataBaseInstance.Initialize();
            }
        }

    }

    private void SpawnPlayer()
    {
        // GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f, 19f), 4, 0), Quaternion.identity);
        GameObject player = PhotonNetwork.Instantiate("Player", userRespawn.transform.position, Quaternion.identity);

        if(player.GetComponent<PhotonView>().IsMine)
        {
            player.GetComponent<Inventory>().Initialize();
        }
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
        OnJoinedRoom();
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


//싱글톤 구현
// // using Photon.Pun;
// // using Photon.Realtime;
// // using TMPro;
// // using UnityEngine;
// // using UnityEngine.UI;
// // using System.Collections;

// // public class NetworkManager : MonoBehaviourPunCallbacks
// // {
// //     public TMP_InputField NickNameInput;
// //     public GameObject DisconnectPanel;
// //     public GameObject RespawnPanel;
// //     public Button RespawnButton;
// //     public TMP_Text loadingText;

// //     private EnemyManager enemyManagerInstance; // EnemyManager 인스턴스 참조
// //     private ItemDataBase itemDataBaseInstance;
// //     private Coroutine loadingCoroutine;

// //     void Awake()
// //     {
// //         DontDestroyOnLoad(this.gameObject);
// //         Screen.SetResolution(960, 540, false);
// //         PhotonNetwork.SendRate = 60;
// //         PhotonNetwork.SerializationRate = 30;

// //         // 씬에서 EnemyManager 오브젝트 찾기
// //         enemyManagerInstance = FindObjectOfType<EnemyManager>();
// //         itemDataBaseInstance = FindObjectOfType<ItemDataBase>();
// //     }

// //     private void Start()
// //     {
// //         if (RespawnButton != null)
// //         {
// //             RespawnButton.onClick.AddListener(OnRespawnButtonClicked);
// //         }

// //         if (loadingText != null)
// //         {
// //             loadingText.gameObject.SetActive(false);
// //         }
// //     }

// //     private void Update()
// //     {
// //         if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
// //         {
// //             PhotonNetwork.Disconnect();
// //         }
// //     }

// //     public void Connect()
// //     {
// //         if (loadingText != null)
// //         {
// //             loadingText.gameObject.SetActive(true);
// //             if (loadingCoroutine == null)
// //             {
// //                 loadingCoroutine = StartCoroutine(LoadingAnimation());
// //             }
// //         }

// //         if (PhotonNetwork.IsConnected)
// //         {
// //             if (PhotonNetwork.InRoom)
// //             {
// //                 PhotonNetwork.LeaveRoom();
// //             }
// //             else
// //             {
// //                 PhotonNetwork.ReconnectAndRejoin();
// //             }
// //         }
// //         else
// //         {
// //             PhotonNetwork.ConnectUsingSettings();
// //         }
// //     }

// //     public override void OnConnectedToMaster()
// //     {
// //         string nickName = NickNameInput.text;
// //         PhotonNetwork.LocalPlayer.NickName = nickName;
// //         PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
// //     }

// //     public override void OnJoinedRoom()
// //     {
// //         DisconnectPanel.SetActive(false);
// //         RespawnPanel.SetActive(false);
// //         if (loadingText != null)
// //         {
// //             loadingText.gameObject.SetActive(false);
// //             if (loadingCoroutine != null)
// //             {
// //                 StopCoroutine(loadingCoroutine);
// //                 loadingCoroutine = null;
// //             }
// //         }
// //         SpawnPlayer();

// //         if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트가 적 스폰 및 아이템데이터베이스 관리
// //         {
// //             if (enemyManagerInstance != null)
// //             {
// //                 enemyManagerInstance.SpawnEnemies();
// //             }
// //             if (itemDataBaseInstance != null)
// //             {
// //                 // 마스터 클라이언트가 아이템 생성 관리
// //                 itemDataBaseInstance.Initialize();
// //             }
// //         }
// //     }

// //     private void SpawnPlayer()
// //     {
// //         GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f, 19f), 4, 0), Quaternion.identity);
// //         if (player.GetComponent<PhotonView>().IsMine)
// //         {
// //             player.GetComponent<Inventory>().Initialize();
// //         }
// //     }

// //     public override void OnLeftRoom()
// //     {
// //         if (PhotonNetwork.IsConnected)
// //         {
// //             PhotonNetwork.JoinLobby();
// //         }
// //         else
// //         {
// //             PhotonNetwork.ConnectUsingSettings();
// //         }
// //     }

// //     public override void OnDisconnected(DisconnectCause cause)
// //     {
// //         base.OnDisconnected(cause);

// //         if (DisconnectPanel != null)
// //         {
// //             DisconnectPanel.SetActive(true);
// //         }

// //         if (RespawnPanel != null)
// //         {
// //             RespawnPanel.SetActive(false);
// //         }

// //         if (loadingText != null)
// //         {
// //             loadingText.gameObject.SetActive(false);
// //             if (loadingCoroutine != null)
// //             {
// //                 StopCoroutine(loadingCoroutine);
// //                 loadingCoroutine = null;
// //             }
// //         }
// //     }

// //     private void OnRespawnButtonClicked()
// //     {
// //         OnJoinedRoom();
// //     }

// //     private IEnumerator LoadingAnimation()
// //     {
// //         string baseText = "로딩 중";
// //         int dotCount = 0;

// //         while (true)
// //         {
// //             loadingText.text = baseText + new string('.', dotCount);
// //             dotCount = (dotCount + 1) % 4; // 0, 1, 2, 3 순으로 반복
// //             yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 변경
// //         }
// //     }
// // }


// // using Photon.Pun;
// // using Photon.Realtime;
// // using TMPro;
// // using UnityEngine;
// // using UnityEngine.UI;
// // using System.Collections;

// // public class NetworkManager : MonoBehaviourPunCallbacks
// // {
// //     public static NetworkManager Instance { get; private set; } // 싱글톤 인스턴스

// //     public TMP_InputField NickNameInput;
// //     public GameObject DisconnectPanel;
// //     public GameObject RespawnPanel;
// //     public Button RespawnButton;
// //     public TMP_Text loadingText;

// //     private EnemyManager enemyManagerInstance; // EnemyManager 인스턴스 참조
// //     private ItemDataBase itemDataBaseInstance;
// //     private Coroutine loadingCoroutine;

// //     void Awake()
// //     {
// //         // 싱글톤 설정
// //         if (Instance != null && Instance != this)
// //         {
// //             Destroy(this.gameObject);
// //             return;
// //         }
// //         Instance = this;
// //         DontDestroyOnLoad(this.gameObject);

// //         Screen.SetResolution(960, 540, false);
// //         PhotonNetwork.SendRate = 60;
// //         PhotonNetwork.SerializationRate = 30;

// //         // 씬에서 EnemyManager 오브젝트 찾기
// //         enemyManagerInstance = FindObjectOfType<EnemyManager>();
// //         itemDataBaseInstance = FindObjectOfType<ItemDataBase>();
// //     }

// //     private void Start()
// //     {
// //         if (RespawnButton != null)
// //         {
// //             RespawnButton.onClick.AddListener(OnRespawnButtonClicked);
// //         }

// //         if (loadingText != null)
// //         {
// //             loadingText.gameObject.SetActive(false);
// //         }
// //     }

// //     private void Update()
// //     {
// //         if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
// //         {
// //             PhotonNetwork.Disconnect();
// //         }
// //     }

// //     public void Connect()
// //     {
// //         if (loadingText != null)
// //         {
// //             loadingText.gameObject.SetActive(true);
// //             if (loadingCoroutine == null)
// //             {
// //                 loadingCoroutine = StartCoroutine(LoadingAnimation());
// //             }
// //         }

// //         if (PhotonNetwork.IsConnected)
// //         {
// //             if (PhotonNetwork.InRoom)
// //             {
// //                 PhotonNetwork.LeaveRoom();
// //             }
// //             else
// //             {
// //                 PhotonNetwork.ReconnectAndRejoin();
// //             }
// //         }
// //         else
// //         {
// //             PhotonNetwork.ConnectUsingSettings();
// //         }
// //     }

// //     public override void OnConnectedToMaster()
// //     {
// //         string nickName = NickNameInput.text;
// //         PhotonNetwork.LocalPlayer.NickName = nickName;
// //         PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
// //     }

// //     public override void OnJoinedRoom()
// //     {
// //         DisconnectPanel.SetActive(false);
// //         RespawnPanel.SetActive(false);
// //         if (loadingText != null)
// //         {
// //             loadingText.gameObject.SetActive(false);
// //             if (loadingCoroutine != null)
// //             {
// //                 StopCoroutine(loadingCoroutine);
// //                 loadingCoroutine = null;
// //             }
// //         }
// //         SpawnPlayer();

// //         if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트가 적 스폰 및 아이템데이터베이스 관리
// //         {
// //             if (enemyManagerInstance != null)
// //             {
// //                 enemyManagerInstance.SpawnEnemies();
// //             }
// //             if (itemDataBaseInstance != null)
// //             {
// //                 // 마스터 클라이언트가 아이템 생성 관리
// //                 itemDataBaseInstance.Initialize();
// //             }
// //         }
// //     }

// //     private void SpawnPlayer()
// //     {
// //         GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f, 19f), 4, 0), Quaternion.identity);
// //         if (player.GetComponent<PhotonView>().IsMine)
// //         {
// //             player.GetComponent<Inventory>().Initialize();
// //         }
// //     }

// //     public override void OnLeftRoom()
// //     {
// //         if (PhotonNetwork.IsConnected)
// //         {
// //             PhotonNetwork.JoinLobby();
// //         }
// //         else
// //         {
// //             PhotonNetwork.ConnectUsingSettings();
// //         }
// //     }

// //     public override void OnDisconnected(DisconnectCause cause)
// //     {
// //         base.OnDisconnected(cause);

// //         if (DisconnectPanel != null)
// //         {
// //             DisconnectPanel.SetActive(true);
// //         }

// //         if (RespawnPanel != null)
// //         {
// //             RespawnPanel.SetActive(false);
// //         }

// //         if (loadingText != null)
// //         {
// //             loadingText.gameObject.SetActive(false);
// //             if (loadingCoroutine != null)
// //             {
// //                 StopCoroutine(loadingCoroutine);
// //                 loadingCoroutine = null;
// //             }
// //         }
// //     }

// //     private void OnRespawnButtonClicked()
// //     {
// //         OnJoinedRoom();
// //     }

// //     private IEnumerator LoadingAnimation()
// //     {
// //         string baseText = "로딩 중";
// //         int dotCount = 0;

// //         while (true)
// //         {
// //             loadingText.text = baseText + new string('.', dotCount);
// //             dotCount = (dotCount + 1) % 4; // 0, 1, 2, 3 순으로 반복
// //             yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 변경
// //         }
// //     }
// // }

// 디스커넥트 문제 해결
// using Photon.Pun;
// using Photon.Realtime;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
// using System.Collections;

// public class NetworkManager : MonoBehaviourPunCallbacks
// {
//     public TMP_InputField NickNameInput;
//     public GameObject DisconnectPanel;
//     public GameObject RespawnPanel;
//     public Button RespawnButton;
//     public TMP_Text loadingText;

//     private EnemyManager enemyManagerInstance;
//     private ItemDataBase itemDataBaseInstance;
//     private Coroutine loadingCoroutine;
//     private bool isSceneTransitioning = false;

//     void Awake()
//     {
//         DontDestroyOnLoad(this.gameObject);

//         Screen.SetResolution(960, 540, false);
//         PhotonNetwork.SendRate = 60;
//         PhotonNetwork.SerializationRate = 30;

//         enemyManagerInstance = FindObjectOfType<EnemyManager>();
//         itemDataBaseInstance = FindObjectOfType<ItemDataBase>();

//         SceneManager.sceneLoaded += OnSceneLoaded;

//         if (DisconnectPanel != null)
//         {
//             DisconnectPanel.SetActive(true); // Ensure it's active on start
//         }
//     }

//     private void Start()
//     {
//         if (RespawnButton != null)
//         {
//             RespawnButton.onClick.AddListener(OnRespawnButtonClicked);
//         }

//         if (loadingText != null)
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

//     public void Connect()
//     {
//         if (loadingText != null)
//         {
//             loadingText.gameObject.SetActive(true);
//             if (loadingCoroutine == null)
//             {
//                 loadingCoroutine = StartCoroutine(LoadingAnimation());
//             }
//         }

//         if (PhotonNetwork.IsConnected)
//         {
//             if (PhotonNetwork.InRoom)
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
//         if (!isSceneTransitioning)
//         {
//             DisconnectPanel.SetActive(false);
//         }
//         RespawnPanel.SetActive(false);
//         if (loadingText != null)
//         {
//             loadingText.gameObject.SetActive(false);
//             if (loadingCoroutine != null)
//             {
//                 StopCoroutine(loadingCoroutine);
//                 loadingCoroutine = null;
//             }
//         }
//         SpawnPlayer();

//         if (PhotonNetwork.IsMasterClient)
//         {
//             if (enemyManagerInstance != null)
//             {
//                 enemyManagerInstance.SpawnEnemies();
//             }
//             if (itemDataBaseInstance != null)
//             {
//                 itemDataBaseInstance.Initialize();
//             }
//         }
//     }

//     private void SpawnPlayer()
//     {
//         GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f, 19f), 4, 0), Quaternion.identity);
//         if (player.GetComponent<PhotonView>().IsMine)
//         {
//             player.GetComponent<Inventory>().Initialize();
//         }
//     }

//     public override void OnLeftRoom()
//     {
//         if (PhotonNetwork.IsConnected)
//         {
//             PhotonNetwork.JoinLobby();
//         }
//         else
//         {
//             PhotonNetwork.ConnectUsingSettings();
//         }
//     }

//     public override void OnDisconnected(DisconnectCause cause)
//     {
//         base.OnDisconnected(cause);

//         if (!isSceneTransitioning)
//         {
//             if (DisconnectPanel != null)
//             {
//                 DisconnectPanel.SetActive(true);
//             }

//             if (RespawnPanel != null)
//             {
//                 RespawnPanel.SetActive(false);
//             }

//             if (loadingText != null)
//             {
//                 loadingText.gameObject.SetActive(false);
//                 if (loadingCoroutine != null)
//                 {
//                     StopCoroutine(loadingCoroutine);
//                     loadingCoroutine = null;
//                 }
//             }
//         }
//     }

//     private void OnRespawnButtonClicked()
//     {
//         OnJoinedRoom();
//     }

//     private IEnumerator LoadingAnimation()
//     {
//         string baseText = "로딩 중";
//         int dotCount = 0;

//         while (true)
//         {
//             loadingText.text = baseText + new string('.', dotCount);
//             dotCount = (dotCount + 1) % 4;
//             yield return new WaitForSeconds(0.5f);
//         }
//     }

//     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {
//         isSceneTransitioning = false;

//         if (DisconnectPanel != null)
//         {
//             DisconnectPanel.SetActive(false);
//         }

//         if (RespawnPanel != null)
//         {
//             RespawnPanel.SetActive(false);
//         }

//         if (loadingText != null)
//         {
//             loadingText.gameObject.SetActive(false);
//             if (loadingCoroutine != null)
//             {
//                 StopCoroutine(loadingCoroutine);
//                 loadingCoroutine = null;
//             }
//         }
//     }

//     private void OnDestroy()
//     {
//         SceneManager.sceneLoaded -= OnSceneLoaded;
//     }

//     public void LoadScene(string sceneName)
//     {
//         isSceneTransitioning = true;
//         SceneManager.LoadScene(sceneName);
//     }
// }
