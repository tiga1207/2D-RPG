using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Player : Entity, IPunObservable
{   
    public string currentMapName; // 플레이어 현 위치 맵(씬)이름
    public static Player LocalPlayerInstance;

    public bool isCrushed= false;
    public float StopTakeDamageTime= 0.5f;

    private float xInput, yInput; //x축 이동, y축 이동
    private SpriteRenderer childSr;
    private Collider2D playerCollider;

    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gravity = 3.5f;
    [SerializeField] private float jumpForce;

    [Header("Dash")]
    public bool dashSkillActivate;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration; // 대시 지속시간
    [SerializeField] public float dashCooldown;//대시 스킬 쿨타임
    [SerializeField] private float dashTime;// 대시 지속시간 쿨타이머 0으로 떨어질 시 대시 끝
    [SerializeField] public float dashCooldownTimer;
    [SerializeField] public bool dashActive= false;
    public bool isDashing = false;

    [Header("Attack Info")]
    [SerializeField] private float attackCooldown=1f;
    [SerializeField] private float attackCooldownTimer;
    [SerializeField] private bool attackActive = true;
    
    [SerializeField] private float comboTime = 0.3f;
    private float comboTimeWindow;
    [SerializeField] protected GameObject slashEffect;
    [SerializeField] protected GameObject ultimateEffect;
    [SerializeField] private bool isAttacking;


    public bool ultimateSkillActivate;
    [SerializeField] private bool isUltimateAttacking;
    public float ultimateAttackCooldown=1f;
    public float ultimateAttackCooldownTimer;
    public bool ultimateAttackActive = true;


    private int comboCounter;

    [Header("Player State")]
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private float hitFlashSpeed;
    [SerializeField] public bool invincible = false;
    public bool isTakeDamage = false;
    public bool isPlayerDie = false;
    [SerializeField] private GameObject sr;
    public float levelupStatPoint=1;
    public float levelupSkillPoint=1;


    [Header("Jump Ability")]
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private int maxJumpCount = 2;
    public bool isJumping= false;

    [Header("Photon")]
    public TMP_Text NickNameText;
    public Image HealthImage;
    public Image ManaImage;
    public Inventory playerInventory;
    
    public PhotonView PV;

    [Header("Skill")]

    [Header("Healing")]
    [SerializeField] private GameObject healEffect;
    public bool healSkillActivate;

    public bool isHealing = false; // 힐 스킬 사용 여부
    public bool healActive= true;
    public int healAmount = 1; //힐량
    public int healManaCost = 3;
    public float healCooldown= 5; //쿨타임 시간.
    public float  healCoolTimer=5; //쿨타임 타이머
    [SerializeField] protected float  healChanneling= 2; // 정신집중 시간.
    [SerializeField] protected float  healChannelingTimer =2; // 정신집중 시간.

    [SerializeField] protected float healInterval = 1; // 힐 간격
    private Coroutine healCoroutine;
    // private string previousMapName;

    protected override void Awake()
    {
        base.Awake();
        playerCollider = GetComponent<Collider2D>();
        playerInventory = GetComponent<Inventory>();

        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

        if (sr != null)
        {    
            childSr = sr.GetComponent<SpriteRenderer>();

        }

         if (PV.IsMine)
        {
            // 2D 카메라
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;

            // SceneManager.sceneLoaded += OnSceneLoaded;  // 씬 로드 이벤트 구독

            photonView.RPC("InitializeInventory", RpcTarget.AllBuffered); // 인벤토리 초기화
            LocalPlayerInstance = this;
        }

        //플레이어끼리의 충돌 방지
        int playerLayer = LayerMask.NameToLayer("Player");
        Physics2D.IgnoreLayerCollision(playerLayer, playerLayer);
    }
    
    //  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     if (PV.IsMine)
    //     {
    //         previousMapName = currentMapName;
    //         currentMapName = scene.name;

    //         if (!string.IsNullOrEmpty(previousMapName) && previousMapName != currentMapName)
    //         {
    //             // 이전 맵에 있는 본인 플레이어 객체를 삭제
    //             DestroyPreviousPlayer();
    //         }
    //     }
    // }

    // private void DestroyPreviousPlayer()
    // {
    //     // 이전 맵에 남아 있는 본인 플레이어 객체를 파괴
    //     PhotonView[] allPhotonViews = FindObjectsOfType<PhotonView>();
    //     foreach (PhotonView pv in allPhotonViews)
    //     {
    //         if (pv.IsMine && pv != this.PV)
    //         {
    //             PhotonNetwork.Destroy(pv.gameObject);
    //         }
    //     }
    // }

    // private void OnDestroy()
    // {
    //     if (PV.IsMine)
    //     {
    //         SceneManager.sceneLoaded -= OnSceneLoaded;  // 씬 로드 이벤트 구독 해제
    //     }
    // }


    [PunRPC]
    public void InitializeInventory()
    {
        playerInventory.Initialize();
    }

    protected override void Start()
    {
        base.Start();
        if (PV.IsMine)
        {
            // 초기 HP와 MP 설정
            Hp = maxHp;
            Mp = maxMp;
            Exp=0;
            Level=1;
            Damage=5;
            LevelupStatPoint=0;
            UIManager.Instance.SetPlayer(this);
            SkillUIManager.Instance.SetPlayer(this);
            StatUI.Instance.SetPlayer(this);
            SkillUI.Instance.SetPlayer(this);
            QuestUI.Instance.SetPlayer(this);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (PV.IsMine)
        {
            PlayerDying();
            AnimatorController();
            
            Movement();
            JumpAbility();
            CheckInput();
            CooldownManager();
            FlipController();
            PlayerWhenHited();
            
            LerpPlayerUI();
            // DashAbility(); //대시 코루틴 미사용시
        }
    }
    // 적이랑 충돌 시 밀려남(recoil) 구현
    private void OnCollisionEnter2D(Collision2D other)
    {
        // if (other.gameObject.CompareTag("Enemy") && !isCrushed)
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable") && !isCrushed &&!invincible)
        {
            // float recoilDistance =2f; 
            float dirX = transform.position.x - other.transform.position.x > 0 ? 2f : -2f;
            Debug.Log("dirx 값"+dirX);
            transform.position = new Vector2(transform.position.x + dirX, transform.position.y);
            Debug.Log("충돌"); 
            isCrushed= true;
        }
    }

    private void LerpPlayerUI()
    {
        UIManager.Instance.UpdateHP(hp, maxHp);
        UIManager.Instance.UpdateMP(mp, maxMp);
        UIManager.Instance.UpdateEXP(exp,maxExp);
    }

    public void PlayerDieAfter() //플레이어 사망 후처리 로직
    {
        gameObject.SetActive(false);// 플레이어 오브젝트 비활성화 시키기.
        isPlayerDie = false;
        PlayerRespawn.Instance.OnRespawnPanel();    
    }

    public void PlayerDying()
    {
        if (Hp <= 0 && !isPlayerDie)
        {
            isTakeDamage= false; // 피격상태로 게임 오브젝트가 비활성화 될 경우 리스폰시 피격상태로 판정됨.
            invincible = false; // 피격상태로 게임 오브젝트가 비활성화 될 경우 리스폰시 피격상태로 판정됨.        
            isPlayerDie = true; // 죽는 모션 플레이
            // PhotonNetwork.Destroy(gameObject);

        }
    }

    private void CooldownManager()
    {
        /*대시 스킬 코루틴 미사용시
        if (!dashActive)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
            {
                dashCooldownTimer = 0;
                dashActive = true;
            }
        }
        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                dashTime = 0;
                isDashing = true;
            }
        }
        */
        // healCoolTimer-=Time.deltaTime;// 힐 쿨타임. 0보다 작으면 사용가능.
        comboTimeWindow -= Time.deltaTime;

    }
//공격
    private void StartAttackEvent()
    {
        if (!isGrounded||isUltimateAttacking)
        {
            return; // 플레이어가 땅에 있지 않으면 아래 다 날리기.
        }

        if (comboTimeWindow < 0)
        {
            comboCounter = 0;
        }
        if (attackActive && (yInput == 0 || yInput < 0) && isGrounded)// 플레이어가 지면에 있으면서 공격 가능 상태 일 경우
        {
            isAttacking= true;
            attackActive=false;
            // comboCounter++;
            // if (comboCounter > 2)
            // {
            //     comboCounter = 0;
            // }
            CameraShaker.Instance.ShakeCamera(2f, 0.1f);//공격시 카메라 흔들림 기능 추가.

            attackCooldownTimer=attackCooldown;
            Hit(AttackTransform, AttackArea);// Hit 메서드 호출하여 적 공격.
            StartCoroutine(AttackCooldown());//공격쿨타임(공격속도에 따른) 코루틴 호출
            GameObject slash = PhotonNetwork.Instantiate(slashEffect.name, AttackTransform.position, Quaternion.identity); // 슬래시 이펙트를 네트워크 상에서 생성
            if(!facingRight)// 만약 플레이어가 반대 방향을 바라볼 경우 슬래시 이펙트 반대 방향에 생성
            {
                slash.transform.Rotate(0f, 180f, 0f);
            }
            comboTimeWindow = comboTime;
            // isAttacking=false;
            // attackActive=false;
            // attackCooldownTimer=attackCooldown;

        // isAttacking = true;
        }
    }

    private IEnumerator AttackCooldown()
    {
        while (attackCooldownTimer >= 0)
        {
            // isAttacking=false;
            attackCooldownTimer -= Time.deltaTime;
            yield return null;
        }
        attackActive= true;//공격쿨타임 타이머가 0이하일 때 공격 상태 활성화
    }

    private IEnumerator UltimateAttackCooldown()
    {
        while (ultimateAttackCooldownTimer >= 0)
        {
            // isAttacking=false;
            ultimateAttackCooldownTimer -= Time.deltaTime;
            SkillUIManager.Instance.UpdateUltimate(ultimateAttackCooldownTimer);
            yield return null;
        }
        ultimateAttackActive= true;//공격쿨타임 타이머가 0이하일 때 공격 상태 활성화
    }

    //일반 공격 범위
    protected void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);//overlapBox 생성

        for (int i = 0; i < objectsToHit.Length; ++i)//overlapBox 내부에 영역 검사.
        {
            // if (objectsToHit[i].GetComponent<Enemy_Skeleton>() != null) // overlapBox 영역 내부에 적 존재 시 
            // if (objectsToHit[i].CompareTag("Enemy")) // overlapBox 영역 내부에 적 존재 시 
            if (objectsToHit[i].gameObject.layer == LayerMask.NameToLayer("Attackable")) // overlapBox 영역 내부에 적 존재 시 
            {
                // Enemy_Skeleton enemy = objectsToHit[i].GetComponent<Enemy_Skeleton>();
                var enemy = objectsToHit[i].GetComponent<PhotonView>();
                // if (!enemy.attackers.Contains(PV))
                // {
                //     enemy.attackers.Add(PV); // 플레이어의 PhotonView를 attackers 목록에 추가
                // }
                if(enemy!=null)
                {
                enemy.RPC("AddAttackerRPC", RpcTarget.AllBuffered, PV.ViewID);
                // enemy.PV.RPC("HitedRPC", RpcTarget.AllBuffered, damage, (Vector2)(transform.position - objectsToHit[i].transform.position));
                enemy.RPC("HitedRPC", RpcTarget.AllBuffered, damage, (Vector2)(transform.position - objectsToHit[i].transform.position));
                }
            
            }
        }
    }

    public void AttackOver()
    {

        if (isTakeDamage)
        {
            return; // 피격 상태에서는 공격 종료를 처리하지 않음
        }
        isAttacking = false;
        // attackActive=false;
        attackCooldownTimer=attackCooldown;
        comboCounter++;

        if (comboCounter > 2)
        {
            comboCounter = 0;
        }
    }

    //궁극기
    private void UltimateAttackEvent()
    {
        if (!isGrounded||isAttacking)
        {
            return; // 플레이어가 땅에 있지 않으면 아래 다 날리기.
        }
        if (ultimateSkillActivate &&ultimateAttackActive && (yInput == 0 || yInput < 0) && isGrounded)// 플레이어가 지면에 있으면서 공격 가능 상태 일 경우
        {
            isUltimateAttacking= true;
            invincible= true;
            ultimateAttackActive=false;

            ultimateAttackCooldownTimer=ultimateAttackCooldown;
            // Hit(AttackTransform, AttackArea);// Hit 메서드 호출하여 적 공격.
            StartCoroutine(UltimateAttackRoutine());
            StartCoroutine(UltimateAttackCooldown());//공격쿨타임(공격속도에 따른) 코루틴 호출
            GameObject ultimate = PhotonNetwork.Instantiate(ultimateEffect.name, AttackTransform.position, Quaternion.identity); // 슬래시 이펙트를 네트워크 상에서 생성
            if(!facingRight)// 만약 플레이어가 반대 방향을 바라볼 경우 슬래시 이펙트 반대 방향에 생성
            {
                ultimate.transform.Rotate(0f, 180f, 0f);
            }
        }
    }

    private IEnumerator UltimateAttackRoutine()
{
    while (isUltimateAttacking) // 궁극기 애니메이션이 끝날 때까지
    {
        Hit(AttackTransform, AttackArea); // 타격 발생
        CameraShaker.Instance.ShakeCamera(2f, 0.1f);//공격시 카메라 흔들림 기능 추가.
        yield return new WaitForSeconds(0.2f); // 타격 간격
    }
}

    public void UltimateAttackOver()
    {
        if (isTakeDamage)
        {
            return; // 피격 상태에서는 공격 종료를 처리하지 않음
        }
        isUltimateAttacking = false;
        invincible= false;
        ultimateAttackCooldownTimer=ultimateAttackCooldown;

    }

    private void CheckInput() // 사용자 키 입출력 관리
    {
        if(isPlayerDie)
        {
            return;
        }

        xInput = Input.GetAxisRaw("Horizontal"); // 수평 입력 관리
        yInput = Input.GetAxisRaw("Vertical"); // 수직 입력 관리
        
        if (EventSystem.current.IsPointerOverGameObject()) // UI 클릭시 게임 입력을 처리하지 않음
        {
            return; 
        }

        if (Input.GetKeyDown(KeyCode.Space)) //점프 입력 관리
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))// 대시 입력 관리
        {
            Dash();
        }
        
        if(isTakeDamage)// 캐릭터가 데미지를 입고 있다면 아래 입력 무시.
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0)) // 마우스 좌클릭으로 캐릭터 공격 시전
        {
            StartAttackEvent();// 공격속도 조건 필요
        }

        if (Input.GetKeyDown(KeyCode.T)) // T키를 누를 시에 힐 스킬 시전
        {
            StartHealing();
            Debug.Log("힐링 키 누름");
        }
        if (Input.GetKeyUp(KeyCode.T)) // T키를 뗄 시에 힐 스킬 중단
        {
            StopHealing();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            UltimateAttackEvent();
        }
    }

