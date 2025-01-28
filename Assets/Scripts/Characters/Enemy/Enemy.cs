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

    public ParticleSystem damageEffect;

    public void Start()
    {
        positionA = transform.position;
        positionB = transform.position + (transform.localScale * range);
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
        if (UpdateBreaker())return;
        Move(0);
        Turn(1);
        Jump(0);
        EnemyUpdate();
    }

    protected override void Move(float moveInput)
    {
        Vector2 newPos = new Vector2(
            Mathf.MoveTowards(transform.position.x, targetPosition.x, stat.moveSpeed * Time.deltaTime),
            transform.position.y
        );

        transform.position = newPos;

        if (Mathf.Abs(transform.position.x - targetPosition.x) < 0.1f)
        {
            targetPosition = targetPosition == positionA ? positionB : positionA;
        }

        animator.SetFloat(AnimationParametre.Velocity.ToString(), 1);
    }

    protected override void Turn(float moveInput)
    {
        if (moveInput != 0)
        {
            Vector3 directionVector = new Vector3(transform.position.x < targetPosition.x ? 1 : -1, 1, 1);
            transform.localScale = Vector3.Scale(currentAbsScale, directionVector);
        }
    }

    protected override void Jump(float moveInput)
    {
        Vector2 visionRange = transform.position + (transform.localScale.x == 1 ? Vector3.right : Vector3.left) / 1.5f;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, visionRange, groundLayer);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (hit.collider != null && hit.collider.gameObject.CompareTag(Tag.Ground.ToString()) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stat.jumpForce);
        }

        animator.SetBool(AnimationParametre.IsJumpRun.ToString(), !isGrounded);
    }

    public void RemoveThis()
    {
        GameObject.FindGameObjectWithTag(Tag.GameManager.ToString()).GetComponent<GameManager>().RemoveEnemy(this, positionA);
    }

    public bool UpdateBreaker()
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
        if (dash.isDashing) return true;
        if (knockback.isKnockback) return true;
        return false;
    }

    protected abstract void EnemyStart();
    protected abstract void EnemyUpdate();
}
