using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Skeleton : Entity
{

    bool isattacking;
    [Header("Move info")]
    [SerializeField] private float moveSpeed = 1f;

    [Header("Player Dectection")]
    [SerializeField] private float playerCheckDistance;
    [SerializeField] private LayerMask whatIsPlayer;
    private RaycastHit2D isPlayerDectected;

    [SerializeField] protected Player player;

    protected override void Awake()
    {
        base.Awake();
        player = Player.Instance;

    }
    protected override void Start()
    {
        base.Start();

        if(AttackTransform==null){
            AttackTransform=transform;
        }
    }
    protected override void Update()
    {
        base.Update();

        // RecoilController(); 

        HpController(); 
        Movement();

        

        if (!isGrounded || isWallDeteted) // 벽 혹은 땅쪽일 경우 방향 전환
        {
            Filp();
        }

    }


    private void Movement()//기본 이동
    {
        if (isPlayerDectected) //플레이어 발견 여부에 따른 이동속도 및 행동
        {
            if (isPlayerDectected.distance > 1) 
            {
                rb.velocity = new Vector2(moveSpeed * 1.5f * facingDir, rb.velocity.y); //적 발견시 이동
                Debug.Log("i see the player");
                isattacking = false;
                
            }
            else //공격 사거리 내 플레이어 접근 시
            {
                Debug.Log("Attack" + isPlayerDectected.collider.gameObject.name);
                isattacking = true;
                AttackPlayer();
            }

        }
        else //평상시
        {
            rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);

        }
    }

    protected override void CollisionCheck()//충돌 체크
    {
        base.CollisionCheck();

        isPlayerDectected = Physics2D.Raycast(transform.position, Vector2.right,playerCheckDistance *facingDir,whatIsPlayer);

    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos(); 
        Gizmos.color=Color.blue;     
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + playerCheckDistance* facingDir,transform.position.y)); 
    }


    protected override void Hited(float _damageDone, Vector2 _hitDirection)
    {
        base.Hited(_damageDone, _hitDirection);
    }

    public void AttackPlayer()
    {
        if (!Player.Instance.invincible)
        {
            Player.Instance.TakeDamage(damage);
        }
    }

    // public void AttackPlayer()
    // {
    //  Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(AttackTransform.position, AttackArea, 0, whatIsPlayer);
    //     foreach (var hitPlayer in hitPlayers)
    //     {
    //         if (hitPlayer.CompareTag("Player") && !Player.Instance.invincible)
    //         {
    //             Player.Instance.TakeDamage(damage);
    //         }
    //     }
    // }



    // public void OnTriggerStay2D(Collider2D _other) {
    //      if(_other.CompareTag("Player") && (!Player.Instance.invincible)){
    //         AttackPlayer();
    //      }
    // }



}
