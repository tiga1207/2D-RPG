using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;

    [Header("Collision info")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [Space]
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected  LayerMask whatIsGround;
    
    protected bool isGrounded;
    protected bool isWallDeteted;

    [Header("Attack")]
    [SerializeField]protected Transform AttackTransform;
    [SerializeField]protected Vector2 AttackArea;
    [SerializeField]protected LayerMask attackableLayer;
    [SerializeField]protected GameObject slashEffect;


    [Header("HP")]
    [SerializeField]protected float hp;
    [SerializeField]protected float damage;





    protected int facingDir = 1;
    protected  bool facingRight = true;




    protected virtual  void Start()
    {
        anim=GetComponentInChildren<Animator>();
        rb=GetComponent<Rigidbody2D>();  


        // 플레이어에게 wallCheck을 따로 할당 안해주면 오류가 생기는 것을 방지해줌
        if(wallCheck==null){
            wallCheck=transform;
        }

    }

    // Update is called once per frame
    protected virtual   void Update()
    {
        CollisionCheck();
    }

    protected virtual void CollisionCheck()// Ground & Wall 체크
    {

        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDeteted=Physics2D.Raycast(wallCheck.position,Vector2.right, wallCheckDistance*facingDir, whatIsGround);

        //캡슐 콜라이더로 체크할 경우 아래 코드
        // isGrounded = Physics2D.Raycast(transform.position, Vector2.down, capsuleCollider2D.size.y/2f +0.5f, whatIsGround);

    }


    protected virtual void Filp() //캐릭터 좌.우 뒤집기
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    protected virtual void OnDrawGizmos() //Ground & Wall 체크 기즈모(선)
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance*facingDir , wallCheck.position.y));
        Gizmos.DrawWireCube(AttackTransform.position, AttackArea);
    }
    protected virtual void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0,attackableLayer);
        if(objectsToHit.Length >0){
             Debug.Log("hit");
        }
        for(int i=0; i< objectsToHit.Length; ++i){
            if(objectsToHit[i].GetComponent<Enemy_Skeleton>()!= null)
            {
                objectsToHit[i].GetComponent<Enemy_Skeleton>().Hited(damage);
            }
        }
    }

    protected void HpController(){
        if(hp<=0){
            Destroy(gameObject);
        }
    }
    protected void Hited(float _damageDone){
        hp-=_damageDone;
        Debug.Log("Enemy HP"+hp);
    }
}