//대쉬 기능
    private void Dash() //대쉬 기능
    {
        if (dashActive && !isAttacking && dashSkillActivate) // 대쉬 조건
        {
            isDashing=true;
            dashActive=false;
            dashCooldownTimer = dashCooldown;
            dashTime = dashDuration;
            StartCoroutine(DashDuration()); // 대쉬 유지 시간 코루틴 호출
            StartCoroutine(DashCooldown()); // 대쉬 쿨타임 코루틴 호출
        }
    }

    private IEnumerator DashDuration() // 대쉬 유지시간 코루틴
    {
        while (dashTime >= 0) // 대쉬 시간이 0이하가 될 경우 탈출
        {
            dashTime -= Time.deltaTime;
            yield return null;
        }
        isDashing = false;
    }

    private IEnumerator DashCooldown() // 대쉬 쿨타임 코루틴
    {
        while (dashCooldownTimer >= 0) // 대쉬 쿨타임 타이머가 0이하가 될 경우 탈출
        {
            dashCooldownTimer -= Time.deltaTime;
            SkillUIManager.Instance.UpdateDash(dashCooldownTimer); //SkillUIManager에 해당 플레이어 대쉬 쿨타임 시간 넘겨줌.
            yield return null;
        }
        dashActive= true;
    }

    private void Movement()
    {
        if (isAttacking || isPlayerDie || isUltimateAttacking) // 플레이어가 공격중일 때
        {
            rb.velocity = new Vector2(0, 0);// 이동 불가.
        }
        else if (dashTime > 0) // 플레이어 대쉬 중일 때 
        {
            rb.velocity = new Vector2(facingDir * dashSpeed, 0);
        }
        else // 평소 이동 속도
        {
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }
    }

    [PunRPC]
    public void AddJumpCountRPC()
    {
        maxJumpCount = 3;
    }

    private void Jump() // 플레이어 점프 시.
    {
        if (isGrounded || jumpCount < maxJumpCount)// 플레이어가 땅에 있거나, 점프 카운트가 남아있을 경우
        {
            isJumping=true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }
    }

    private void JumpAbility() // 점프 조건
    {
        if (isGrounded && rb.velocity.y <= 0)
        {
            jumpCount = 0;
            isJumping=false;
        }
    }

