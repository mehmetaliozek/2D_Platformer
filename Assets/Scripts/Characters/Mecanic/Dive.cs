using System.Collections;
using UnityEngine;

[System.Serializable]
public class Dive
{
    [HideInInspector]
    public bool isDiving;

    [SerializeField]
    private ParticleSystem particle;

    [SerializeField]
    private LayerMask targetLayer;
    [SerializeField]
    public Vector2 size;

    public IEnumerator DiveCoroutine(Rigidbody2D rb, TrailRenderer tr, Character character)
    {
        rb.linearVelocity = new Vector2(0, -2f * character.stat.jumpForce);
        tr.emitting = true;
        isDiving = true;
        while (!Physics2D.OverlapCircle(character.ground.position, character.groundCheckRadius, character.groundLayer))
        {
            yield return null;
        }
        tr.emitting = false;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(character.ground.position, size, 0, targetLayer);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out Character c))
            {
                c.Knockback(character);
            }
        }
        particle.Play();
        isDiving = false;
    }
}