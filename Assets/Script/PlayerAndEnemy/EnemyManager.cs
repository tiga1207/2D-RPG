using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;

public class EnemyManager : MonoBehaviourPunCallbacks
{
    public static EnemyManager Instance;
    [Header("Enemy Settings")]
    public List<Transform> skeletonSpawnPoints; // 적을 스폰할 위치 목록
    public GameObject skeletonPrefab; // 적 프리팹
    public GameObject bossTrigger; // 상호작용시 없애버리는 용도

    public List<Transform> samuraiSpawnPoints; // 적을 스폰할 위치 목록
    public GameObject samuraiPrefab; // 적 프리팹
    public GameObject bossPrefab; // 적 프리팹
    public Transform bossSpawnPoint;

    private List<GameObject> bossList = new List<GameObject>();
    

    private List<GameObject> enemies = new List<GameObject>();
    private bool isPlayerNearbyBossZone = false; // 플레이어가 보스 스폰 Collider 근처에 있는지 여부
    public GameObject bossSpawnPanel;
    public GameObject BossInteractionDesc;
    public int bossSpawnPanelCount=0;

    public GameObject bgmObject; 
    public AudioClip bossBgm; // 보스 등장 시 바뀔 브금
    private AudioSource bgmAudioSource;


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
        if (bgmObject != null)
        {
            bgmAudioSource = bgmObject.GetComponent<AudioSource>();
        }
        //적 중복 생성 제거를 위해 주석처리
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     SpawnEnemies();
        // }
        // 적 중복 생성 제거를 위해 주석처리
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    SpawnBoss();
        //}
    }
    void Update()
    {
        if (isPlayerNearbyBossZone && Input.GetKeyDown(KeyCode.F)&& bossSpawnPanelCount ==0) // F키 눌렀을 때
        {
            bossSpawnPanelCount++;
            StartCoroutine(BossSpawnPanelOn());
            //bossTrigger.SetActive(false);
            PhotonNetwork.Destroy(bossTrigger);

            Debug.Log("F키 누름");
        }
    }

    IEnumerator BossSpawnPanelOn()
    {
        bossSpawnPanel.SetActive(true);
        BossInteractionDesc.SetActive(false);
        BossBgmOn(); // 보스 소환클릭시 음악 변경
        CameraShaker.Instance.ShakeCamera(10f, 3f);// 카메라 흔들림 기능 추가.
        yield return new WaitForSeconds(3);
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnBoss();
        }
        bossSpawnPanel.SetActive(false);
    }
    //private void OnTriggerEnter2D(Collider2D other)
    public void PlayerEnteredBossZone()
    {
        //if (other.CompareTag("Player"))
        //{
            isPlayerNearbyBossZone = true;
            BossInteractionDesc.SetActive(true);
        //}
    }
    //private void OnTriggerExit2D(Collider2D other)
        public void PlayerExitedBossZone()
        {
            //if (other.CompareTag("Player"))
            //{
                isPlayerNearbyBossZone = false;
                BossInteractionDesc.SetActive(false);
            //}
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

    private void BossBgmOn()
    {
        if (bgmAudioSource != null && bossBgm != null)
        {
            bgmAudioSource.clip = bossBgm;
            bgmAudioSource.Play(); // 보스 음악 재생
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

    //현재 보스 리스폰은 비활성화 함.
     IEnumerator RespawnBossCoroutine(Vector3 position)
    {
        yield return new WaitForSeconds(50f); //보스 리스폰 소요 시간
        //GameObject boss = PhotonNetwork.Instantiate(bossPrefab.name, bossSpawnPoint.position, Quaternion.identity);
        //bossList.Add(boss);
    }
}