//애니메이션 관리
    private void AnimatorController() //플레이어 애니메이션 관리 
    {
        anim.SetFloat("yVelopcity", rb.velocity.y);

        bool isMoving = rb.velocity.x != 0;
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isDashing", dashTime > 0);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("comboCounter", comboCounter);
        anim.SetBool("isTakeDamage", isTakeDamage);
        anim.SetBool("isPlayerDie", isPlayerDie);
        anim.SetBool("isHealing",isHealing);
        anim.SetBool("isUltimateAttacking",isUltimateAttacking);

        
    }

    private void PlayerWhenHited()// 플레이어가 무적 상태일 경우 효과 추가.
    {
        if (isTakeDamage && childSr != null)
        {
            float alpha = Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f);
            Color color = childSr.color;
            color.a = alpha;
            childSr.color = color;
        }
        else if (childSr != null)
        {
            Color color = childSr.color;
            color.a = 1.0f;
            childSr.color = color;
        }
    }

//뒤집기
    private void FlipController() // 캐릭터 좌,우 반전 기능.
    {
        if (rb.velocity.x > 0 && !facingRight)
        {
            PV.RPC("FlipRPC", RpcTarget.AllBuffered, true);

        }
        else if (rb.velocity.x < 0 && facingRight)
        {
            PV.RPC("FlipRPC", RpcTarget.AllBuffered, false);

        }
    }

    [PunRPC]
    private void FlipRPC(bool faceRight)// RPC로 네트워크 상에 해당 상태 적용
    {
        facingRight = faceRight;
        facingDir = faceRight ? 1 : -1;
        transform.Rotate(0, 180, 0);
        NickNameText.transform.Rotate(0, 180, 0);
        hpBar.transform.Rotate(0,180,0);
        mpBar.transform.Rotate(0,180,0);
        levelText.transform.Rotate(0,180,0);
    }

    public void TakeDamage(float _damage)
    {
        PV.RPC("TakeDamageRPC", RpcTarget.AllBuffered, _damage);
    }

    [PunRPC]
    private void TakeDamageRPC(float _damage)
    {
        if(PV.IsMine)// 내 플레이어일 경우
        {
            if(hp<=0 || invincible)
            {
                return;
            }
            Hp -= Mathf.RoundToInt(_damage); //hp 감소(damage의 소숫점 첫 번째 자리에서 반올림한 값을 int형으로 반환)
            isTakeDamage = true;
            isAttacking = false; // 공격중이더라도 피격시 공격 종료.
            StartCoroutine(StopTakeDamage());//데미지를 연속적으로 입는 것을 방지하기 위한 코루틴 호출
        }
    }

    
    private IEnumerator StopTakeDamage()
    {
        invincible = true;// 캐릭터 무적 상태
        //GameObject _bloodEffectParticle = Instantiate(bloodEffect, transform.position, Quaternion.identity);
        GameObject _bloodEffectParticle = PhotonNetwork.Instantiate(bloodEffect.name, transform.position, Quaternion.identity);//네트워크 상에 피격 파티클 생성
        StartCoroutine(DestroyAfter(_bloodEffectParticle, StopTakeDamageTime));//생성된 파티클을 1초 뒤 파괴
        yield return new WaitForSeconds(StopTakeDamageTime);// 1초 후 종료
        isTakeDamage = false;
        invincible = false;
        isCrushed = false;
    }
    

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(Hp);
            stream.SendNext(Mp);
            stream.SendNext(Exp);
            stream.SendNext(Level);
            stream.SendNext(maxHp);
            stream.SendNext(maxMp);

        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            Hp = (float)stream.ReceiveNext();
            Mp = (float)stream.ReceiveNext();
            Exp=(float)stream.ReceiveNext();
            Level=(float)stream.ReceiveNext();
            maxHp = (float)stream.ReceiveNext(); // 최대 체력 수신
            maxMp = (float)stream.ReceiveNext(); // 최대 마나 수신


        }
    }

    #region 체력회복 스킬 관련
    #region 1. 체력회복 시작 메서드
    private void StartHealing()
    {
        if (healSkillActivate && healActive && Hp < maxHp && !isJumping && !isDashing && !isTakeDamage && !isAttacking) //점프, 대쉬, 
        {
            isHealing=true;
            healCoroutine = StartCoroutine(HealCoroutine()); //healCoroutine변수에 해당 코루틴을 담아서 startHealing이 실행 돼야지만 stophealing이 작동되도록 함.
            // healActive=false;
        }
    }

    #endregion
    #region 2.체력회복 멈춤 메서드
    private void StopHealing()
    {
        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine); // 힐 스킬 코루틴 중단 
            healActive=false;
            isHealing=false;
            healChannelingTimer=healChanneling;
            healCoolTimer = healCooldown;
            StartCoroutine(HealCooldown()); // 힐 스킬 쿨타임 코루틴 호출

            healCoroutine = null;// healCoroutine 변수를 null 값으로 비워줘서 다음 호출에도 값을 씌울 수 있도록 함.
        }
    }

        private IEnumerator HealCooldown()
    {
        while (healCoolTimer >= 0)//힐 쿨타이머가 0 이하일 때 탈출.
        {
            healCoolTimer -= Time.deltaTime;
            SkillUIManager.Instance.UpdateHeal(healCoolTimer);// 힐 스킬 쿨타임동안 SkillUIManager에 힐 쿨타임 반영
            yield return null;
        }
        healActive= true;
    }

    #endregion

    #region 3.체력회복 코루틴
        private IEnumerator HealCoroutine()
    {
        while (Input.GetKey(KeyCode.T) && healActive)
        {
            healChannelingTimer -= healInterval; // 힐 코루틴 동안 힐 차징시간(T keydown 유지 시간)이 힐 인터벌 시간 만큼씩 감소
            PV.RPC("HealRPC", RpcTarget.AllBuffered, healAmount, healManaCost);
            GameObject _healEffect = PhotonNetwork.Instantiate(healEffect.name, transform.position, Quaternion.identity);
            StartCoroutine(DestroyAfter(_healEffect, 1.5f));
            if(healChannelingTimer<=0) // 힐 차징시간(T keydown 유지 시간)이 0 이하일 경우
            {
                healCoolTimer=healCooldown;
                StopHealing(); //힐 중단 메서드 호출
                yield break; // 해당 코루틴 탈출
            }
            yield return new WaitForSeconds(healInterval);
        }
        isHealing=false;
        healActive=false;
    }
    #endregion
    #region 4.RPC
    [PunRPC]
    private void HealRPC(int _healAmount, int _manaAmount)
    {
        if(PV.IsMine)
        {
            Mp -=Mathf.RoundToInt(_manaAmount);
            if(Hp+ _healAmount >maxHp)
            {
                Hp =maxHp;
            }
            else
            {
                Hp +=Mathf.RoundToInt(_healAmount);
            }
        }
    }
    #endregion

    #endregion

    #region 체력포션
    [PunRPC]
    private void HpPotionRPC(int _healAmount)
    {

        if(PV.IsMine)
        {                
            if(Hp+ _healAmount >maxHp) // 힐 포션 사용 시 최대 체력보다 더 많이 체력 회복할 때
            {
                Hp =maxHp;// 현재 체력을 최대체력으로 바꿈.
            }
            else
            {
                // Hp +=Mathf.RoundToInt(_healAmount); //힐량 만큼 힐.
                Hp +=_healAmount;

            }
        }
    }

    public void HpPotion(int _healAmount)
    {
        PV.RPC("HpPotionRPC",RpcTarget.AllBuffered, _healAmount);
    }
    #endregion

    #region 마나 포션

    [PunRPC]
    private void MpPotionRPC(int _ManaAmount)
    {
        if(PV.IsMine)
        {
            if(Mp+ _ManaAmount >maxMp)
            {
                Mp =maxMp;
            }
            else
            {
                // Mp +=Mathf.RoundToInt(_ManaAmount);
                Mp +=_ManaAmount;

            }
        } 
    }

    public void MpPotion(int _ManaAmount)
    {
        PV.RPC("MpPotionRPC",RpcTarget.AllBuffered, _ManaAmount);
    }


