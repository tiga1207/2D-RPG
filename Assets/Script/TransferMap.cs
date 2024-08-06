using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{
    public string TransferMapName;
    private Player player;

    void Start()
    {
        if (player != null)
        {
            player = FindObjectOfType<Player>();
            Debug.LogError("Player object not found in the scene!");
        }
    }

        private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
         {
            SceneManager.LoadScene(TransferMapName);
            //PhotonNetwork.LoadLevel(TransferMapName); 플레이어 전부 씬 이동시
            player = FindObjectOfType<Player>(); 
            if(player != null)
            {
                player.currentMapName = TransferMapName;
            }
         }
    }

}
