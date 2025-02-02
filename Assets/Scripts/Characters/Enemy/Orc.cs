using System.Collections;
using UnityEngine;

public class Orc : Enemy
{
    private Weapon weapon;
    private bool isAttacking;

    private void Start()
    {
        EnemyStart();
        weapon = GetComponentInChildren<Weapon>();
    }

    private void Update()
    {
        if (CheckCharacterState()) return;
        EnemyUpdate();
        Attack();
    }

    protected override void Attack()
    {
        if (!isAttacking && IsPlayerInSight())
        {
            StartCoroutine(AxeAttack());
            isAttacking = true;
        }
    }

    protected override void Roll()
    {

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
        if (isAttacking)
        {
            if (IsOnEdge())
            {
                rb.linearVelocity = Vector2.zero;
                isAttacking = false;
            }
            return true;
        }
        return false;
    }

    IEnumerator AxeAttack()
    {
        float distance = Mathf.Abs(transform.position.x - Player.Instance.transform.position.x);

        while (IsPlayerInSight())
        {
            distance = Mathf.Abs(transform.position.x - Player.Instance.transform.position.x);

            if (distance >= 2)
            {
                rb.linearVelocity = new Vector2(transform.localScale.x * stat.moveSpeed * 2.5f, 0f);

                if (IsOnEdge())
                {
                    rb.linearVelocity = Vector2.zero;
                    isAttacking = false;
                    yield break;
                }
            }
            else
            {
                weapon.Attack();
                yield return new WaitUntil(() => !weapon.GetAttacking());
            }

            yield return null;
        }
        isAttacking = false;
    }
}