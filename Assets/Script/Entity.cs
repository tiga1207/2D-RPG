using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] protected Image hpBar;
    [SerializeField] protected TextMeshProUGUI HpText;


    [Header("Mana")]
    [SerializeField] protected float maxMp;
    [SerializeField] protected float mp;
    [SerializeField] protected Image mpBar;
    [SerializeField] protected TextMeshProUGUI MpText;

    

    protected int facingDir = 1;
    protected bool facingRight = true;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (wallCheck == null)
        {
            wallCheck = transform;
        }
    }

    protected virtual void Start()
    {
        hp = maxHp; // 초기 HP 설정
        HpBarController(hp); // 초기 HP바 설정
        mp = maxMp;
        MpBarController(mp);
    }

    protected virtual void Update()
    {
        CollisionCheck();
        // Heal();
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
                HpBarController(hp); // HP가 변경될 때 HP바 업데이트
                HpController(); // HP가 변경될 때 HP 상태 확인
            }
        }
    }

    public virtual void HpBarController(float hp)
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = hp / maxHp;
        }
        if (HpText != null)
        {
            HpText.text = hp.ToString("F0"); // 텍스트로 변환
        }
    }

    public virtual float Mp
    {
        get { return mp; }
        set
        {
            if (mp != value)
            {
                mp = Mathf.Clamp(value, 0, maxMp);
                MpBarController(mp); // HP가 변경될 때 HP바 업데이트
            }
        }
    }

    public virtual void MpBarController(float mp)
    {
        if (mpBar != null)
        {
            mpBar.fillAmount = mp / maxMp;
        }
        if (MpText != null)
        {
            MpText.text = mp.ToString("F0"); // 텍스트로 변환
        }
    }

    // public virtual void Heal()
    // {
    //     if(Input.GetKeyDown(KeyCode.T) && isGrounded)//점프 및 대쉬 상태가 아니어야함.
    //     {
    //         //
    //         healCoolTimer+=Time.deltaTime;
    //         if(healCoolTimer>=healingTime)
    //         {
    //             hp++;
    //             healCoolTimer=0;  
    //         }
    //         else
    //         {
    //             healCoolTimer=0;
    //         }
    //     }

    // }

    protected virtual void Hited(float _damageDone, Vector2 _hitDirection)
    {
        Hp -= _damageDone;
    }
}
