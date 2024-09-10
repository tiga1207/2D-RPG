using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviourPunCallbacks
{
    public static EnemyManager Instance;
    [Header("Enemy Settings")]
    public List<Transform> spawnPoints; // 적을 스폰할 위치 목록
    public GameObject enemyPrefab; // 적 프리팹

    private List<GameObject> enemies = new List<GameObject>();


    void Awake()
    {
       // Singleton 패턴 구현
       if (Instance == null)
       {
           Instance = this;
           DontDestroyOnLoad(gameObject);
       }
       else
       {
           Destroy(gameObject);
       }
    }
    void Start()
    {
        //적 중복 생성 제거를 위해 주석처리
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     SpawnEnemies();
        // }
    }

    public void SpawnEnemies()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null) // 스폰 포인트가 null이 아닌지 확인
            {
                GameObject enemy = PhotonNetwork.Instantiate(enemyPrefab.name, spawnPoint.position, Quaternion.identity);
                enemies.Add(enemy);
            }
        }
    }

    public void RespawnEnemy(Vector3 position)
    {
        StartCoroutine(RespawnEnemyCoroutine(position));
    }

    IEnumerator RespawnEnemyCoroutine(Vector3 position)
    {
        yield return new WaitForSeconds(5f);
        GameObject enemy = PhotonNetwork.Instantiate(enemyPrefab.name, position, Quaternion.identity);
        enemies.Add(enemy);
    }
}

