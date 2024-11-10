using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitFirstCamera : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineCamera;
    public SpriteRenderer mapSpriteRenderer; // 카메라 위치

    void Start()
    {
        Player player = Player.LocalPlayerInstance;

        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
        }
        else
        {
            CamearaInit();
        }

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

}
