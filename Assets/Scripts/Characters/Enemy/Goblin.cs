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

    protected override void Attack()
    {
        Roll();
    }

    protected override void Roll()
    {
        if (roll.canRoll && IsPlayerInSight())
        {
            animator.SetFloat(AnimationParametre.Velocity.ToString(), 0);
            StartCoroutine(roll.RollCoroutine(rb, transform, tr, 0.25f));
        }
    }
}