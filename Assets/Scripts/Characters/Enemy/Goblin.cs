using UnityEngine;

public class Goblin : Enemy
{
    protected override void EnemyStart()
    {

    }

    protected override void EnemyUpdate()
    {
        Attack();
    }

    protected override void Dash()
    {
        if (IsPlayerInSight() && dash.canDash)
        {
            animator.SetFloat(AnimationParametre.Velocity.ToString(), 0);
            StartCoroutine(dash.DashCoroutine(rb, transform, tr, 0.25f));
        }
    }

    bool IsPlayerInSight()
    {
        Vector2 visionRange = transform.position + ((transform.localScale.x == 1 ? Vector3.right : Vector3.left) * 2f);
        RaycastHit2D hit = Physics2D.Linecast(transform.position, visionRange, playerLayer);
        return hit.collider != null && hit.collider.gameObject.CompareTag(Tag.Player.ToString());
    }

    protected override void Attack()
    {
        Dash();
    }
}