#endregion

    public void AddExp(float _ExpAmount)
    {
        PV.RPC("AddExpRPC", RpcTarget.AllBuffered, _ExpAmount);
    }

    [PunRPC]
    public void AddExpRPC(float _ExpAmount) //경험치 및 레벨업 시스템.
    {
        if(PV.IsMine)
        {
            if(Level == maxLevel)  //최대 레벨 도달 시 exp 예외 처리
            {
                Exp +=Mathf.RoundToInt(_ExpAmount);
                return;
            }
            if(Exp + _ExpAmount >= maxExp)// 경험치 획득량이 현 레벨 최대 경험치량 이상일 때(레벨업 시)
            {
                Exp += _ExpAmount - maxExp;
                PlayerLevelUp();// 레벨업 메서드 호출
                PV.RPC("UpdateLevelUIRPC", RpcTarget.AllBuffered, Level);

            }
            else
            {
                 Exp += Mathf.RoundToInt(_ExpAmount);
                //Exp += _ExpAmount;
            }
        }
    }

    private void PlayerLevelUp()
    {
        Level += 1; // 플레이어 레벨 1 업.
        maxExp *= 2; // 레벨업 시 경험치 통 직전 레벨에 비해 2배 증가
        // UIManager.Instance.UpdateEXP(exp,maxExp);
        maxHp*=1.1f; // 최대 체력을 이전보다 1.1배 증가
        maxMp*=1.1f; // 최대 마나를 이전보다 1.1배 증가
        LevelupStatPoint += 1; // 레벨업 시 스탯포인트를 1 증가시킴
        LevelupSkillPoint += 1; // 레벨업 시 스킬포인트를 1 증가시킴.
        Hp=maxHp; // 캐릭터 체력 최대체력으로 회복
        Mp=maxMp; // 캐릭터 마나를 최대 마나로 회복
    }

    [PunRPC]
    private void UpdateLevelUIRPC(float newLevel)
    {
        level = newLevel;
        LevelController(newLevel);
    }

