using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviourPunCallbacks
{
    [Header("Enemy Settings")]
    public List<Transform> spawnPoints; // 적을 스폰할 위치 목록
    public GameObject enemyPrefab; // 적 프리팹

    private List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnEnemies();
        }
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

