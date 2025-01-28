using System.Collections;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public Stat stat;
    public Dash dash;
    public Knockback knockback;

    [SerializeField]
    protected Bar healtBar;

    [SerializeField]
    protected Animator animator;
    protected Rigidbody2D rb;
    protected TrailRenderer tr;

    protected bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    protected bool isDeath = false;

    protected abstract void Move(float moveInput);

    protected abstract void Turn(float moveInput);

    protected abstract void Jump(float moveInput);

    protected abstract void Dash();

    protected abstract void Attack();

    public void Hit(float damage)
    {
        if (isDeath) return;
        StartCoroutine(HitCoroutine());

        stat.health -= damage;
        healtBar.SetValue(stat.health);

        if (stat.health <= 0)
        {
            Death();
        }
    }

    private IEnumerator HitCoroutine()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();

        yield return new WaitForSeconds(0.15f);
        sr.color = Color.red;

        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }

    protected void Death()
    {
        animator.SetTrigger(AnimationParametre.Death.ToString());
    }


    public void DeathStart()
    {
        isDeath = true;
    }

    public void DeathEnd()
    {
        animator.speed = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stat.health <= 0 && collision.gameObject.CompareTag(Tag.Ground.ToString()))
        {
            CapsuleCollider2D capsuleCollider = gameObject.GetComponent<CapsuleCollider2D>();
            float colliderThresholdY = transform.position.y - (capsuleCollider.size.y / 4);
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.point.y < colliderThresholdY)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                    capsuleCollider.isTrigger = true;
                    break;
                }
            }
        }
    }
}