// [PunRPC]
// public void HitedRPC(float _damageDone, Vector2 _hitDirection)
// {
//     Hited(_damageDone, _hitDirection);
// }

    public override float Hp
    {
        get { return hp; }
        set
        {
            if (hp != value)
            {
                hp = Mathf.Clamp(value, 0, maxHp);
                if (PV.IsMine)
                {
                    // UIManager.Instance.UpdateHP(hp, maxHp); // 보간 사용 안할시 해당 위치에서 사용하여 메모리 사용량 줄이기
                    StatUI.Instance.UpdateHP(maxHp);
                }
                HpBarController(hp);
            }
        }
    }

    public override float Mp
    {
        get { return mp; }
        set
        {
            if (mp != value)
            {
                mp = Mathf.Clamp(value, 0, maxMp);
                if (PV.IsMine)
                {
                    // UIManager.Instance.UpdateMP(mp, maxMp);
                    StatUI.Instance.UpdateMP(MaxMp);
                }
                MpBarController(mp);
            }
        }
    }

    public override float Exp
    {
        get { return exp; }
        set
        {
            if (exp != value)
            {
                exp = Mathf.Clamp(value, 0, maxExp);
                if (PV.IsMine)
                {
                    // UIManager.Instance.UpdateEXP(exp, maxExp);
                }
            }
        }
    }

    public override float Level
    {
        get { return level; }
        set
        {
            if (level != value)
            {
                level = Mathf.Clamp(value, 0, maxLevel);
                if (PV.IsMine)
                {
                    UIManager.Instance.UpdateLEVEL(level);
                    StatUI.Instance.UpdateLEVEL(level);
                }
            }
        }
    }

    public override float Damage
    {
        get { return damage; }
        set
        {
            if (damage != value)
            {
                damage = Mathf.Max(value, 0);
                if (PV.IsMine)
                {
                    StatUI.Instance.UpdateDamage(damage);
                }
            }
        }
    }

    public float LevelupStatPoint
    {
        get { return levelupStatPoint; }
        set
        {
            if (levelupStatPoint != value)
            {
                levelupStatPoint = Mathf.Max(value, 0);
                if (PV.IsMine)
                {
                    StatUI.Instance.UpdateStatPoint(levelupStatPoint);
                }
            }
        }
    }

    public float LevelupSkillPoint
    {
        get { return levelupSkillPoint; }
        set
        {
            if (levelupSkillPoint != value)
            {
                levelupSkillPoint = Mathf.Max(value, 0);
                if (PV.IsMine)
                {
                    SkillUI.Instance.UpdateSkillPoint(levelupSkillPoint);
                }
            }
        }
    }


    public override void HpBarController(float hp)
    {
        base.HpBarController(hp);
    }
    public override void MpBarController(float mp)
    {
        base.MpBarController(mp);
    }

}

