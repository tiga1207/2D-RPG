using System.Collections;
using UnityEngine;

public class Player : Entity
{
    // private Rigidbody2D rb;
    // private Animator anim;
    private float xInput,yInput;

    [Header("Move Info ")]
     [SerializeField] private float moveSpeed;
     [SerializeField] private float gravity=3.5f;
    [SerializeField] private float JumpForce;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    private float dashTime;
    [SerializeField] private float dashCooldown;
    private float dashCooldownTimer;


    [Header("Attack Info")]
    [SerializeField]private float comboTime=.3f;
    private float comboTimeWindow;
    [SerializeField]private bool isAttacking;
    private int comboCounter;

    public bool isTakeDamage = false;


 
    [Header("JumpAbilty")]
    [SerializeField]private int jumpCount=0;
    [SerializeField] private int maxJumpCount = 2; // 최대 점프 횟수

    // [Header("Recoil")]
    
    // [SerializeField]private int recoilXStep =5;
    // [SerializeField]private int recoilYStep =5;
    // [SerializeField]private float recoilXSpeed =100;
    // [SerializeField]private float recoilYSpeed =100;
    // int stepXRecoild, stepYRecoild;

    // 싱글톤
    [Header("Camera")]
    public static Player Instance;
    // [SerializeField] public bool invincible =false;


    protected override void Awake()
    {
        base.Awake();
        if(Instance != null && Instance != this)
       {
           Destroy(gameObject);
       }
       else
       {
           Instance = this;
       }
    //    Hp = maxHp;
    }
    protected override void Start()
    {
        base.Start();
          
    }
    protected override void Update()
    {
        base.Update();
        Movement();
        JumpAbilty();
        CheckInput();
        CooldownManager();
        AnimatorController();
        FlipController();

        // Recoil();
    }

    private void CooldownManager()
    {
        dashTime -= Time.deltaTime; //대시 쿨다운
        dashCooldownTimer -= Time.deltaTime;// 대시 쿨다운
        comboTimeWindow -= Time.deltaTime;// 콤보 쿨다운
    }

    private void StartAttackEvent() //공격 시작시점
    {     
        if(!isGrounded)
        {
            return;//이 시점에서 함수 완료 및 그 아래 있는 문들을 실행시키지 않도록 함
        }

        if (comboTimeWindow < 0)
        {
            comboCounter = 0;
        }
        if(yInput==0 || yInput<0 && isGrounded){

            Vector2 attackDirection = facingRight ? Vector2.right : Vector2.left;
            
            // Hit(AttackTransform,AttackArea,ref recoilX,recoilXSpeed); // 공격 이펙트와 별개로 데미지 적용 문제 해결해야함.
            Hit(AttackTransform,AttackArea); // 공격 이펙트와 별개로 데미지 적용 문제 해결해야함.
            Instantiate(slashEffect,AttackTransform);
        }
        isAttacking = true;
        comboTimeWindow = comboTime;
    }

    public void AttackOver() //공격 종료 시점
    {
        isAttacking=false;
        comboCounter++;

        if(comboCounter > 2)
        {
            comboCounter = 0;
        }        
    }


    private void CheckInput() //사용자 입력 조작
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical"); 
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartAttackEvent();
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashAbility();
        }

    }

  
    private void DashAbility()
    {
        if (dashCooldownTimer < 0 && !isAttacking)
        {
            dashCooldownTimer = dashCooldown;
            dashTime = dashDuration;
        }
    }

    //좌우 움직임 컨트롤
    private void Movement()
    {
        if(isAttacking)
        {
            rb.velocity = new Vector2(0, 0);
        }

        else if (dashTime > 0)
        {
            //rb.velocity = new Vector2(xInput * dashSpeed, rb.velocity.y);
            rb.velocity = new Vector2(facingDir * dashSpeed, 0);
        }
        else
        {
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }

    }

    //점프
    private void Jump()
    {
        if (isGrounded || jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            jumpCount++;
        }

    }

    private void JumpAbilty()
    {
        if (isGrounded && rb.velocity.y <= 0)
        {
            jumpCount = 0;
        }
    }

    //캐릭터 애니메이션
    private void AnimatorController()
    {
        anim.SetFloat("yVelopcity", rb.velocity.y);

        //움직임 관련 애니메이션
        bool isMoving = rb.velocity.x != 0;
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isDashing", dashTime > 0);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("comboCounter", comboCounter);
        anim.SetBool("isTakeDamage",isTakeDamage);



    }

    private void FlipController()
    {
        if (rb.velocity.x > 0 && !facingRight)
        {
            Filp();
        }
        else if (rb.velocity.x < 0 && facingRight)
        {
            Filp();
        }
    }

    protected override void CollisionCheck()
    {
        base.CollisionCheck();
    }

    // public virtual void ClampHp()
    // {
    //     hp = Mathf.Clamp(hp, 0, maxHp);
    // }

  
    public virtual void TakeDamage(float _damage)
    {
        Hp -= Mathf.RoundToInt(_damage);
        isTakeDamage= true;
        StartCoroutine(StopTakeDamage());

    }

    IEnumerator StopTakeDamage()
    {
        invincible =true;
        // ClampHp();
        yield return new WaitForSeconds(1f);
        isTakeDamage = false;
        invincible = false;
    }



    /*
          void Recoil()
        {   
            if(recoilX)
            {
                if(facingRight)
                {
                    rb.velocity = new Vector2(-recoilXSpeed,0);
                }
                else
                {
                    rb.velocity = new Vector2(recoilXSpeed,0);
                }
            }
            if(recoilY)
            {
                if(yInput < 0){
                    rb.gravityScale =  0;
                    rb.velocity = new Vector2(rb.velocity.x,recoilYSpeed);
                }
                else{
                    rb.velocity = new Vector2(rb.velocity.x,-recoilYSpeed);

                }
                jumpCount =0 ; 
            }
            else
            {
                rb.gravityScale= gravity;
            }

            //recoil 중단 조건

            if(recoilX && stepXRecoild< recoilXStep)
            {
                stepXRecoild ++;
            }
            else
            {
                StopXRecoil();
            }

            if(recoilY && stepYRecoild< recoilYStep)
            {
                stepYRecoild ++;
            }
            else
            {
                StopYRecoil();
            }
            if(isGrounded)
            {
                StopYRecoil();

            }
        }

        private void StopXRecoil(){
            stepXRecoild=0;
            recoilX=false;      
        }

        private void StopYRecoil(){
            stepYRecoild=0;
            recoilY=false; 
        }

    */
}

