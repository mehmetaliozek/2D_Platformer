using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    public float smoothSpeed = 2.5f;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag(Tag.Player).transform;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + Vector3.back;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
