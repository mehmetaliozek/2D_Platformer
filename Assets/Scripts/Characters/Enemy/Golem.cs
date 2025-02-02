using System.Collections;
using UnityEngine;

public class Golem : Enemy
{
    public Dive dive;

    private bool isAttacking;

    private void Start()
    {
        EnemyStart();
    }

    private void Update()
    {
        if (CheckCharacterState()) return;
        EnemyUpdate();
        Attack();
    }

    protected override void Attack()
    {
        if (isGrounded && IsPlayerInSight())
        {
            float distance = Mathf.Abs(transform.position.x - Player.Instance.transform.position.x);
            if (!isAttacking && distance <= 2)
            {
                isAttacking = true;
                StartCoroutine(DiveAttack());
            }
            else if (distance > 2)
            {
                Roll();
            }
        }
    }

    protected override void Roll()
    {
        if (roll.canRoll)
        {
            SetRollState(RollState.Start);
            if (!isGrounded)
            {
                animator.SetBool(AnimationParametre.IsJumpIdle.ToString(), false);
                animator.SetBool(AnimationParametre.IsJumpRun.ToString(), false);
            }
            RollStart(0.25f, callback: () => SetRollState(RollState.End));
        }
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
        if (roll.isRolling)
        {
            if (IsOnEdge())
            {
                RollStop();
                rb.linearVelocity = Vector2.zero;
            }
            return true;
        }
        if (knockback.isKnockback) return true;
        if (dive.isDiving) return true;
        return false;
    }

    IEnumerator DiveAttack()
    {
        if (dive.isDiving) yield break;

        float elapsed = 0;
        while (elapsed <= 0.2f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stat.jumpForce);
            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        StartCoroutine(dive.DiveCoroutine(rb, tr, this));
        yield return new WaitUntil(() => !dive.isDiving);
        isAttacking = false;
    }
}
