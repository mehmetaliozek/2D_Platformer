using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    public float smoothSpeed = 2.5f;

    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.3f;
    public float dampingSpeed = 1.0f;

    private Vector3 initialPosition;
    private float currentShakeDuration = 0f;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag(Tag.Player).transform;
        initialPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + Vector3.back;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        if (currentShakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            currentShakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            currentShakeDuration = 0f;
            initialPosition = transform.position;
        }
    }

    public void TriggerShake(float duration = 0.5f)
    {
        currentShakeDuration = duration;
    }
}
