using System.Collections;
using UnityEngine;

[System.Serializable]
public class Knockback
{
    public bool isKnockback;
    public IEnumerator KnockbackCoroutine(Rigidbody2D rb, Vector3 direction)
    {
        isKnockback = true;
        Vector3 force = direction.normalized * 10;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);
        isKnockback = false;
    }
}