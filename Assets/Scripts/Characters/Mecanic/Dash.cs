using System.Collections;
using UnityEngine;

[System.Serializable]
public class Dash
{
    public bool canDash = true;
    [HideInInspector]
    public bool isDashing;

    [HideInInspector]
    public bool isWaiting;

    [SerializeField]
    private float dashingPower = 6f;
    [SerializeField]
    private float dashingTime = 0.4f;
    [SerializeField]
    private float dashingCooldown = 1f;

    public IEnumerator DashCoroutine(Rigidbody2D rb, Transform tf, TrailRenderer tr, float waiting = 0f)
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(waiting);
        rb.velocity = new Vector2(tf.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}