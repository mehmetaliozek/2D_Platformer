using UnityEngine;

public class Goblin : Enemy
{
    private new void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        healtBar.SetMaxValue(stat.health);
    }

    private new void Update()
    {
        base.Update();
        Attack();
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
        Dash();
    }
}