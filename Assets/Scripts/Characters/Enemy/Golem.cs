using UnityEngine;

public class Golem : Enemy
{
    protected override void EnemyStart()
    {

    }

    protected override void EnemyUpdate()
    {

    }

    protected override void Attack()
    {

    }

    protected override void Roll()
    {
        //if (roll.canRoll && IsPlayerInSight())
        //{
        //    SetRollState(RollState.Start);
        //    if (!isGrounded)
        //    {
        //        animator.SetBool(AnimationParametre.IsJumpIdle.ToString(), false);
        //        animator.SetBool(AnimationParametre.IsJumpRun.ToString(), false);
        //    }
        //    StartCoroutine(roll.RollCoroutine(rb, transform, tr, 0.25f, callback: () => SetRollState(RollState.End)));
        //}
    }
}
// Oyunucuyu görünce yuvarlanarak ona doðru gitsin hýzlýca gidince yere vursun ittirsin
// yuvarlanma az vursun daha geri savursun
// yere vurma çok vursun az savursun
