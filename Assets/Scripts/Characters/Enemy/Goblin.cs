public class Goblin : Enemy
{
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
        Roll();
    }

    protected override void Roll()
    {
        if (roll.canRoll && IsPlayerInSight())
        {
            animator.SetFloat(AnimationParametre.Velocity.ToString(), 0);
            RollStart(0.25f);
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
        if (roll.isRolling) return true;
        if (knockback.isKnockback) return true;
        return false;
    }
}