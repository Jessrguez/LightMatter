using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -7f);
    [SerializeField, Range(0, 20f)] private float smoothSpeed = 5f;
    [SerializeField] private bool lookAtPlayer = true;

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        if (lookAtPlayer) transform.LookAt(player);
    }
}
