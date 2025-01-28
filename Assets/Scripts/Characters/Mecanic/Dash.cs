using System.Collections;
using UnityEngine;

[System.Serializable]
public class Dash
{
    [HideInInspector]
    public bool canDash = true;
    [HideInInspector]
    public bool isDashing;

    [SerializeField]
    private float dashingPower = 6f;
    public float dashingTime = 0.4f;
    public float dashingCooldown = 1f;

    public IEnumerator DashCoroutine(Rigidbody2D rb, Transform tf, TrailRenderer tr, float duration = 0f)
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = new Vector2(tf.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}