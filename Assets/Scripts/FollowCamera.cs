using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // el jugador
    public Vector3 offset = new Vector3(0, 3, -6);
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target.position + Vector3.up * 1.5f); // mirar hacia el jugador, un poco arriba
    }
}
