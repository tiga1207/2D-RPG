 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   
    private Rigidbody2D rb;
     [SerializeField]private  float moveSpeed;
    [SerializeField]private float JumpForce;
    private float xInput; 

    private Animator anim;

    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;




    private int facingDir =1;
    private bool facingRight =true;   


    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        anim=GetComponentInChildren<Animator>();

    }

    void Update()
    {
        Movement();
        CheckInput();
        CollisionCheck();

        Debug.Log(isGrounded);
        AnimatorController();
        FlipController();



    }

    private void CollisionCheck()
    {

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);

        //캡슐 콜라이더로 체크할 경우 아래 코드
        // isGrounded = Physics2D.Raycast(transform.position, Vector2.down, capsuleCollider2D.size.y/2f +0.5f, whatIsGround);

    }

    //
    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    //좌우 움직임 컨트롤
    private void Movement()
    {
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    //점프
    private void Jump()
    {
        if(isGrounded)
            {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            }
    }

    //캐릭터 애니메이션
    private void AnimatorController()
    {
        anim.SetFloat("yVelopcity",rb.velocity.y);

        //움직임 관련 애니메이션
        bool isMoving = rb.velocity.x != 0;
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isGrounded",isGrounded);
    }

    private void Filp(){
        facingDir = facingDir*-1;
        facingRight= !facingRight;
        transform.Rotate(0,180,0);
    }

    private void FlipController(){
        if(rb.velocity.x>0 && !facingRight){
            Filp();
        }
        else if(rb.velocity.x<0 && facingRight){
            Filp();
        }
    }

 
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
    }



}

  