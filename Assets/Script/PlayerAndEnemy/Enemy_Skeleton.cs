using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System;

public class Enemy_Skeleton : Entity,IPunObservable
{

    public PhotonView PV;
    public bool isEnemyDie= false;
    public bool isTakeDamage = false;
    public bool isAttacking=false;
    public bool attackActivate =true;
    [SerializeField] private float attackCooldown=1f;
    [SerializeField] private float attackCooldownTimer=1f;
    public bool isCooldownActive = false;

    [Header("Move info")]
    [SerializeField] private float moveSpeed = 1f;

    [Header("Player Detection")]
    [SerializeField] private float playerCheckDistance;
    [SerializeField] private LayerMask whatIsPlayer;
    private RaycastHit2D isPlayerDetected;

    public Player player;
    [SerializeField] private float experiencePoints = 90;
    public GameObject fieldItem;

    public float collisionDmg= 1f;

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

        // HpController();
        Movement();
        AnimatorController();
        EnemyDyingCheck();


        if (!isGrounded || isWallDetected) // 벽 혹은 땅쪽일 경우 방향 전환
        {
            FlipController();
        }
    }


    private void FlipController()
    {
        if(facingRight)
        {
            PV.RPC("FlipRPC", RpcTarget.AllBuffered, false);
        }
        else if(!facingRight)
        {
            PV.RPC("FlipRPC", RpcTarget.AllBuffered, true);
        }
    }

    private void Movement()
    {
        if (isPlayerDetected) //플레이어 발견 여부에 따른 이동속도 및 행동
        {
            if (isPlayerDetected.distance > 1)// 최대 탐지 사거리 playerCheckDistance보단 작음.
            {
                rb.velocity = new Vector2(moveSpeed * 1.5f * facingDir, rb.velocity.y); //적 발견시 이동속도
                // Debug.Log("I see the player");
                // isAttacking = false;
            }
            else //공격 사거리(isPlayerDetected.distance <1 일 경우)) 내 플레이어 접근 시
            {
                if(!isAttacking && attackActivate && !isEnemyDie)
                {
                // Debug.Log("Attack " + isPlayerDetected.collider.gameObject.name);
                // isAttacking = true;
                rb.velocity = new Vector2(0,0);// 특정 애니메이션(공격 모션 끝)이 끝나기 전까지는 해당 오브젝트가 움직이지 못하도록 고정시켜야함.
                AttackPlayer(isPlayerDetected.collider.gameObject.GetComponent<PhotonView>().ViewID,damage);
                if (!isCooldownActive) // 쿨타임이 진행 중이 아니면 코루틴 시작
                {
                    StartCoroutine(AttackCooldown()); // 쿨타임 시작
                }
                // StartCoroutine(AttackCooldown());//공격 쿨타임(공격 속도에 따른) 코루틴 호출
                }
            }
        }
        else if(isEnemyDie || isTakeDamage)
        {
            rb.velocity = new Vector2(0,0);
        }
        else //평상시
        {
            rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);
        }
    }

    [PunRPC]
    private void FlipRPC(bool faceRight)
    {
        facingRight = faceRight;
        facingDir = faceRight ? 1 : -1;
        transform.Rotate(0, 180, 0);
        hpBar.transform.Rotate(0,180,0);
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

    private void AnimatorController() //플레이어 애니메이션 관리 
    {
        bool isMoving = rb.velocity.x != 0;
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isEnemyDie", isEnemyDie);
        anim.SetBool("isTakeDamage", isTakeDamage);
        anim.SetBool("isAttacking",isAttacking);
        
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
    
    [PunRPC]
    public void HitedRPC(float _damageDone, Vector2 _hitDirection)
    {
        Debug.Log("적 히트 rpc 호출");
        if(Hp<=0) return;
        Hp -= _damageDone; //체력 감소
        isAttacking=false;// 공격중이라도 피격시 취소.
        isTakeDamage =true;
        HpBarController(Hp);// 체력바 업데이트
        ShowDamageText(_damageDone, transform.position);// 데미지 표시 텍스트 메서드 호출
        //피격후 1초동안 피격당하지 않을 시 피격 애니메이션 종료.
        // StartCoroutine(StopTakeDamageSkeleton());
    }

     //코루틴 예시
    private IEnumerator StopTakeDamageSkeleton()
    {
        yield return new WaitForSeconds(1f);
        isTakeDamage = false;
    }

    private void EnemyDyingCheck()
    {
        if (Hp <= 0 && !isEnemyDie)
        {
            isTakeDamage= false;
            isAttacking = false;
            isEnemyDie = true;
            return;
        }
    }

    public void Enemy_DieAfter()
    {
        Vector3 respawnPosition = transform.position;
        Vector3 itemPosition = new Vector3(transform.position.x,transform.position.y-1,transform.position.z);
        if (PhotonNetwork.IsMasterClient)// 마스터 클라이언트 일 경우 
        {
            PhotonNetwork.Destroy(gameObject);//네트워크 상에서 적을 파괴함.
            EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager != null && PhotonNetwork.IsMasterClient)
            {
                enemyManager.RespawnEnemy(respawnPosition);// 적 리스폰
            }
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
            }
        }
    }

    private static void ItemDrop(GameObject gameObject,Vector3 _respawnPosition) //아이템 드랍 코드
    {
        Debug.Log("아이템 드랍 로직 진입");
        int randomDropP = UnityEngine.Random.Range(1, 11); //1~10까지 중 랜덤 수
        int itemP = UnityEngine.Random.Range(0, ItemDataBase.instance.itemDB.Count-1);
        if (randomDropP > 3)//아이템 드랍 확률
        {
            Debug.Log($"랜덤하게 아이템 생성! 아이템 리스트 {itemP+1}번째 아이템");
            ItemDataBase.instance.itemAdd(itemP);
            GameObject go = PhotonNetwork.Instantiate(gameObject.name, _respawnPosition, Quaternion.identity);
            go.GetComponent<PhotonView>().RPC("SetItemID", RpcTarget.AllBuffered, ItemDataBase.instance.itemDB[itemP].itemID);
        }
        else
        {
            Debug.Log($"랜덤가챠 실패!! 랜덤 시드: {randomDropP}");
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
    //플레이어 공격 시
    public void AttackPlayer(int playerViewID, float _damage)// 플레이어 공격
    {
        PhotonView playerPV = PhotonView.Find(playerViewID);
        if (playerPV != null && playerPV.IsMine)
        {
            Player player = playerPV.GetComponent<Player>();
            if (player != null && !player.invincible)
            {
                Debug.Log("플레이어 공격중 i'm attack player!!!");
                attackActivate= false;
                isAttacking= true;
                // if(player.invincible) return;
                player.TakeDamage(_damage);
            }
        }
    }

    //단순 충돌 시
    public void AttackPlayerJustCollision(int playerViewID, float _damage)// 플레이어 공격
    {
        PhotonView playerPV = PhotonView.Find(playerViewID);
        if (playerPV != null && playerPV.IsMine)
        {
            Player player = playerPV.GetComponent<Player>();
            if (player != null && !player.invincible)
            {
                player.TakeDamage(_damage);
            }
        }
    }

    public void AttackOver()
    {
        if (isTakeDamage || isEnemyDie)
        {
            return; // 피격 상태에서는 공격 종료를 처리하지 않음
        }
        isAttacking = false;
        // attackCooldownTimer=attackCooldown;
    }

    public void TakeDamageOver()
    {
        if (isAttacking || isEnemyDie)
        {
            return;
        }
        isTakeDamage = false;
    }

    private IEnumerator AttackCooldown()
    {
        Debug.Log("쿨다운 코루틴 실행");
        if (isCooldownActive) 
        {
            Debug.Log("쿨다운 코루틴 실행 취소");
            yield break; // 쿨타임이 이미 진행 중이면 코루틴을 중단
        }

        isCooldownActive = true;
        
        while (attackCooldownTimer >= 0)
        {
            Debug.Log("쿨타임 내부 실행");
            attackActivate=false;
            attackCooldownTimer -= Time.deltaTime;
            yield return null;
        }
        Debug.Log("쿨타임 끝");
        attackCooldownTimer = attackCooldown; // 쿨타임 타이머를 초기화
        isCooldownActive = false;
        attackActivate = true;//공격쿨타임 타이머가 0이하일 때 공격 상태 활성화
    }

    //플레이어와 충돌시 플레이어에게 데미지 부여.
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player"))
        {
            AttackPlayerJustCollision(other.gameObject.GetComponent<PhotonView>().ViewID,collisionDmg);
        }
        
    }

    protected void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);//overlapBox 생성

        for (int i = 0; i < objectsToHit.Length; ++i)//overlapBox 내부에 영역 검사.
        {
            if (objectsToHit[i].GetComponent<Enemy_Skeleton>() != null) // overlapBox 영역 내부에 적 존재 시 
            {
                Enemy_Skeleton enemy = objectsToHit[i].GetComponent<Enemy_Skeleton>();
                if (!enemy.attackers.Contains(PV))
                {
                    enemy.attackers.Add(PV); // 플레이어의 PhotonView를 attackers 목록에 추가
                }
                enemy.PV.RPC("HitedRPC", RpcTarget.AllBuffered, damage, (Vector2)(transform.position - objectsToHit[i].transform.position));
            
            }
        }
    }

    public override void HpBarController(float hp)
    {
        base.HpBarController(hp);
    }

    public override float Hp
    {
        get { return hp; }
        set
        {
            if (hp != value)
            {
                hp = Mathf.Clamp(value, 0, maxHp);
                // HpBarController(hp);

            }
        }
    }


     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Hp);
            // stream.SendNext(maxHp);
        }
        else
        {
            Hp = (float)stream.ReceiveNext();
            // maxHp = (float)stream.ReceiveNext();
            // HpBarController(hp);
        }
    }
}
