using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviourPunCallbacks
{
    public static EnemyManager Instance;
    [Header("Enemy Settings")]
    public List<Transform> skeletonSpawnPoints; // 적을 스폰할 위치 목록
    public GameObject skeletonPrefab; // 적 프리팹

    public List<Transform> samuraiSpawnPoints; // 적을 스폰할 위치 목록
    public GameObject samuraiPrefab; // 적 프리팹
    public GameObject bossPrefab; // 적 프리팹
    public Transform bossSpawnPoint;

    private List<GameObject> bossList = new List<GameObject>();
    

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
        // 적 중복 생성 제거를 위해 주석처리
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnBoss();
        }
    }

    public void SpawnEnemies()
    {
        foreach (Transform skeletonSpawnPoint in skeletonSpawnPoints)
        {
            if (skeletonSpawnPoint != null) // 스폰 포인트가 null이 아닌지 확인
            {
                GameObject enemy = PhotonNetwork.Instantiate(skeletonPrefab.name, skeletonSpawnPoint.position, Quaternion.identity);
                enemies.Add(enemy);
            }
        }

        foreach (Transform samuraiSpawnPoint in samuraiSpawnPoints)
        {
            if (samuraiSpawnPoint != null) // 스폰 포인트가 null이 아닌지 확인
            {
                GameObject samurai = PhotonNetwork.Instantiate(samuraiPrefab.name, samuraiSpawnPoint.position, Quaternion.identity);
                enemies.Add(samurai);
            }
        }
    }

    //보스 스폰 로직
    public void SpawnBoss()
    {
        if(bossSpawnPoint != null)
        {
            GameObject boss = PhotonNetwork.Instantiate(bossPrefab.name, bossSpawnPoint.position, Quaternion.identity);
            bossList.Add(boss);
        }
    }

    public void RespawnSkeleton(Vector3 position)
    {
        StartCoroutine(RespawnSkeletonCoroutine(position));
    }
    public void RespawnSamurai(Vector3 position)
    {
        StartCoroutine(RespawnSamuraiCoroutine(position));
    }
    public void RespawnEnemy(Vector3 position)
    {
        StartCoroutine(RespawnEnemyCoroutine(position));
    }
    public void RespawnBoss(Vector3 position)
    {
        StartCoroutine(RespawnBossCoroutine(position));
    }

    IEnumerator RespawnSkeletonCoroutine(Vector3 position)
    {
        yield return new WaitForSeconds(5f); //리스폰 소요 시간
        GameObject enemy = PhotonNetwork.Instantiate(skeletonPrefab.name, position, Quaternion.identity);
        enemies.Add(enemy);

    }
    IEnumerator RespawnSamuraiCoroutine(Vector3 position)
    {
        yield return new WaitForSeconds(5f); //리스폰 소요 시간

        GameObject samurai = PhotonNetwork.Instantiate(samuraiPrefab.name, position, Quaternion.identity);
        enemies.Add(samurai);

    }
    IEnumerator RespawnEnemyCoroutine(Vector3 position)
    {
        yield return new WaitForSeconds(5f); //리스폰 소요 시간
        // GameObject enemy = PhotonNetwork.Instantiate(skeletonPrefab.name, position, Quaternion.identity);
        // enemies.Add(enemy);
        // GameObject samurai = PhotonNetwork.Instantiate(samuraiPrefab.name, position, Quaternion.identity);
        // enemies.Add(samurai);

    }
     IEnumerator RespawnBossCoroutine(Vector3 position)
    {
        yield return new WaitForSeconds(50f); //보스 리스폰 소요 시간
        GameObject boss = PhotonNetwork.Instantiate(bossPrefab.name, bossSpawnPoint.position, Quaternion.identity);
        bossList.Add(boss);
    }
}

