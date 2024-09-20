using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour
{
    public GameObject respawnPanel;
    public static PlayerRespawn Instance;
    public Button respawnBtn;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        NetworkManager networkManager = NetworkManager.Instance;
        respawnPanel.SetActive(false);
        if(respawnBtn != null)
        {
            respawnBtn.onClick.AddListener(RespawnPlayer);
        }
    }

    public void RespawnPlayer()
    {
        Debug.Log("리스폰 버튼 눌림");
        respawnPanel.SetActive(false);

        Player player = Player.LocalPlayerInstance;
        if(player != null)
        {
            player.transform.position = NetworkManager.Instance.userRespawn.transform.position; // 리스폰 위치로 이동
            player.Hp=player.MaxHp/2f; // 플레이어의 최대체력의 반 회복시키고 부활
            if(player.Exp > player.MaxExp/5f)// 사망시 플레이어 경험치 10퍼 삭제.
            {
                player.Exp-= player.MaxExp/5f;
            }
            else
            {
                player.Exp = 0;
            }
            player.gameObject.SetActive(true); //플레이어 활성화
        }

        // GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        // if (player != null)
        // {
        //     player.transform.position = NetworkManager.Instance.userRespawn.transform.position; // 리스폰 위치로 이동
        //     Player playerComponent = player.GetComponent<Player>();
        //     playerComponent.Hp = 10; // 체력을 10으로 회복
        //     player.SetActive(true); // 플레이어 재활성화
        // }


        // NetworkManager networkManager = NetworkManager.Instance;
        // networkManager.SpawnPlayer();

    }

    public void OnRespawnPanel()
    {
        respawnPanel.SetActive(true);
    }
}
