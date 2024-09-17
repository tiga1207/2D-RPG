using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{
    public string TransferMapName;  // 이동할 씬의 이름
    private Player player;

    void Start()
    {
        // 씬에서 Player 오브젝트를 찾습니다.
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
        }
    }


//     void Start()
//     {
//         if (player != null)
//         {
//             player = FindObjectOfType<Player>();
//             Debug.LogError("Player object not found in the scene!");
//         }
//     }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 트리거에 들어온 오브젝트가 Player이고 해당 클라이언트의 소유인 경우
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && photonView != null && photonView.IsMine)
        {
            // 씬을 전환합니다.
            SceneManager.LoadScene(TransferMapName);

            // 씬이 로드된 후 Player 오브젝트를 다시 찾습니다.
            player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.currentMapName = TransferMapName;
            }
            else
            {
                Debug.LogError("Player object not found after scene transition!");
            }
        }
    }

//         private void OnTriggerEnter2D(Collider2D other) {
//         if(other.CompareTag("Player"))
//          {
//             SceneManager.LoadScene(TransferMapName);
//             //PhotonNetwork.LoadLevel(TransferMapName); 플레이어 전부 씬 이동시
//             player = FindObjectOfType<Player>(); 
//             if(player != null)
//             {
//                 player.currentMapName = TransferMapName;
//             }
//          }
//     }

}
