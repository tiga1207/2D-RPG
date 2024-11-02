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

    [SerializeField] protected Transform ShurikenAttackTransform;
    [SerializeField] protected Vector2 ShurikenAttackArea;
    public float dashSpeed = 10f;
    public float jumpHeight = 5f;
    public float shurikenSpeed = 30f;
    public float shurikenLifetime = 2f;
    public float dashDmg = 50f; 
    public bool attackPattern1=false;
    public bool attackPattern2=false;
    public bool attackPattern3=false;
    public bool isThrowShuriken=false;
    public float detectPlayerDistanceLimit= 50f;

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
    }
    protected override void Update()
    {
        base.Update();
        Movement();
        // NotAttackCondition();
        AnimatorController();
        LookAtTarget(targetPlayer);
    }
    protected override void Movement()
    {
        if (targetPlayer == null)
        {
            FindClosestPlayer();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

        // 일정 거리 이상 멀어졌을 경우 타겟 해제
        if (distanceToPlayer > detectPlayerDistanceLimit)
        {
            targetPlayer = null;
            rb.velocity = new Vector2(0,0);
            return;
        }

        // 공격 범위 내에 플레이어가 있고 쿨타임 중이면 보스를 멈춤
        if (IsPlayerInAttackArea() && isCooldownActive)
        {
            rb.velocity = new Vector2(0,0);
            return;
        }

        if (IsPlayerInShurikenArea() && attackActivate && !isAttacking && !isEnemyDie && !isCooldownActive)
        {
            rb.velocity = Vector2.zero;
            ThrowShuriken();
            if (!isCooldownActive)
            {
                StartCoroutine(AttackCooldown());
            }
        }

        // AttackArea 범위 내에 플레이어가 있을 경우 공격 준비
        else if (IsPlayerInAttackArea() && attackActivate && !isAttacking && !isEnemyDie && !isCooldownActive)
        {
            RandomAttackPattern();
            // 공격 쿨타임이 진행 중이 아닐 때만 코루틴 실행
            if (!isCooldownActive)
            {
                StartCoroutine(AttackCooldown());
            }
            // if(isBossDashing) return;
            new Vector2(0,0);
            
            
            
        }
        
        else if (!isAttacking && !isEnemyDie) // 공격 중이 아니고 살아있으면 이동
        {

            // // 수리검 공격 범위에 플레이어가 있으면 수리검 던지기
            // 
            // else
            // {
            //     // 플레이어를 추적
            //     Vector2 direction = (targetPlayer.position - transform.position).normalized;
            //     rb.velocity = new Vector2(moveSpeed * direction.x, rb.velocity.y);
            // }
            // 플레이어를 추적
            Vector2 direction = (targetPlayer.position - transform.position).normalized;
            rb.velocity = new Vector2(moveSpeed * direction.x, rb.velocity.y);
        }
    }

    private bool IsPlayerInAttackArea()
    {
        // AttackTransform은 공격 범위의 중심 위치, AttackArea는 공격 범위 크기, attackableLayer는 플레이어가 포함된 레이어
        Collider2D[] playerColliders = Physics2D.OverlapBoxAll(AttackTransform.position, AttackArea, 0, attackableLayer);

        foreach (Collider2D collider in playerColliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true; // 공격 범위 내에 플레이어가 있을 경우 true 반환
            }
        }
        return false; // 공격 범위 내에 플레이어가 없을 경우 false 반환
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireCube(ShurikenAttackTransform.position, ShurikenAttackArea);
    }

    private bool IsPlayerInShurikenArea()
    {
        // AttackTransform은 공격 범위의 중심 위치, AttackArea는 공격 범위 크기, attackableLayer는 플레이어가 포함된 레이어
        Collider2D[] playerColliders = Physics2D.OverlapBoxAll(ShurikenAttackTransform.position, ShurikenAttackArea, 0, attackableLayer);

        foreach (Collider2D collider in playerColliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true; // 공격 범위 내에 플레이어가 있을 경우 true 반환
            }
        }
        return false; // 공격 범위 내에 플레이어가 없을 경우 false 반환
    }

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

    private void RandomAttackPattern()
    {
        // isAttacking =true;
        int randomAttack = UnityEngine.Random.Range(0,3);
        switch (randomAttack)
        {
            case 0:
                Attack1();
                break;
            case 1:
                Attack2();
                break;
            case 2:
                Attack3();
                break;
            // case 3:
            //     StartCoroutine(DashAttack());
            //     break;
            // case 4:
            //     ThrowShuriken();
            //     break;
            
        // }
        }   
        // isAttacking = false;
        // isWaitNextAttack = false;
    }


    // // 대쉬 공격 패턴
    // private IEnumerator DashAttack()
    // {
    //     isAttacking = true;
    //     isBossDashing = true;
    //     collisionDmg = dashDmg; // 충돌 데미지를 대쉬 데미지로 변환
    //     Vector2 direction = (targetPlayer.position - transform.position).normalized;
    //     // rb.velocity = direction * dashSpeed;
    //     rb.velocity = new Vector2(direction.x * dashSpeed, rb.velocity.y);


    //     yield return new WaitForSeconds(3f); // 대쉬 지속 시간

    //     // rb.velocity = Vector2.zero;
    //     isBossDashing = false;
    //     isAttacking = false;
    //     collisionDmg = 10f;// 기본 충돌 데미지로 변경.
    // }


    private void Attack1()
    {
        isAttacking = true;
        attackPattern1 = true;
        Vector2 attackPosition = transform.position; // 공격 위치를 보스 위치로 설정
        // Collider2D[] playerToHit = Physics2D.OverlapBoxAll(attackPosition, attackBoxSize, 0, LayerMask.GetMask("Player"));//overlapBox 생성
        Collider2D[] playerToHit = Physics2D.OverlapBoxAll(AttackTransform.position, AttackArea, 0, attackableLayer);//overlapBox 생성


            foreach (Collider2D playerCollider in playerToHit)
            {
                if (playerCollider != null && playerCollider.CompareTag("Player"))
                {
                    Player player = playerCollider.GetComponent<Player>();
                    // if (player != null && !player.invincible)
                    if (player != null)
                    {
                        attackActivate = false;
                        // 플레이어에게 데미지 입히기
                        isAttacking = true;
                        attackPattern1 = true;
                        player.TakeDamage(damage); // 데미지 계산 방식에 따라 적절히 처리
                        Debug.Log("보스 근접 공격1패턴 실행");
                    }
                }
            }
    }

    private void Attack2()
    {
        // isAttacking = true;
        // attackPattern2 = true;
        Vector2 attackPosition = transform.position; // 공격 위치를 보스 위치로 설정
        // Collider2D[] playerToHit = Physics2D.OverlapBoxAll(attackPosition, attackBoxSize, 0, LayerMask.GetMask("Player"));//overlapBox 생성
        Collider2D[] playerToHit = Physics2D.OverlapBoxAll(AttackTransform.position, AttackArea, 0, attackableLayer);//overlapBox 생성


            foreach (Collider2D playerCollider in playerToHit)
            {
                if (playerCollider != null && playerCollider.CompareTag("Player"))
                {
                    Player player = playerCollider.GetComponent<Player>();
                    // if (player != null && !player.invincible)
                    if (player != null)
                    {
                        attackActivate = false;
                        // 플레이어에게 데미지 입히기
                        isAttacking = true;
                        attackPattern2 = true;
                        player.TakeDamage(damage); // 데미지 계산 방식에 따라 적절히 처리
                        Debug.Log("보스 근접 공격2패턴 실행");
                    }
                }
            }
    }

    private void Attack3()
    {
        isAttacking = true;
        attackPattern3 = true;
        // Vector2 attackPosition = transform.position; // 공격 위치를 보스 위치로 설정
        // Collider2D[] playerToHit = Physics2D.OverlapBoxAll(attackPosition, attackBoxSize, 0, LayerMask.GetMask("Player"));//overlapBox 생성
        Collider2D[] playerToHit = Physics2D.OverlapBoxAll(AttackTransform.position, AttackArea, 0, attackableLayer);//overlapBox 생성

            foreach (Collider2D playerCollider in playerToHit)
            {
                if (playerCollider != null && playerCollider.CompareTag("Player"))
                {
                    Player player = playerCollider.GetComponent<Player>();
                    // if (player != null && !player.invincible)
                    if (player != null)
                    {
                        attackActivate = false;
                        // 플레이어에게 데미지 입히기
                        isAttacking = true;
                        attackPattern3 = true;
                        player.TakeDamage(damage);
                        Debug.Log("보스 근접 공격3패턴 실행");
                    }
                }
            }
    }

    public void BossAttackOver()
    {
        // if (isTakeDamage)
        // {
        //     return; // 피격 상태에서는 공격 종료를 처리하지 않음
        // }
        isAttacking = false;
        attackPattern1 = false;
        attackPattern2 = false;
        attackPattern3 = false;
        isThrowShuriken = false;

    }

    // 수리검 던지기 공격
    private void ThrowShuriken()
    {
        isAttacking = true;
        isThrowShuriken = true;
        GameObject shuriken = PhotonNetwork.Instantiate(shurikenPrefab.name, shurikenSpawnPoint.position, Quaternion.identity);
        Rigidbody2D shurikenRb = shuriken.GetComponent<Rigidbody2D>();
        Vector2 direction = (targetPlayer.position - shurikenSpawnPoint.position).normalized;
        shurikenRb.velocity = direction * shurikenSpeed;
        if(shuriken != null)
        {
            StartCoroutine(DestroyAfter(shuriken, shurikenLifetime));
        }
    }

    protected override void AnimatorController() //플레이어 애니메이션 관리 
    {
        base.AnimatorController();
        anim.SetBool("isTakeDamage", !isAttacking && isTakeDamage);
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
        Debug.Log("적 히트 rpc 호출");
        if(Hp<=0) return;
        Hp -= _damageDone; //체력 감소
        // isAttacking=false;// 공격중이라도 피격시 취소.
        HpBarController(Hp);// 체력바 업데이트
        ShowDamageText(_damageDone, transform.position);// 데미지 표시 텍스트 메서드 호출
        if(isAttacking || isEnemyDie)
        {
            return;
        }
        isTakeDamage =true;
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

    public override void Enemy_DieAfter()
    {
        Vector3 respawnPosition = transform.position;
        Vector3 itemPosition = new Vector3(transform.position.x,transform.position.y-1,transform.position.z);
        if (PhotonNetwork.IsMasterClient)// 마스터 클라이언트 일 경우 
        {
            PhotonNetwork.Destroy(gameObject);//네트워크 상에서 적을 파괴함.
            // EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
            // if (enemyManager != null && PhotonNetwork.IsMasterClient)
            // {
            //     enemyManager.RespawnEnemy(respawnPosition);// 적 리스폰
            // }
            RespawnEnemy(respawnPosition);
            ItemDrop(fieldItem,itemPosition);
        }
        else // 슬레이브 클라이언트 일 경우
        {
            PV.RPC("RequestDestroy", RpcTarget.MasterClient, PV.ViewID, respawnPosition); //마스터 클라이언트에게 파괴 요청.
        }

        // 적을 처치한 플레이어에게 경험치 부여
        foreach (PhotonView attacker in attackers)
        {
            if (attacker != null)
            {
                QuestManager.instance.UpdateKillCount();
                attacker.RPC("AddExpRPC", attacker.Owner, experiencePoints);
                attacker.RPC("AddJumpCountRPC", attacker.Owner);
            }
        }
        
    }


}

