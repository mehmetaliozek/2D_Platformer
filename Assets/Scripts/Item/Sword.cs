using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private LayerMask enemyLayers;

    private bool isAttacking = false;

    public void Attack()
    {
        if (Input.GetKey(KeyCode.Mouse0) && !isAttacking)
        {
            animator.SetTrigger(AnimationParametre.Attack.ToString());
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, 0.5f, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                Character character = enemy.GetComponent<Character>();
                character.Hit(GetComponentInParent<Character>().stat.damage);
                (character as Enemy).damageEffect.Play();
                if (character.knockback.isKnockback) continue;
                Vector2 direction = -(transform.position - character.transform.position).normalized;
                StartCoroutine(character.knockback.KnockbackCoroutine(character.GetComponent<Rigidbody2D>(), direction));
            }
        }
    }

    public void AttackStart() => isAttacking = true;

    public void AttackEnd() => isAttacking = false;
}