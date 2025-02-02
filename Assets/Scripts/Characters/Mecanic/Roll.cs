using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Roll
{
    private Coroutine coroutine;
    public Coroutine Coroutine
    {
        get { return coroutine; }
        set { coroutine = value; }
    }

    private Action callback;

    [HideInInspector]
    public bool canRoll = true;
    [HideInInspector]
    public bool isRolling;

    [SerializeField]
    private float rollingPower = 6f;
    public float rollingTime = 0.4f;
    public float rollingCooldown = 1f;

    public IEnumerator RollStartCoroutine(Rigidbody2D rb, Transform tf, TrailRenderer tr, float duration, Action callback)
    {
        canRoll = false;
        isRolling = true;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        this.callback = callback;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = new Vector2(tf.localScale.x * rollingPower, 0f);
        tr.emitting = true;
        float elapsed = 0f;
        while (elapsed <= rollingTime)
        {
            rb.linearVelocity = new Vector2(tf.localScale.x * rollingPower, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        tr.emitting = false;
        isRolling = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        callback?.Invoke();
        yield return new WaitForSeconds(rollingCooldown);
        canRoll = true;
    }

    public IEnumerator RollStopCoroutine(Rigidbody2D rb, TrailRenderer tr)
    {
        tr.emitting = false;
        isRolling = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        callback?.Invoke();
        yield return new WaitForSeconds(rollingCooldown);
        canRoll = true;
    }
}