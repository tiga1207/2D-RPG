using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Boss : EnemyBase
{
    public Transform[] players; // 플레이어들의 Transform 배열
    private Transform targetPlayer; // 현재 보스가 바라볼 타겟 플레이어

    public GameObject shurikenPrefab;
    public Transform shurikenSpawnPoint;

    [Header("Attack Settings")]
    public bool isWaitNextAttack = false;
    Vector2 attackBoxSize = new Vector2(2f, 1f); // 공격 범위 설정 (너비 2, 높이 1)
    public float dashSpeed = 10f;
    public float jumpHeight = 5f;
    public float shurikenSpeed = 10f;
    public float shurikenLifetime = 2f;
    public float dashDmg = 50f; 
    public bool attackPattern1=false;
    public bool attackPattern2=false;
    public bool attackPattern3=false;
    public bool isThrowShuriken=false;
    public float detectPlayerDistanceLimit= 30f;

    // private bool isAttacking = false;
    [SerializeField] private bool isBossDashing = false;
    public float delayAttackPatternTime = 40f;

    public float distanceToPlayer; 

    private System.Random random = new System.Random(); // 랜덤 패턴을 위한 변수

    protected override void Start()
    {
        base.Start();
        collisionDmg= 10f; // 충돌 데미지 기본값 지정
        FindClosestPlayer(); // 초기 타겟 플레이어 설정
        // StartCoroutine(RandomAttackPattern()); // 랜덤 패턴 선택 시작
    }

    // 매 프레임마다 가장 가까운 플레이어를 찾음
    protected override void Update()
    {
        base.Update();
        Movement();
        NotAttackCondition();
        AnimatorController();
        LookAtTarget(targetPlayer);
    }
    protected override void Movement()
    {
        if(targetPlayer == null)
        {
            FindClosestPlayer();
            return;
        }
        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

        // 플레이어가 30f 거리 내에 있으면 이속 2배
        if (distanceToPlayer < detectPlayerDistanceLimit)
        {
            rb.velocity = new Vector2(moveSpeed * 2f * facingDir, rb.velocity.y);
        }

        // else if (!isAttacking && !isEnemyDie)
        else if (isAttacking || !isEnemyDie)
        {
            rb.velocity = Vector2.zero; // 공격 중일 때 이동 멈춤
            // AttackPlayer(isPlayerDetected.collider.gameObject.GetComponent<PhotonView>().ViewID, damage);
            // isPlayerDetected에 대한 null 체크 추가
            if (isPlayerDetected.collider != null)
            {
                // AttackPlayer(isPlayerDetected.collider.gameObject.GetComponent<PhotonView>().ViewID, damage);
            }
            else
            {
                Debug.LogWarning("No player detected in Movement");
            }

        }
        
        else if (isEnemyDie || isTakeDamage)
        {
            rb.velocity = Vector2.zero; // 적이 죽거나 데미지를 입으면 이동 멈춤
        }
        else // 평소 속도
        {
            rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);
        }
    }

    private void NotAttackCondition()
    {
        if(!isAttacking && !isWaitNextAttack)
        {   
            AttackCondition();
        }
    }

    private void AttackCondition()
    {
        FindClosestPlayer();// 공격 중이 아닐 때만 플레이어 추적

        if (targetPlayer == null)
        {
            Debug.LogWarning("No target player found in NotAttackCondition");
            return; // targetPlayer가 null일 경우 함수 종료
        }

        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

        // 플레이어가 일정 거리 내에 있을 때 공격 패턴 실행
        if (distanceToPlayer <= 10f && isWaitNextAttack) // 10f는 공격을 시작할 거리
        {
            StartCoroutine(RandomAttackPattern());
        }
    }

    // 가장 가까운 플레이어를 찾는 메서드
    // private void FindClosestPlayer()
    // {
    //     if (players.Length == 0)
    //     {
    //         Debug.LogWarning("No players found!");
    //         return;
    //     }
    //     float closestDistance = Mathf.Infinity;
    //     players = FindPlayers(); // 현재 씬의 플레이어들을 찾음

    //     foreach (Transform player in players)
    //     {
    //         float distanceToPlayer = Vector2.Distance(transform.position, player.position);
    //         if (distanceToPlayer <= detectPlayerDistanceLimit &&distanceToPlayer < closestDistance)
    //         {
    //             closestDistance = distanceToPlayer;
    //             targetPlayer = player;
    //         }
    //     }

    //     // 모든 플레이어가 같은 위치에 있을 경우 랜덤으로 한 명 선택
    //     if (closestDistance == 0 && players.Length > 1)
    //     {
    //         targetPlayer = players[random.Next(players.Length)];
    //     }

    //     if (targetPlayer == null)
    //     {
    //         Debug.LogWarning("No target player found within range!");
    //         return;
    //     }

    //     // 타겟 플레이어를 향해 회전
    //     LookAtTarget(targetPlayer);
    // }

    private void FindClosestPlayer()
{
    players = FindPlayers(); // 현재 씬의 플레이어들을 먼저 찾음

    if (players.Length == 0)
    {
        Debug.LogWarning("No players found!");
        return;
    }

    float closestDistance = Mathf.Infinity;
    
    foreach (Transform player in players)
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectPlayerDistanceLimit && distanceToPlayer < closestDistance)
        {
            closestDistance = distanceToPlayer;
            targetPlayer = player;
        }
    }

    // 플레이어가 한 명일 경우, 무조건 그 플레이어를 타겟으로 설정
    if (players.Length == 1)
    {
        targetPlayer = players[0];
    }
    // 모든 플레이어가 같은 위치에 있을 경우 랜덤으로 한 명 선택 (두 명 이상일 때만 실행)
    else if (closestDistance == 0 && players.Length > 1)
    {
        targetPlayer = players[random.Next(players.Length)];
    }

    if (targetPlayer == null)
    {
        Debug.LogWarning("No target player found within range!");
        return;
    }

    // 타겟 플레이어를 향해 회전
    LookAtTarget(targetPlayer);
}

    // 타겟 플레이어를 바라보는 함수
    private void LookAtTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction.x > 0 && !facingRight)
        {
            // Flip();
            PV.RPC("FlipRPC", RpcTarget.AllBuffered, true);
        }
        else if (direction.x < 0 && facingRight)
        {
            // Flip();
            PV.RPC("FlipRPC", RpcTarget.AllBuffered, false);
        }
    }

    

    // 플레이어들을 찾는 메서드 (PhotonView가 있는 오브젝트들만)
    private Transform[] FindPlayers()
    {
        Player[] playersArray = FindObjectsOfType<Player>();
        List<Transform> playerTransforms = new List<Transform>();

        foreach (Player player in playersArray)
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                playerTransforms.Add(player.transform);
            }
        }
        return playerTransforms.ToArray();
    }

    // 랜덤한 공격 패턴을 선택하는 코루틴
    private IEnumerator RandomAttackPattern()
    {
        isAttacking =true;
        isWaitNextAttack= true;
            // if (!isAttacking)
            // {
                int randomAttack = random.Next(5); //5개 패턴 중 랜덤 선택
                switch (randomAttack)
                {
                    case 0:
                        yield return StartCoroutine(DashAttack());
                        break;
                    case 1:
                        Attack1();
                        break;
                    case 2:
                        ThrowShuriken();
                        break;
                    case 3:
                        Attack2();
                        break;
                    case 4:
                        Attack3();
                        break;
                    
                // }
            }

            yield return new WaitForSeconds(delayAttackPatternTime); // 공격 패턴 간 대기 시간
            isAttacking = false;
            isWaitNextAttack = false;
    
    }


    // 대쉬 공격 패턴
    private IEnumerator DashAttack()
    {
        isAttacking = true;
        isBossDashing = true;
        collisionDmg = dashDmg; // 충돌 데미지를 대쉬 데미지로 변환
        Vector2 direction = (targetPlayer.position - transform.position).normalized;
        rb.velocity = direction * dashSpeed;

        yield return new WaitForSeconds(1f); // 대쉬 지속 시간

        rb.velocity = Vector2.zero;
        isBossDashing = false;
        isAttacking = false;
        collisionDmg = 10f;// 기본 충돌 데미지로 변경.
    }

    private void Attack1()
    {
        isAttacking = true;
        attackPattern1 = true;
        Vector2 attackPosition = transform.position; // 공격 위치를 보스 위치로 설정
        Collider2D[] playerToHit = Physics2D.OverlapBoxAll(attackPosition, attackBoxSize, 0, LayerMask.GetMask("Player"));//overlapBox 생성

            foreach (Collider2D playerCollider in playerToHit)
            {
                if (playerCollider != null && playerCollider.CompareTag("Player"))
                {
                    Player player = playerCollider.GetComponent<Player>();
                    if (player != null)
                    {
                        // 플레이어에게 데미지 입히기
                        player.TakeDamage(damage); // 데미지 계산 방식에 따라 적절히 처리
                        Debug.Log("보스 근접 공격1패턴 실행");
                    }
                }
            }
    }

    private void Attack2()
    {
        isAttacking = true;
        attackPattern2 = true;
        Vector2 attackPosition = transform.position; // 공격 위치를 보스 위치로 설정
        Collider2D[] playerToHit = Physics2D.OverlapBoxAll(attackPosition, attackBoxSize, 0, LayerMask.GetMask("Player"));//overlapBox 생성

            foreach (Collider2D playerCollider in playerToHit)
            {
                if (playerCollider != null && playerCollider.CompareTag("Player"))
                {
                    Player player = playerCollider.GetComponent<Player>();
                    if (player != null)
                    {
                        // 플레이어에게 데미지 입히기
                        player.TakeDamage(damage); // 데미지 계산 방식에 따라 적절히 처리
                        Debug.Log("보스 근접 공격1패턴 실행");
                    }
                }
            }
    }

    private void Attack3()
    {
        isAttacking = true;
        attackPattern3 = true;
        Vector2 attackPosition = transform.position; // 공격 위치를 보스 위치로 설정
        Collider2D[] playerToHit = Physics2D.OverlapBoxAll(attackPosition, attackBoxSize, 0, LayerMask.GetMask("Player"));//overlapBox 생성

            foreach (Collider2D playerCollider in playerToHit)
            {
                if (playerCollider != null && playerCollider.CompareTag("Player"))
                {
                    Player player = playerCollider.GetComponent<Player>();
                    if (player != null)
                    {
                        // 플레이어에게 데미지 입히기
                        player.TakeDamage(damage); // 데미지 계산 방식에 따라 적절히 처리
                        Debug.Log("보스 근접 공격1패턴 실행");
                    }
                }
            }
    }

    public void BossAttackOver()
    {
        if (isTakeDamage)
        {
            return; // 피격 상태에서는 공격 종료를 처리하지 않음
        }
        isAttacking = false;
        attackPattern1 = false;
        attackPattern2 = false;
        attackPattern3 = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, attackBoxSize);
    }

    // // 점프 공격 패턴
    // private IEnumerator JumpAttack()
    // {
    //     if (isGrounded)
    //     {
    //         isAttacking = true;
    //         isJumping = true;
    //         rb.velocity = new Vector2(rb.velocity.x, jumpHeight);

    //         yield return new WaitForSeconds(1f); // 점프 지속 시간

    //         isJumping = false;
    //         isAttacking = false;
    //     }
    // }

    // 수리검 던지기 공격
    private void ThrowShuriken()
    {
        isAttacking = true;
        isThrowShuriken = true;
        GameObject shuriken = PhotonNetwork.Instantiate(shurikenPrefab.name, shurikenSpawnPoint.position, Quaternion.identity);
        Rigidbody2D shurikenRb = shuriken.GetComponent<Rigidbody2D>();
        Vector2 direction = (targetPlayer.position - shurikenSpawnPoint.position).normalized;
        shurikenRb.velocity = direction * shurikenSpeed;
        StartCoroutine(DestroyAfter(shuriken, shurikenLifetime));
        isThrowShuriken = false;
        isAttacking = false;
    }

    protected override void AnimatorController() //플레이어 애니메이션 관리 
    {
        base.AnimatorController();
        anim.SetBool("isBossDashing", isBossDashing);
        anim.SetBool("attackPattern1", attackPattern1);
        anim.SetBool("attackPattern2", attackPattern2);
        anim.SetBool("attackPattern3", attackPattern3);
        anim.SetBool("isThrowShuriken", isThrowShuriken);

        
    }

    protected override void EnemyDyingCheck()
    {
        if (Hp <= 0 && !isEnemyDie)
        {
            isTakeDamage= false;
            isAttacking = false;
            attackPattern1 =false;
            attackPattern2 = false;
            attackPattern3 = false;
            isThrowShuriken = false;
            isEnemyDie = true;
            // gameObject.tag="DeadEnemy";
            gameObject.layer=LayerMask.NameToLayer("DeadEnemy");
            return;
        }
    }

    // 보스가 피해를 입으면 가장 가까운 플레이어를 다시 찾음
    [PunRPC]
    public override void HitedRPC(float _damageDone, Vector2 _hitDirection)
    {
        base.HitedRPC(_damageDone, _hitDirection);
        FindClosestPlayer(); // 보스가 공격을 받을 때마다 다시 가까운 플레이어를 추적
    }

    protected override void ReqeustRespawn(Vector3 respawnPosition)
    {
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.RespawnBoss(respawnPosition);
        }
    }
    protected override void RespawnEnemy(Vector3 respawnPosition)
    {

        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null && PhotonNetwork.IsMasterClient)
        {
            enemyManager.RespawnBoss(respawnPosition);
        }
    }   

}

