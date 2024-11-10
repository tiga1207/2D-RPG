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
        // CinemachineConfiner2D ������Ʈ�� �����ɴϴ�.
        confiner = virtualCamera.GetComponent<CinemachineConfiner>();
    }

    void Update()
    {
        // ���� �÷��̾ �ִ��� Ȯ���ϰ� ���� ĳ�������� Ȯ���մϴ�.
        Player player = Player.LocalPlayerInstance;

        if (player != null && player.GetComponent<PhotonView>().IsMine)
        {
            // �÷��̾� ��ġ�� ���� ������ Bounding Shape�� �����մϴ�.
            if (player.transform.position.x < zoneTransform.position.x) // ���� ������ ���� ��
            {
                confiner.m_BoundingShape2D = leftZone;
            }
            else // ������ ������ ���� ��
            {
                confiner.m_BoundingShape2D = rightZone;
            }
        }
    }
}