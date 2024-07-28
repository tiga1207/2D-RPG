using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Enemy_Skeleton : Entity
{

    public PhotonView PV;
    bool isAttacking;
    [Header("Move info")]
    [SerializeField] private float moveSpeed = 1f;

    [Header("Player Detection")]
    [SerializeField] private float playerCheckDistance;
    [SerializeField] private LayerMask whatIsPlayer;
    private RaycastHit2D isPlayerDetected;

    public Player player;

    protected override void Awake()
    {
        base.Awake();
        FindLocalPlayer();
    }

    protected override void Start()
    {
        base.Start();

        if (AttackTransform == null)
        {
            AttackTransform = transform;
        }
    }

    protected override void Update()
    {
        base.Update();

        HpController();
        Movement();

        if (!isGrounded || isWallDetected) // 벽 혹은 땅쪽일 경우 방향 전환
        {
            Flip();
        }
    }

    private void FindLocalPlayer()
    {
        Player[] players = FindObjectsOfType<Player>();
        foreach (var player in players)
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                this.player = player;
                break;
            }
        }
    }

    private void Movement()
    {
        if (isPlayerDetected) //플레이어 발견 여부에 따른 이동속도 및 행동
        {
            if (isPlayerDetected.distance > 1)
            {
                rb.velocity = new Vector2(moveSpeed * 1.5f * facingDir, rb.velocity.y); //적 발견시 이동
                Debug.Log("I see the player");
                isAttacking = false;
            }
            else //공격 사거리 내 플레이어 접근 시
            {
                Debug.Log("Attack " + isPlayerDetected.collider.gameObject.name);
                isAttacking = true;
                AttackPlayer(isPlayerDetected.collider.gameObject.GetComponent<PhotonView>().ViewID);
                // AttackPlayer();
            }
        }
        else //평상시
        {
            rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);
        }
    }

    protected override void CollisionCheck()
    {
        base.CollisionCheck();
        isPlayerDetected = Physics2D.Raycast(transform.position, Vector2.right, playerCheckDistance * facingDir, whatIsPlayer);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + playerCheckDistance * facingDir, transform.position.y));
    }

    protected override void Hited(float _damageDone, Vector2 _hitDirection)
    {
        base.Hited(_damageDone, _hitDirection);
        if (Hp <= 0)
        {
            Vector3 respawnPosition = transform.position;

            if (PV.IsMine || PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
                EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
                if (enemyManager != null && PhotonNetwork.IsMasterClient)
                {
                    enemyManager.RespawnEnemy(respawnPosition);
                }
            }
            else
            {
                // 마스터 클라이언트에게 삭제 요청
                PV.RPC("RequestDestroy", RpcTarget.MasterClient, PV.ViewID, respawnPosition);
            }
        }
    }

    [PunRPC]
    private void RequestDestroy(int viewID, Vector3 respawnPosition)
    {
        PhotonView enemyPV = PhotonView.Find(viewID);
        if (enemyPV != null && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(enemyPV.gameObject);
            EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.RespawnEnemy(respawnPosition);
            }
        }
    }

    public void AttackPlayer(int playerViewID)
    {
        PhotonView playerPV = PhotonView.Find(playerViewID);
        if (playerPV != null && playerPV.IsMine)
        {
            Player player = playerPV.GetComponent<Player>();
            if (player != null && !player.invincible)
            {
                Debug.Log("플레이어 공격중");
                player.TakeDamage(damage);
            }
        }
    }
}
