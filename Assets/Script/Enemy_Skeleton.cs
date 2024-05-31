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


        HpController(); 
        Movement();

        if (!isGrounded || isWallDeteted) // 벽 혹은 땅쪽일 경우 방향 전환
        {
            Filp();
        }

    }


    private void Movement()//기본 이동
    {
        if (isPlayerDectected) //플레이어 발견시 행동
        {
            if (isPlayerDectected.distance > 1)
            {
                rb.velocity = new Vector2(moveSpeed * 20 * facingDir, rb.velocity.y); //적 발견시 이동
                Debug.Log("i see the player");
                isattacking = false;
            }
            else 
            {
                Debug.Log("Attack" + isPlayerDectected.collider.gameObject.name);
                isattacking = true;
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
}
