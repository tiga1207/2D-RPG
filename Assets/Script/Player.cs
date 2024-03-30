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
    // [SerializeField]private bool isMoving;


    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        anim=GetComponentInChildren<Animator>();

    }

    void Update()
    {
        Movement();
        CheckInput();
        AnimatorController();

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
        rb.velocity = new Vector2(rb.velocity.x, JumpForce);
    }

    //캐릭터 애니메이션
    private void AnimatorController()
    {
        //움직임 관련 애니메이션
        bool isMoving = rb.velocity.x != 0;
        anim.SetBool("isMoving", isMoving);
    }
}
  