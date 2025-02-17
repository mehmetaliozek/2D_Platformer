using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Character
{
    public static Player Instance { get; private set; }

    public Dive dive;
    private Weapon weapon;
    private bool isInGate;
    private bool isGetKey;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;
    private bool moveWithAcceleration;

    [SerializeField] private ParticleSystem dust;
    [SerializeField] private Bar coolDownBar;
    [SerializeField] private GameObject infoPanelGate;

    private int amountOfJumps = 1;
    private int amountOfJumpsLeft;
    public float wallSlideSpeed;
    public float airDragMultiplier = 0.9f;
    private Vector2 wallJumpDirection = new Vector2(-1, 1);

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        animator.SetTrigger(AnimationParametre.Spawn.ToString());
        weapon = GetComponentInChildren<Weapon>();

        healtBar.SetMaxValue(stat.health);
        coolDownBar.SetMaxValue(1);
        coolDownBar.animationDuration = roll.rollingCooldown * 2;

        amountOfJumpsLeft = amountOfJumps;

        HitEvent += (_, _) =>
        {
            Camera.main.GetComponent<CameraController>().TriggerShake(0.1f);
        };
    }

    private void Update()
    {
        if (CheckCharacterState()) return;

        float moveInput = Input.GetAxis("Horizontal");
        UpdateChecks(moveInput);

        Move(moveInput);
        Turn(moveInput);
        Jump(moveInput);
        Roll();
        Attack();
        Dive();
    }


    private void UpdateChecks(float moveInput)
    {
        isGrounded = Physics2D.OverlapCircle(ground.position, groundCheckRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapPoint(transform.position + Vector3.right * moveInput * 0.5f, groundLayer);
        isWallSliding = isTouchingWall && !isGrounded && !Mathf.Approximately(moveInput, 0) && rb.linearVelocity.y < 0;

        if (isTouchingWall)
        {
            moveWithAcceleration = true;
        }
        else if (isGrounded)
        {
            moveWithAcceleration = false;
        }
    }

    protected override void Move(float moveInput)
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(stat.moveSpeed * moveInput, rb.linearVelocity.y);
        }
        else if (!isWallSliding && !Mathf.Approximately(moveInput, 0))
        {
            rb.AddForce(new Vector2(stat.moveSpeed * moveInput, 0));
            if (Mathf.Abs(rb.linearVelocity.x) > stat.moveSpeed)
            {
                rb.linearVelocity = new Vector2(stat.moveSpeed * Mathf.Sign(rb.linearVelocity.x), rb.linearVelocity.y);
            }
        }
        else if (!isWallSliding)
        {
            if (moveWithAcceleration)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * airDragMultiplier, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(stat.moveSpeed * moveInput, rb.linearVelocity.y);
            }
        }

        if (isWallSliding && rb.linearVelocity.y < -wallSlideSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
        animator.SetFloat(AnimationParametre.Velocity.ToString(), Mathf.Abs(moveInput));
    }

    protected override void Turn(float moveInput)
    {
        if (Mathf.Approximately(moveInput, 0)) return;

        float newScaleX = Mathf.Sign(moveInput);
        if (!Mathf.Approximately(transform.localScale.x, newScaleX))
        {
            transform.localScale = new Vector3(newScaleX, 1, 1);
            if (isGrounded && dust != null) dust.Play();
        }
    }

    protected override void Jump(float moveInput)
    {
        if ((isGrounded && rb.linearVelocity.y <= 0) || isWallSliding)
            amountOfJumpsLeft = amountOfJumps;

        canJump = amountOfJumpsLeft > 0;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump && !isWallSliding)
            {
                dust.Play();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, stat.jumpForce);
                amountOfJumpsLeft--;
            }
            else if (isWallSliding && Mathf.Approximately(moveInput, 0) && canJump)
            {
                dust.Play();
                isWallSliding = false;
                amountOfJumpsLeft--;
                Vector2 force = new Vector2(stat.jumpForce * wallJumpDirection.x * -Mathf.Sign(transform.localScale.x),
                                            stat.jumpForce * wallJumpDirection.y);
                rb.AddForce(force, ForceMode2D.Impulse);
            }
            else if ((isWallSliding || isTouchingWall) && !Mathf.Approximately(moveInput, 0) && canJump)
            {
                dust.Play();
                isWallSliding = false;
                amountOfJumpsLeft--;
                Vector2 force = new Vector2(stat.jumpForce * wallJumpDirection.x * moveInput,
                                            stat.jumpForce * wallJumpDirection.y);
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }

        animator.SetBool(AnimationParametre.IsJumpIdle.ToString(), !isGrounded && Mathf.Approximately(moveInput, 0));
        animator.SetBool(AnimationParametre.IsJumpRun.ToString(), !isGrounded && !Mathf.Approximately(moveInput, 0));
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
            RollStart(callback: () => SetRollState(RollState.End));
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
        if (Input.GetKey(KeyCode.Mouse0))
            weapon.Attack();
    }

    private void Dive()
    {
        if (Input.GetKeyDown(KeyCode.S) && !isGrounded)
            StartCoroutine(dive.DiveCoroutine(rb, tr, this));
    }

    protected override bool CheckCharacterState()
    {
        if (isInGate)
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            Vector3 halfScale = transform.localScale / 2f;
            if (viewportPos.y - halfScale.y > 1)
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

    public void SetKey() => isGetKey = true;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (enemy.roll.isRolling)
            {
                Knockback(enemy);
                enemy.RollStop();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(Tag.Gate.ToString()) && isGetKey)
        {
            if (roll.isRolling)
                RollStop();

            isInGate = true;
            Camera.main.GetComponent<CameraController>().LevelEnding();
            rb.linearVelocity = Vector2.zero;
            rb.AddForceY(50f, ForceMode2D.Impulse);

            int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Split(' ')[1]);
            int openedLevelCount = PlayerPrefs.GetInt(PlayerPrefKey.OpenedLevelCount, 1);
            if (currentLevel == openedLevelCount)
            {
                openedLevelCount++;
                PlayerPrefs.SetInt(PlayerPrefKey.OpenedLevelCount, openedLevelCount);
            }
        }
        else
        {
            // TODO: Anahtar isteme ekraný veya bilgi paneli gösterilebilir.
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(Tag.Gate.ToString()) && !isGetKey)
        {
            // TODO: Anahtar isteme ekraný tetiklenebilir.
        }
    }
}