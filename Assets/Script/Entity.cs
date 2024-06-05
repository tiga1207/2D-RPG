using Photon.Pun;
using UnityEngine;

public class Entity : MonoBehaviourPunCallbacks
{
    protected Animator anim;
    protected Rigidbody2D rb;

    [Header("Collision info")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [Space]
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    protected bool isGrounded;
    protected bool isWallDetected;

    [Header("Attack")]
    [SerializeField] protected Transform AttackTransform;
    [SerializeField] protected Vector2 AttackArea;
    [SerializeField] protected LayerMask attackableLayer;
    [SerializeField] protected GameObject slashEffect;

    [Header("HP")]
    [SerializeField] protected float maxHp;
    [SerializeField] protected float hp;
    [SerializeField] protected float damage;

    protected int facingDir = 1;
    protected bool facingRight = true;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

    }

    protected virtual void Start()
    {
        if (wallCheck == null)
        {
            wallCheck = transform;
        }
    }

    protected virtual void Update()
    {
        CollisionCheck();
    }

    protected virtual void CollisionCheck()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance * facingDir, whatIsGround);
    }

    protected virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireCube(AttackTransform.position, AttackArea);
    }

    protected virtual void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        for (int i = 0; i < objectsToHit.Length; ++i)
        {
            if (objectsToHit[i].GetComponent<Enemy_Skeleton>() != null)
            {
                objectsToHit[i].GetComponent<Enemy_Skeleton>().Hited(damage, (transform.position - objectsToHit[i].transform.position).normalized);
            }
        }
    }

    protected virtual void HpController()
    {
        if (Hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual float Hp
    {
        get { return hp; }
        set
        {
            if (hp != value)
            {
                hp = Mathf.Clamp(value, 0, maxHp);
            }
        }
    }

    protected virtual void Hited(float _damageDone, Vector2 _hitDirection)
    {
        Hp -= _damageDone;
    }
}
