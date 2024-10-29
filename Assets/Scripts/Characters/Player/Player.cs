using System.Collections;
using UnityEngine;

public class Player : Character
{
    public Dive dive;
    private Sword sword;
    private bool isInGate;

    [SerializeField]
    private ParticleSystem dust;
    [SerializeField]
    private Bar coolDownBar;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        animator.SetTrigger(AnimationParametre.Spawn.ToString());
        sword = GetComponentInChildren<Sword>();

        healtBar.SetMaxValue(stat.health);
        coolDownBar.SetMaxValue(1);
        coolDownBar.animationDuration = dash.dashingCooldown * 2;
    }

    private void Update()
    {
        if (isDeath) return;
        if (dash.isDashing) return;
        if (knockback.isKnockback) return;
        if (dive.isDiving) return;

        float moveInput = Input.GetAxis("Horizontal");
        Move(moveInput);
        Turn(moveInput);
        Jump(moveInput);
        Dash();
        Attack();
        Dive();
        NextLevel();
    }

    private void Dive()
    {
        if (Input.GetKeyDown(KeyCode.S) && !isGrounded)
        {
            StartCoroutine(dive.DiveCoroutine(rb, tr, dust));
        }
    }

    protected override void Move(float moveInput)
    {
        rb.velocity = new Vector2(moveInput * stat.moveSpeed, rb.velocity.y);
        animator.SetFloat(AnimationParametre.Velocity, Mathf.Abs(moveInput));
    }

    protected override void Turn(float moveInput)
    {
        float lastLocalScale = transform.localScale.x;
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(moveInput > 0 ? 1 : -1, 1, 1);
        }

        if (lastLocalScale != transform.localScale.x && isGrounded)
        {
            dust.Play();
        }
    }

    protected override void Jump(float moveInput)
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            dust.Play();
            rb.velocity = new Vector2(rb.velocity.x, stat.jumpForce);
        }

        animator.SetBool(AnimationParametre.IsJumpIdle.ToString(), !isGrounded && moveInput == 0);
        animator.SetBool(AnimationParametre.IsJumpRun.ToString(), !isGrounded && moveInput != 0);
    }

    protected override void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && dash.canDash)
        {
            coolDownBar.SetValue(0);
            animator.SetTrigger(AnimationParametre.Dash.ToString());
            if (!isGrounded)
            {
                animator.SetBool(AnimationParametre.IsJumpIdle.ToString(), false);
                animator.SetBool(AnimationParametre.IsJumpRun.ToString(), false);
            }
            StartCoroutine(dash.DashCoroutine(rb, transform, tr));
            StartCoroutine(SetCoolDownValueCoroutine());
        }
    }

    private IEnumerator SetCoolDownValueCoroutine()
    {
        yield return new WaitForSeconds(dash.dashingCooldown + dash.dashingTime);
        coolDownBar.SetValue(1);
    }

    protected override void Attack()
    {
        sword.Attack();
    }

    public new void Hit(float damage)
    {
        FindObjectOfType<CameraController>().TriggerShake(0.1f);
        base.Hit(damage);
    }

    private void NextLevel()
    {
        if (Input.GetKeyDown(KeyCode.E) && isInGate)
        {
            LevelManager.Instance.CompleteLevel();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Character character))
        {
            if (character.CompareTag(Tag.Goblin) && character.dash.isDashing)
            {
                Hit(character.stat.damage);
                Vector2 direction = (transform.position - other.transform.position).normalized;
                StartCoroutine(knockback.KnockbackCoroutine(rb, direction));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(Tag.Gate))
        {
            isInGate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(Tag.Gate))
        {
            isInGate = false;
        }
    }
}