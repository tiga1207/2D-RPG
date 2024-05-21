using UnityEngine;

public class Player : Entity
{
    // private Rigidbody2D rb;
    // private Animator anim;

    [Header("Move Info ")]
     [SerializeField] private float moveSpeed;
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

    private float xInput;


    // [Header("Collision info")]
    // [SerializeField] private float groundCheckDistance;  
    // [SerializeField] private LayerMask whatIsGround;
    // private bool isGrounded;
  



    // private int facingDir = 1;
    // private bool facingRight = true;

    protected override void Start()
    {
        base.Start();
          
    }

    // void Start()
    // {
    //     // rb = GetComponent<Rigidbody2D>();
    //     // anim = GetComponentInChildren<Animator>();

    // }

    protected override void Update()
    {
        base.Update(); 
        Movement();
        CheckInput();
        // CollisionCheck();

        dashTime -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;
        comboTimeWindow -= Time.deltaTime;
        AnimatorController();
        FlipController();



    }

    public void AttackOver()
    {
        isAttacking=false;
        comboCounter++;

        if(comboCounter > 2)
        {
            comboCounter = 0;
        }
        
    }



    //
    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
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

    private void StartAttackEvent()
    {
        
        if(!isGrounded)
        {
            return;//이 시점에서 함수 완료 및 그 아래 있는 문들을 실행시키지 않도록 함
        }

        if (comboTimeWindow < 0)
        {
            comboCounter = 0;
        }


        isAttacking = true;
        comboTimeWindow = comboTime;
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
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
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


    }

    // private void Filp()
    // {
    //     facingDir = facingDir * -1;
    //     facingRight = !facingRight;
    //     transform.Rotate(0, 180, 0);
    // }

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


    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
    // }



}

