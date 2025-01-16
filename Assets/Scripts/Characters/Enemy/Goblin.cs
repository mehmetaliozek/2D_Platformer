using UnityEngine;

public class Goblin : Character
{
    public float range;
    private Vector2 positionA;
    private Vector2 positionB;
    private Vector2 targetPosition;
    private bool isRemoved = false;

    public LayerMask playerLayer;

    public ParticleSystem damageEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();

        positionA = transform.position;
        positionB = transform.position + (transform.localScale * range);
        targetPosition = positionB;

        healtBar.SetMaxValue(stat.health);
    }

    private void Update()
    {
        if (isDeath)
        {
            if (!isRemoved)
            {
                RemoveThis();
                isRemoved = true;
            }
            return;
        }
        if (dash.isDashing) return;
        if (knockback.isKnockback) return;

        Move(0);
        Turn(1);
        Jump(0);
        Dash();
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

        animator.SetFloat(AnimationParametre.Velocity, 1);
    }

    protected override void Turn(float moveInput)
    {
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(transform.position.x < targetPosition.x ? 1 : -1, 1, 1);
        }
    }

    protected override void Jump(float moveInput)
    {
        Vector2 visionRange = transform.position + (transform.localScale.x == 1 ? Vector3.right : Vector3.left) / 1.5f;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, visionRange, groundLayer);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (hit.collider != null && hit.collider.gameObject.CompareTag(Tag.Ground) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stat.jumpForce);
        }

        animator.SetBool(AnimationParametre.IsJumpRun.ToString(), !isGrounded);
    }

    protected override void Dash()
    {
        if (IsPlayerInSight() && dash.canDash)
        {
            animator.SetFloat(AnimationParametre.Velocity, 0);
            StartCoroutine(dash.DashCoroutine(rb, transform, tr, 0.25f));
        }
    }

    bool IsPlayerInSight()
    {
        Vector2 visionRange = transform.position + ((transform.localScale.x == 1 ? Vector3.right : Vector3.left) * 2f);
        RaycastHit2D hit = Physics2D.Linecast(transform.position, visionRange, playerLayer);
        return hit.collider != null && hit.collider.gameObject.CompareTag(Tag.Player);
    }

    protected override void Attack()
    {

    }

    public void RemoveThis()
    {
        GameObject.FindGameObjectWithTag(Tag.GameManager).GetComponent<GameManager>().RemoveEnemy(this, positionA);
    }
}