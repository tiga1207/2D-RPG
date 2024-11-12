using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    public GameObject RespawnPanel;
    public int userId;
    public GameObject UserUI;
    public GameObject UserSkillUi;
    public GameObject userRespawn;
    public Button RespawnButton;
    private bool isSceneTransitioning = false;
    public EnemyManager enemyManagerInstance;
    public ItemDataBase itemDataBaseInstance;
    // private Coroutine loadingCoroutine;

    public static NetworkManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        
        
        // 초기화 로직
        // Screen.SetResolution(960, 540, false);
        Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.AutomaticallySyncScene = false;

        // enemyManagerInstance = FindObjectOfType<EnemyManager>();
        // itemDataBaseInstance = FindObjectOfType<ItemDataBase>();
    }

    private void Start()
    {
        if (RespawnButton != null)
        {
            RespawnButton.onClick.AddListener(OnRespawnButtonClicked);
        }

        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }
    }

    private void Update()
    {
        //->esc 누를 시에 ui 띄우는 것으로 대체 예정.
        // if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        // {
        //     PhotonNetwork.Disconnect();
        // } 
    }

    public override void OnJoinedRoom()
    {
        RespawnPanel.SetActive(false);
        UserUI.SetActive(true);
        UserSkillUi.SetActive(true);

        SpawnPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            if (enemyManagerInstance != null)
            {
                enemyManagerInstance.SpawnEnemies();
            }
            if (itemDataBaseInstance != null)
            {
                itemDataBaseInstance.Initialize();
            }
        }
    }

    public int MakeUserId(string nickname)
    {
        int userId = 0;
        foreach (char c in nickname)
        {
            userId += (int)c;
        }
        return userId;
    }

    public void SpawnPlayer()
    {

        if (PhotonNetwork.LocalPlayer.TagObject != null)// 로컬플레이어가 중복된 캐릭터 조작 불가능하게.
        {
            return;
        }
        GameObject player = PhotonNetwork.Instantiate("Player", userRespawn.transform.position, Quaternion.identity);
        PhotonNetwork.LocalPlayer.TagObject = player;//


        if (player.GetComponent<PhotonView>().IsMine)
        {

            userId = MakeUserId(PhotonNetwork.NickName);
            player.GetComponent<Player>().userId = userId;


            player.GetComponent<Inventory>().Initialize();
            // 플레이어의 닉네임 표시
            player.GetComponentInChildren<TMP_Text>().text = PhotonNetwork.NickName;

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

        

        if (RespawnPanel != null)
        {
            RespawnPanel.SetActive(false);
        }
    }

    private void OnRespawnButtonClicked()
    {
        OnJoinedRoom();
    }

}
