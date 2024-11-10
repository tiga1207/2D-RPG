using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public PolygonCollider2D leftZone;
    public PolygonCollider2D rightZone;
    public Transform zoneTransform;

    private CinemachineConfiner confiner;

    void Start()
    {
        // CinemachineConfiner2D 컴포넌트를 가져옵니다.
        confiner = virtualCamera.GetComponent<CinemachineConfiner>();
    }

    void Update()
    {
        // 로컬 플레이어가 있는지 확인하고 본인 캐릭터인지 확인합니다.
        Player player = Player.LocalPlayerInstance;

        if (player != null && player.GetComponent<PhotonView>().IsMine)
        {
            // 플레이어 위치에 따라 적절한 Bounding Shape를 설정합니다.
            if (player.transform.position.x < zoneTransform.position.x) // 왼쪽 구역에 있을 때
            {
                confiner.m_BoundingShape2D = leftZone;
            }
            else // 오른쪽 구역에 있을 때
            {
                confiner.m_BoundingShape2D = rightZone;
            }
        }
    }
}