using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviourPunCallbacks
{
    protected Animator anim;
    protected Rigidbody2D rb;
    public float lerpTime = 5f;

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


    [Header("HP")]
    [SerializeField] protected float maxHp;
    
    [SerializeField] protected float hp;
    [SerializeField] public float damage;
    [SerializeField] protected Image hpBar;
    [SerializeField] protected TextMeshProUGUI HpText;
    public float hpFillAmount;


    [Header("Mana")]
    [SerializeField] protected float maxMp;

    [SerializeField] protected float mp;
    [SerializeField] protected Image mpBar;
    [SerializeField] protected TextMeshProUGUI MpText;
    public float mpFillAmount;

    [Header("Damaged")]
    public GameObject floatingDamageTextPrefab; // 프리팹 레퍼런스 추가
    public Transform DmgTextTransform;
    public float textMoveSpeed;
    public float textColorSpeed;
    public float textDestroyTime;

    [Header("Level")]
    [SerializeField] protected float level=1;
    [SerializeField] protected float maxLevel = 1000;
    [SerializeField] protected TextMeshProUGUI levelText;


    [Header("Exp")]

    [SerializeField] protected float maxExp =100;
    [SerializeField] protected float exp;
    // [SerializeField] protected Image ExpBar;
    // [SerializeField] protected TextMeshProUGUI ExpText;
    //[SerializeField] protected float experiencePoints= 80;
    public List<PhotonView> attackers = new List<PhotonView>();

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
        // HpBarController(hp); // 초기 HP바 설정
        mp = maxMp;
        // MpBarController(mp);
        if(hpBar != null)
        {
            hpFillAmount = hp / maxHp;
            hpBar.fillAmount = hpFillAmount;
        }

        if(mpBar != null )
        {
            mpFillAmount = mp / maxMp;
            mpBar.fillAmount = mpFillAmount;
        }
        exp=0;
        // ExpBarController(exp);
        LevelController(level);
    }

    protected virtual void Update()
    {
        CollisionCheck();
        HpBarController(hp);
        MpBarController(mp);
        // Heal();
    }

    protected virtual void CollisionCheck() //충돌 체크
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround); //레이캐스트를 통해 땅에 닿은 레이어가 Ground 시 땅으로 인식.
        isWallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance * facingDir, whatIsGround); //레이캐스트를 통해 벽에 닿은 레이어가 Ground 시 땅으로 인식.
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

    // protected virtual void Hit(Transform _attackTransform, Vector2 _attackArea)
    // {
    //     Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

    //     for (int i = 0; i < objectsToHit.Length; ++i)
    //     {
    //         if (objectsToHit[i].GetComponent<Enemy_Skeleton>() != null)
    //         {
    //             objectsToHit[i].GetComponent<Enemy_Skeleton>().Hited(damage, (transform.position - objectsToHit[i].transform.position).normalized);
    //         }
    //     }
    // }

    protected virtual void HpController()
    {
        if (Hp <= 0) // hp가 0 이하시 파괴.
        {
            if(photonView.IsMine || PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            // PhotonNetwork.Destroy(gameObject);
        }
    }

    public virtual float Hp
    {
        get { return hp; }
        set
        {
            if (hp != value)
            {
                hp = Mathf.Clamp(value, 0, maxHp);// Mathf.Clamp함수를 통해 체력의 최소값과 최대값 범위 결정.
                // HpBarController(hp); // HP가 변경될 때 HP바 업데이트
                HpController(); // HP가 변경될 때 HP 상태 확인
            }
        }
    }
    public virtual float MaxHp => maxHp;
    public virtual float MaxExp => maxExp;
    // public virtual float MaxLevel => maxLevel;
    public virtual void HpBarController(float hp)
    {
        if (hpBar != null)
        {
            hpFillAmount = Mathf.Lerp(hpFillAmount, hp/ maxHp, Time.deltaTime * lerpTime);
            hpBar.fillAmount = hpFillAmount;
            // hpBar.fillAmount = hp / maxHp;
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
                // MpBarController(mp); // HP가 변경될 때 HP바 업데이트
            }
        }
    }

    public virtual float MaxMp => maxMp;
    public virtual void MpBarController(float mp)
    {
        if (mpBar != null)
        {
            mpFillAmount = Mathf.Lerp(mpFillAmount, mp/ maxMp, Time.deltaTime * lerpTime);
            mpBar.fillAmount = mpFillAmount;
            // mpBar.fillAmount = mp / maxMp;
        }
        if (MpText != null)
        {
            MpText.text = mp.ToString("F0"); // 텍스트로 변환
        }
    }

    public virtual float Exp
    {
        get { return exp; }
        set
        {
            if (exp != value)
            {
                exp = Mathf.Clamp(value, 0, maxExp);
            }
        }
    }

     public virtual float Level
    {
        get { return level; }
        set
        {
            if (level != value)
            {
                level = Mathf.Clamp(value, 0, maxLevel);
            }
        }
    }

    public virtual float Damage
    {
        get { return damage; }
        set
        {
            if (damage != value)
            {
                damage = Mathf.Max(value, 0);
            }
        }
    }
    public virtual void LevelController(float level)
    {
        if (levelText != null)
        {
            levelText.text = "Lv " + level.ToString("F0");
        }
    }

    [PunRPC]
    protected void ShowDamageText(float _damage, Vector3 position)// 데미지 텍스트 생성
    {
        if (floatingDamageTextPrefab != null)
        {
            // PhotonNetwork.Instantiate를 사용하여 피해 텍스트 프리팹을 생성
            GameObject floatingText = PhotonNetwork.Instantiate(floatingDamageTextPrefab.name, position, Quaternion.identity);
            floatingText.GetComponent<FloatingDamageText>().Initialize(_damage);
            // StartCoroutine(DestroyAfter(floatingText,2.0f));
        }
    }

    public IEnumerator DestroyAfter(GameObject _gameObject, float _delay)// delay 시간 만큼 유지후 게임 오브젝트 파괴
    {  
        yield return new WaitForSeconds(_delay);
        if(_gameObject !=null)
        {
            PhotonNetwork.Destroy(_gameObject);
        }
    }


    
}
