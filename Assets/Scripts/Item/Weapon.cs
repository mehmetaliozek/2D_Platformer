using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private Character parent;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private float attackRadius;

    [SerializeField]
    private LayerMask targetLayer;

    private bool isAttacking = false;

    public void Attack()
    {
        if (!isAttacking)
        {
            animator.SetTrigger(AnimationParametre.Attack.ToString());
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, targetLayer);
            foreach (Collider2D hit in hits)
            {
                Character character = hit.GetComponent<Character>();
                if (character.knockback.isKnockback) continue;
                character.Knockback(parent);
            }
        }
    }

    public void AttackStart() => isAttacking = true;

    public void AttackEnd() => isAttacking = false;

    public bool GetAttacking() => isAttacking;
}