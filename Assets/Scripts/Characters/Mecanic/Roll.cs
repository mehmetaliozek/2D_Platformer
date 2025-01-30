using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class Roll
{
    [HideInInspector]
    public bool canRoll = true;
    [HideInInspector]
    public bool isRolling;

    [SerializeField]
    private float rollingPower = 6f;
    public float rollingTime = 0.4f;
    public float rollingCooldown = 1f;

    public IEnumerator RollCoroutine(Rigidbody2D rb, Transform tf, TrailRenderer tr, float duration = 0f, Action callback = null)
    {
        canRoll = false;
        isRolling = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = new Vector2(tf.localScale.x * rollingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(rollingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isRolling = false;
        if (callback is not null)
        {
            callback();
        }
        yield return new WaitForSeconds(rollingCooldown);
        canRoll = true;
    }
}