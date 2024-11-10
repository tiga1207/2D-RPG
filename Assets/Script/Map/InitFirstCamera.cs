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
    public SpriteRenderer mapSpriteRenderer; // ī�޶� ��ġ

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
            // ��������Ʈ�������� bounds�� �̿��� ũ�� �� �߽� ��ġ ��������
            float objectWidth = mapSpriteRenderer.bounds.size.x;
            float objectHeight = mapSpriteRenderer.bounds.size.y;
            Vector3 objectCenter = mapSpriteRenderer.bounds.center;

            // ȭ�� ������ ������Ʈ ���� ���
            float screenRatio = (float)Screen.width / Screen.height;
            float targetRatio = objectWidth / objectHeight;

            // ī�޶��� Orthographic Size ����
            if (screenRatio >= targetRatio)
            {
                cinemachineCamera.m_Lens.OrthographicSize = objectHeight / 2;
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                cinemachineCamera.m_Lens.OrthographicSize = objectHeight / 2 * differenceInSize;
            }

            // ī�޶� ��ġ�� ��������Ʈ �߽ɿ� ����
            cinemachineCamera.transform.position = new Vector3(objectCenter.x, objectCenter.y, cinemachineCamera.transform.position.z);
        }
    }

}
