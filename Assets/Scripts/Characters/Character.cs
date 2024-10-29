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
        GetComponent<CapsuleCollider2D>().size = new Vector2(0.5f, 0.5f);
        GetComponent<CapsuleCollider2D>().isTrigger = true;
    }

    public void StartDeath()
    {
        isDeath = true;
    }

    public void EndDeath()
    {
        animator.speed = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (stat.health <= 0 && other.CompareTag(Tag.Ground))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}