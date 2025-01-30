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

        healtBar.SetMaxValue(stat.health);
        coolDownBar.SetMaxValue(1);
        coolDownBar.animationDuration = roll.rollingCooldown * 2;
    }

    private void Update()
    {
        if (CheckCharacterState()) return;
        float moveInput = Input.GetAxis("Horizontal");
        Move(moveInput);
        Turn(moveInput);
        Jump(moveInput);
        Roll();
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
        if (moveInput == 0) return;

        float newScaleX = Mathf.Sign(moveInput);
        if (transform.localScale.x != newScaleX)
        {
            transform.localScale = new Vector3(newScaleX, 1, 1);
            if (isGrounded) dust.Play();
        }
    }

    protected override void Jump(float moveInput)
    {
        isGrounded = Physics2D.OverlapCircle(ground.position, groundCheckRadius, groundLayer);

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            dust.Play();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stat.jumpForce);
        }

        animator.SetBool(AnimationParametre.IsJumpIdle.ToString(), !isGrounded && moveInput == 0);
        animator.SetBool(AnimationParametre.IsJumpRun.ToString(), !isGrounded && moveInput != 0);
    }

    protected override void Roll()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && roll.canRoll)
        {
            coolDownBar.SetValue(0);
            SetRollState(RollState.Start);
            if (!isGrounded)
            {
                animator.SetBool(AnimationParametre.IsJumpIdle.ToString(), false);
                animator.SetBool(AnimationParametre.IsJumpRun.ToString(), false);
            }
            StartCoroutine(roll.RollCoroutine(rb, transform, tr, callback: () => SetRollState(RollState.End)));
            StartCoroutine(SetCoolDownValueCoroutine());
        }
    }

    private IEnumerator SetCoolDownValueCoroutine()
    {
        yield return new WaitForSeconds(roll.rollingCooldown + roll.rollingTime);
        coolDownBar.SetValue(1);
    }

    protected override void Attack()
    {
        sword.Attack();
    }

    protected override bool CheckCharacterState()
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
            return true;
        }
        if (isDeath) return true;
        if (roll.isRolling) return true;
        if (knockback.isKnockback) return true;
        if (dive.isDiving) return true;
        return false;
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
            if (enemy.roll.isRolling)
            {
                Hit(enemy.stat.damage);
                Vector2 direction = (transform.position - other.transform.position).normalized;
                StartCoroutine(knockback.KnockbackCoroutine(rb, direction, enemy.knockback));
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