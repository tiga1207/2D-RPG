using Photon.Pun;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public float damage = 10f; // 수리검 데미지

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 플레이어에게 데미지 주기
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage); // 데미지 처리
            }

            // 수리검 파괴
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
