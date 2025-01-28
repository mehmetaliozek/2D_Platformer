using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Character
{
    public Dive dive;
    private Sword sword;
    private bool isInGate;
    private bool isGetKey;

    [SerializeField]
    private ParticleSystem dust;
    [SerializeField]
    private Bar coolDownBar;

    [SerializeField]
    private GameObject infoPanelGate;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        animator.SetTrigger(AnimationParametre.Spawn.ToString());
        sword = GetComponentInChildren<Sword>();

        //healtBar = GameObject.FindGameObjectWithTag(Tag.HealthBar.ToString()).GetComponent<Bar>();
        //coolDownBar = GameObject.FindGameObjectWithTag(Tag.CooldownBar.ToString()).GetComponent<Bar>();

        healtBar.SetMaxValue(stat.health);
        coolDownBar.SetMaxValue(1);
        coolDownBar.animationDuration = dash.dashingCooldown * 2;
    }

    private void Update()
    {
        if (isInGate)
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            Vector3 objScale = transform.localScale / 2;

            if (viewportPos.y - objScale.y > 1)
            {
                LevelManager.Instance.CompleteLevel();
                isInGate = false;
            }
            return;
        }
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
    }

    private void Dive()
    {
        if (Input.GetKeyDown(KeyCode.S) && !isGrounded)
        {
            StartCoroutine(dive.DiveCoroutine(rb, tr, dust, this));
        }
    }

    protected override void Move(float moveInput)
    {
        rb.linearVelocity = new Vector2(moveInput * stat.moveSpeed, rb.linearVelocity.y);
        animator.SetFloat(AnimationParametre.Velocity.ToString(), Mathf.Abs(moveInput));
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stat.jumpForce);
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
        FindAnyObjectByType<CameraController>().TriggerShake(0.1f);
        base.Hit(damage);
    }

    public void SetKey()
    {
        isGetKey = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (enemy.CompareTag(Tag.Enemy.ToString()) && enemy.dash.isDashing)
            {
                Hit(enemy.stat.damage);
                Vector2 direction = (transform.position - other.transform.position).normalized;
                StartCoroutine(knockback.KnockbackCoroutine(rb, direction));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(Tag.Gate.ToString()) && isGetKey)
        {
            isInGate = true;
            Camera.main.GetComponent<CameraController>().LevelEnding();
            rb.linearVelocity = Vector2.zero;
            rb.AddForceY(50f, ForceMode2D.Impulse);

            int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Split(" ")[1]);
            int openedLevelCount = PlayerPrefs.GetInt(PlayerPrefKey.OpenedLevelCount, 1);

            if (currentLevel == openedLevelCount)
            {
                openedLevelCount++;
                PlayerPrefs.SetInt(PlayerPrefKey.OpenedLevelCount, openedLevelCount);
            }
        }
        else
        {
            //TODO anahtar isteme gelsin
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(Tag.Gate.ToString()) && !isGetKey)
        {
            //TODO anahtar isteme gitsin
        }
    }
}