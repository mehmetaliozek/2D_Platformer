using System.Collections;
using UnityEngine;

[System.Serializable]
public class Dive
{
    //public Character character;
    [HideInInspector]
    public bool isDiving;

    public IEnumerator DiveCoroutine(Rigidbody2D rb, TrailRenderer tr, ParticleSystem dust,Character character)
    {
        rb.linearVelocity = new Vector2(0, -2f * character.stat.jumpForce);
        tr.emitting = true;
        isDiving = true;
        while (!Physics2D.OverlapCircle(character.groundCheck.position, character.groundCheckRadius, character.groundLayer))
        {
            yield return null;
        }
        tr.emitting = false;
        isDiving = false;
        dust.Play();
    }
}