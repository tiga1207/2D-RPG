using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour
{
    public GameObject respawnPanel;
    public static PlayerRespawn Instance;
    public Button respawnBtn;
    public CinemachineVirtualCamera cinemachineCamera;
    public SpriteRenderer mapSpriteRenderer; // 카메라 위치


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
        //if (respawnBtn != null)
        //{
        //    respawnBtn.onClick.AddListener(RespawnPlayer);
        //}
    }

    private void CamearaInit()
    {
        if (cinemachineCamera != null && mapSpriteRenderer != null)
        {
            // 스프라이트렌더러의 bounds를 이용해 크기 및 중심 위치 가져오기
            float objectWidth = mapSpriteRenderer.bounds.size.x;
            float objectHeight = mapSpriteRenderer.bounds.size.y;
            Vector3 objectCenter = mapSpriteRenderer.bounds.center;

            // 화면 비율과 오브젝트 비율 계산
            float screenRatio = (float)Screen.width / Screen.height;
            float targetRatio = objectWidth / objectHeight;

            // 카메라의 Orthographic Size 설정
            if (screenRatio >= targetRatio)
            {
                cinemachineCamera.m_Lens.OrthographicSize = objectHeight / 2;
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                cinemachineCamera.m_Lens.OrthographicSize = objectHeight / 2 * differenceInSize;
            }

            // 카메라 위치를 스프라이트 중심에 맞춤
            cinemachineCamera.transform.position = new Vector3(objectCenter.x, objectCenter.y, cinemachineCamera.transform.position.z);
        }
    }

    public void RespawnPlayer()
    {

        if (Player.LocalPlayerInstance == null)
        {
            // 새로운 플레이어 객체 생성
            GameObject playerPrefab = PhotonNetwork.Instantiate("Player", NetworkManager.Instance.userRespawn.transform.position, Quaternion.identity);
            if (playerPrefab.GetComponent<PhotonView>().IsMine)
            {
                Player.LocalPlayerInstance = playerPrefab.GetComponent<Player>();
                //playerPrefab.GetComponent<Player>().Hp = playerPrefab.GetComponent<Player>().MaxHp / 2f; // 체력의 절반으로 리스폰
                playerPrefab.GetComponentInChildren<TMP_Text>().text = PhotonNetwork.NickName;
                Player.LocalPlayerInstance.userId = NetworkManager.Instance.MakeUserId(PhotonNetwork.NickName);

                // 필요시 추가 초기화 작업
                //playerPrefab.GetComponent<Inventory>().Initialize();
            }
        }
        respawnPanel.SetActive(false);
        //Debug.Log("리스폰 버튼 눌림");
        //respawnPanel.SetActive(false);

        //Player player = Player.LocalPlayerInstance;
        //if(player != null)
        //{
        //    player.transform.position = NetworkManager.Instance.userRespawn.transform.position; // 리스폰 위치로 이동
        //    player.Hp=player.MaxHp/2f; // 플레이어의 최대체력의 반 회복시키고 부활
        //    if(player.Exp > player.MaxExp/5f)// 사망시 플레이어 경험치 10퍼 삭제.
        //    {
        //        player.Exp-= player.MaxExp/5f;
        //    }
        //    else
        //    {
        //        player.Exp = 0;
        //    }
        //    player.gameObject.SetActive(true); //플레이어 활성화
        //}
        //CamearaInit();

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
