using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.EventSystems;

public class Player : Entity, IPunObservable
{
    private float xInput, yInput;
    private SpriteRenderer childSr;

    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gravity = 3.5f;
    [SerializeField] private float jumpForce;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    private float dashTime;
    private float dashCooldownTimer;
    public bool isDashing = false;

    [Header("Attack Info")]
    [SerializeField] private float comboTime = 0.3f;
    private float comboTimeWindow;
    [SerializeField] private bool isAttacking;
    private int comboCounter;

    [Header("Player State")]
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private float hitFlashSpeed;
    [SerializeField] public bool invincible = false;
    public bool isTakeDamage = false;
    [SerializeField] private GameObject sr;


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

    public bool isHealing = false; // 힐 스킬 사용 여부
    public int healAmount = 1; //힐량
    public int healManaCost = 3;
    [SerializeField] protected float healCooldown= 5; //쿨타임 시간.
    [SerializeField] protected float  healCoolTimer=5; //쿨타임 타이머
    [SerializeField] protected float  healChanneling= 2; // 정신집중 시간.
    [SerializeField] protected float  healChannelingTimer =2; // 정신집중 시간.

    [SerializeField] protected float healInterval = 1; // 힐 간격
    private Coroutine healCoroutine;

    private Collider2D playerCollider;

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
        }

        //플레이어끼리의 충돌 방지
        int playerLayer = LayerMask.NameToLayer("Player");
        Physics2D.IgnoreLayerCollision(playerLayer, playerLayer);

        if (PV.IsMine)
        {
            photonView.RPC("InitializeInventory", RpcTarget.AllBuffered); // 인벤토리 초기화
        }
        
       
    }

    [PunRPC]
    public void InitializeInventory()
    {
        playerInventory.Initialize();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (PV.IsMine)
        {
            Movement();
            JumpAbility();
            CheckInput();
            CooldownManager();
            AnimatorController();
            FlipController();
            FlashWhileInvincible();
            PlayerHpController();
            DashAbility();
        }
    }

    private void PlayerHpController()
    {
        if (Hp <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
            GameObject.Find("Canvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
        }
    }

    private void CooldownManager()
    {
        dashTime -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;
        comboTimeWindow -= Time.deltaTime;
        healCoolTimer-=Time.deltaTime;// 힐 쿨타임. 0보다 작으면 사용가능.

    }
//공격
    private void StartAttackEvent()
    {
        if (!isGrounded)
        {
            return;
        }

        if (comboTimeWindow < 0)
        {
            comboCounter = 0;
        }
        if (yInput == 0 || yInput < 0 && isGrounded)
        {
            Hit(AttackTransform, AttackArea);
            GameObject slash = PhotonNetwork.Instantiate(slashEffect.name, AttackTransform.position, Quaternion.identity); // 슬래시 이펙트를 네트워크 상에서 생성
            if(!facingRight)
            {
                slash.transform.Rotate(0f, 180f, 0f);
            }
            // Instantiate(slashEffect, AttackTransform);
        }
        isAttacking = true;
        comboTimeWindow = comboTime;
    }

    public void AttackOver()
    {
        isAttacking = false;
        comboCounter++;

        if (comboCounter > 2)
        {
            comboCounter = 0;
        }
    }

//키 입출력
    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        
        if (EventSystem.current.IsPointerOverGameObject()) // UI가 클릭시 게임 입력을 처리하지 않음
        {
            return; 
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
        
        if(isTakeDamage)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartAttackEvent();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartHealing();
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            StopHealing();
        }
    }

//대시 기능
    private void Dash()
    {
        if (dashCooldownTimer < 0 && !isAttacking)
        {
            isDashing=true;
            dashCooldownTimer = dashCooldown;
            dashTime = dashDuration;
        }
    }

    private void DashAbility(){
        if(dashTime<0)
        {
            isDashing=false;
        }
    }

    private void Movement()
    {
        if (isAttacking)
        {
            rb.velocity = new Vector2(0, 0);
        }
        else if (dashTime > 0)
        {
            rb.velocity = new Vector2(facingDir * dashSpeed, 0);
        }
        else
        {
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }
    }

    private void Jump()
    {
        if (isGrounded || jumpCount < maxJumpCount)
        {
            isJumping=true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }
    }

    private void JumpAbility()
    {
        if (isGrounded && rb.velocity.y <= 0)
        {
            jumpCount = 0;
            isJumping=false;
        }
    }

