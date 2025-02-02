using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class Knockback
{
    [HideInInspector]
    public bool isKnockback;

    [SerializeField]
    private float knockbackingPower = 10f;
    [SerializeField]
    private float knockbackingTime = 0.2f;
    [SerializeField]
    [Range(0f, 1f)]
    private float knockbackingResistance = 0f;

    public IEnumerator KnockbackCoroutine(Rigidbody2D rb, Vector3 direction, Knockback kb)
    {
        isKnockback = true;
        Vector3 force = direction.normalized * (kb.knockbackingPower - (kb.knockbackingPower * knockbackingResistance));
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(kb.knockbackingTime);
        isKnockback = false;
    }
}