using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;

    [Header("Collision info")]
    [SerializeField] protected Transform groundCheck; 
    [SerializeField] protected float groundCheckDistance;  
    [SerializeField] protected  LayerMask whatIsGround;

    protected bool isGrounded;


    protected int facingDir = 1;
    protected  bool facingRight = true; 

    protected virtual  void Start()
    {
        anim=GetComponentInChildren<Animator>();
        rb=GetComponent<Rigidbody2D>();  

    }

    // Update is called once per frame
    protected virtual   void Update()
    {
        CollisionCheck(); 
    }

    protected virtual void CollisionCheck()
    {

        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        //캡슐 콜라이더로 체크할 경우 아래 코드
        // isGrounded = Physics2D.Raycast(transform.position, Vector2.down, capsuleCollider2D.size.y/2f +0.5f, whatIsGround);

    }

    protected virtual void Filp()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
    }
}
