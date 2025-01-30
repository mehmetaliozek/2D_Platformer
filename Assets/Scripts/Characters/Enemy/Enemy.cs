using UnityEngine;

public abstract class Enemy : Character
{
    public EnemyType type;
    public float range;
    private Vector2 positionA;
    private Vector2 positionB;
    private Vector2 targetPosition;
    private Vector3 currentAbsScale;
    private bool isRemoved = false;

    public LayerMask playerLayer;
    public float visionRange = 2f;
    public ParticleSystem damageEffect;

    public void Start()
    {
        positionA = transform.position;
        positionB = transform.position + new Vector3(range * Mathf.Sign(transform.localScale.x), 0, 0);
        targetPosition = positionB;
        currentAbsScale = new Vector3(
            Mathf.Abs(transform.localScale.x),
            Mathf.Abs(transform.localScale.y),
            Mathf.Abs(transform.localScale.z)
        );

        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        healtBar.SetMaxValue(stat.health);

        EnemyStart();
    }

    public void Update()
    {
        if (CheckCharacterState()) return;
        Move(0);
        Turn(1);
        Jump(0);
        EnemyUpdate();
    }

    protected override void Move(float moveInput)
    {
        transform.position = new Vector2(
            Mathf.MoveTowards(transform.position.x, targetPosition.x, stat.moveSpeed * Time.deltaTime),
            transform.position.y
        );

        if (Mathf.Approximately(transform.position.x, targetPosition.x))
        {
            targetPosition = (targetPosition == positionA) ? positionB : positionA;
        }

        TargetRecalculation();

        animator.SetFloat(AnimationParametre.Velocity.ToString(), 1);
    }

    protected override void Turn(float moveInput)
    {
        if (moveInput == 0) return;

        float direction = (transform.position.x < targetPosition.x) ? 1f : -1f;
        transform.localScale = new Vector3(currentAbsScale.x * direction, currentAbsScale.y, currentAbsScale.z);
    }

    protected override void Jump(float moveInput)
    {
        Vector3 direction = GetDirection();
        Vector3 basePosition = transform.position + Vector3.down * (currentAbsScale.y / 3);
        Vector3 endPosition = basePosition + direction * (currentAbsScale.x / 1.5f);

        isGrounded = Physics2D.OverlapCircle(ground.position, groundCheckRadius, groundLayer);
        RaycastHit2D hit = Physics2D.Linecast(basePosition, endPosition, groundLayer);

        if (hit.collider != null && hit.collider.CompareTag(Tag.Ground.ToString()) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stat.jumpForce);
        }

        animator.SetBool(AnimationParametre.IsJumpRun.ToString(), !isGrounded);
    }

    protected override bool CheckCharacterState()
    {
        if (isDeath)
        {
            if (!isRemoved)
            {
                RemoveThis();
                isRemoved = true;
            }
            return true;
        }
        if (roll.isRolling) return true;
        if (knockback.isKnockback) return true;
        return false;
    }

    private void TargetRecalculation()
    {
        Vector3 direction = GetDirection();
        Vector2 checkPosition = ground.position + Vector3.down + direction / 3;

        if (!Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayer) && isGrounded)
        {
            targetPosition = transform.position - direction * range;
        }
    }

    protected bool IsPlayerInSight()
    {
        Vector3 direction = GetDirection();
        float height = transform.localScale.y * 0.9f;

        return Physics2D.OverlapBox(
            transform.position + direction * (visionRange * 0.5f),
            new Vector2(visionRange, height),
            0,
            playerLayer
        );
    }

    public void RemoveThis()
    {
        GameManager.Instance.RemoveEnemy(this, positionA);
    }

    private Vector3 GetDirection()
    {
        return transform.localScale.x > 0 ? Vector3.right : Vector3.left;
    }

    protected abstract void EnemyStart();
    protected abstract void EnemyUpdate();
}