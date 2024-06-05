using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private float dashTime;
    [SerializeField] private float dashCooldown;
    private float dashCooldownTimer;

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

    [Header("Photon")]
    public TMP_Text NickNameText;
    public Image HealthImage;
    public PhotonView PV;

    protected override void Awake()
    {
        base.Awake();

        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

        if (sr != null)
        {    
            childSr = sr.GetComponent<SpriteRenderer>();
        }
       
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
        }
    }

    private void CooldownManager()
    {
        dashTime -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;
        comboTimeWindow -= Time.deltaTime;
    }

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
            Instantiate(slashEffect, AttackTransform);
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

    private void CheckInput()
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
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }
    }

    private void JumpAbility()
    {
        if (isGrounded && rb.velocity.y <= 0)
        {
            jumpCount = 0;
        }
    }

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

    }

    [PunRPC]
    public virtual void TakeDamageRPC(float _damage)
    {
        Hp -= Mathf.RoundToInt(_damage);
        isTakeDamage = true;
        StartCoroutine(StopTakeDamage());
    }

    private IEnumerator StopTakeDamage()
    {
        invincible = true;
        //GameObject _bloodEffectParticle = Instantiate(bloodEffect, transform.position, Quaternion.identity);
        GameObject _bloodEffectParticle = PhotonNetwork.Instantiate(bloodEffect.name, transform.position, Quaternion.identity);
        Destroy(_bloodEffectParticle, 1.5f);
        yield return new WaitForSeconds(1f);
        isTakeDamage = false;
        invincible = false;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(Hp);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            Hp = (float)stream.ReceiveNext();
        }
    }
}
