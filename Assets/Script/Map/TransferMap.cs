using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{
    //public List<Transform> transformList;
    //public CinemachineVirtualCamera cinemachineCamera;
    //public Transform teleportCameraTransform; // 카메라 위치
    //public SpriteRenderer mapSpriteRenderer; // 카메라 위치
    
    public Transform teleportTransform; //플레이어 텔포 위치
    private bool isPlayerNearbyTeleportZone = false; // 플레이어가 텔레포트 지역 근처에 있는지 여부
    public GameObject teleportPanel;
    //public GameObject teleportDesc;
    private Coroutine loadingCoroutine;
    public TMP_Text loadingText;

    void Start()
    {
        Player player = Player.LocalPlayerInstance;

        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
        }
    }


    void Update()
    {
        if (isPlayerNearbyTeleportZone && Input.GetKeyDown(KeyCode.F) ) // F키 눌렀을 때
        {
            StartCoroutine(teleportPanelOn());
            TeleportPlayer();
            Debug.Log("F키 누름");
        }
    }

    public void TeleportPlayer()
    {
        Player player = Player.LocalPlayerInstance;
        if (player != null && player.GetComponent<PhotonView>().IsMine)
        {
            player.transform.position = teleportTransform.position;
            //CamearaInit();

            //if (cinemachineCamera != null && teleportCameraTransform != null)
            //{
            //    cinemachineCamera.Follow = null;
            //    cinemachineCamera.LookAt = null;
            //    cinemachineCamera.transform.position = new Vector3(teleportCameraTransform.position.x, teleportCameraTransform.position.y, cinemachineCamera.transform.position.z);
            //}
        }
    }

    //private void CamearaInit()
    //{
    //    if (cinemachineCamera != null && mapSpriteRenderer != null)
    //    {
    //        // 스프라이트렌더러의 bounds를 이용해 크기 및 중심 위치 가져오기
    //        float objectWidth = mapSpriteRenderer.bounds.size.x;
    //        float objectHeight = mapSpriteRenderer.bounds.size.y;
    //        Vector3 objectCenter = mapSpriteRenderer.bounds.center;

    //        // 화면 비율과 오브젝트 비율 계산
    //        float screenRatio = (float)Screen.width / Screen.height;
    //        float targetRatio = objectWidth / objectHeight;

    //        // 카메라의 Orthographic Size 설정
    //        if (screenRatio >= targetRatio)
    //        {
    //            cinemachineCamera.m_Lens.OrthographicSize = objectHeight / 2;
    //        }
    //        else
    //        {
    //            float differenceInSize = targetRatio / screenRatio;
    //            cinemachineCamera.m_Lens.OrthographicSize = objectHeight / 2 * differenceInSize;
    //        }

    //        // 카메라 위치를 스프라이트 중심에 맞춤
    //        cinemachineCamera.transform.position = new Vector3(objectCenter.x, objectCenter.y, cinemachineCamera.transform.position.z);
    //    }
    //}
    IEnumerator teleportPanelOn()
    {
        Player player = Player.LocalPlayerInstance;
        if (player != null && player.GetComponent<PhotonView>().IsMine)
        {
            player.isTeleporting = true;
            player.invincible = true;
            teleportPanel.SetActive(true);
            //teleportDesc.SetActive(false);
            TeleportPlayer();
            if (loadingText != null)
            {
                loadingText.gameObject.SetActive(true);
                if (loadingCoroutine == null)
                {
                    loadingCoroutine = StartCoroutine(LoadingAnimation());
                }
            }
            yield return new WaitForSeconds(3);
            player.isTeleporting = false;
            player.invincible = false;
            teleportPanel.SetActive(false);
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearbyTeleportZone = true;
            //teleportDesc.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearbyTeleportZone = false;
            //teleportDesc.SetActive(false);
        }
    }

}