//애니메이션 관리
    private void AnimatorController()
    {
        anim.SetFloat("yVelopcity", rb.velocity.y);

        bool isMoving = rb.velocity.x != 0;
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isDashing", dashTime > 0);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("comboCounter", comboCounter);
        anim.SetBool("isTakeDamage", isTakeDamage);
    }

    private void FlashWhileInvincible()
    {
        if (invincible && childSr != null)
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
    private void FlipController()
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
    private void FlipRPC(bool faceRight)
    {
        facingRight = faceRight;
        facingDir = faceRight ? 1 : -1;
        transform.Rotate(0, 180, 0);
        NickNameText.transform.Rotate(0, 180, 0);
        hpBar.transform.Rotate(0,180,0);
        mpBar.transform.Rotate(0,180,0);
    }

    public void TakeDamage(float _damage)
    {
        PV.RPC("TakeDamageRPC", RpcTarget.AllBuffered, _damage);
    }

    [PunRPC]
    private void TakeDamageRPC(float _damage)
    {
        if(PV.IsMine)
        {
            Debug.Log("TakeDamageRPC working");
            Hp -= Mathf.RoundToInt(_damage);
            isTakeDamage = true;
            StartCoroutine(StopTakeDamage());
        }
    }

    
    private IEnumerator StopTakeDamage()
    {
        invincible = true;
        //GameObject _bloodEffectParticle = Instantiate(bloodEffect, transform.position, Quaternion.identity);
        GameObject _bloodEffectParticle = PhotonNetwork.Instantiate(bloodEffect.name, transform.position, Quaternion.identity);
        StartCoroutine(DestroyAfter(_bloodEffectParticle, 1.5f));
        yield return new WaitForSeconds(1f);
        isTakeDamage = false;
        invincible = false;
    }
    private IEnumerator DestroyAfter(GameObject _gameObject, float _delay)
    {  
        yield return new WaitForSeconds(_delay);
        if(_gameObject !=null)
        {
            PhotonNetwork.Destroy(_gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(Hp);
            stream.SendNext(Mp);

        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            Hp = (float)stream.ReceiveNext();
            Mp = (float)stream.ReceiveNext();
        }
    }

    #region 체력회복 스킬 관련
    #region 1. 체력회복 시작 메서드
    private void StartHealing()
        {
            if (healCoolTimer <= 0 && Hp < maxHp)
            {
                isHealing=true;
                healCoroutine = StartCoroutine(HealCoroutine());
            }
        }

    #endregion
    #region 2.체력회복 멈춤 메서드
        private void StopHealing()
        {
            if (healCoroutine != null)
            {
                StopCoroutine(healCoroutine);
                healCoroutine = null;
            }
                isHealing=false;
                healChannelingTimer=healChanneling;
                if(healCoolTimer<0)
                {
                    healCoolTimer=healCooldown;
                }
        }

    #endregion

    #region 3.체력회복 코루틴
        private IEnumerator HealCoroutine()
    {
        while (Input.GetKey(KeyCode.T) && healCoolTimer <= 0)
        {
            healChannelingTimer -= healInterval;
            PV.RPC("HealRPC", RpcTarget.AllBuffered, healAmount, healManaCost);
            GameObject _healEffect = PhotonNetwork.Instantiate(healEffect.name, transform.position, Quaternion.identity);
            StartCoroutine(DestroyAfter(_healEffect, 1.5f));
            if(healChannelingTimer<=0)
            {
                healCoolTimer=healCooldown;
                StopHealing();
                yield break;
            }
            yield return new WaitForSeconds(healInterval);
        }
        isHealing=false;
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
                Mp +=Mathf.RoundToInt(_ManaAmount);
            }
        } 
    }

    public void MpPotion(int _ManaAmount)
    {
        PV.RPC("MpPotionRPC",RpcTarget.AllBuffered, _ManaAmount);
    }


#endregion


// public void UseItem(ItemEffect item)
// {
//     if(item == null || playerInventory == null)
//     {
//         return;
//     }
//     item.ExecuteRole(this);
// }

}