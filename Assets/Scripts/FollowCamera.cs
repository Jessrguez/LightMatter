using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Referencia al transform del jugador a seguir
    [SerializeField] private Transform player;
    
    // Distancia relativa entre la cámara y el jugador
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -1f);
    
    // Suavizado del movimiento (0 = sin suavizado, 20 = máximo suavizado)
    [SerializeField, Range(0, 20f)] private float smoothSpeed = 5f;
    
    // Si la cámara debe rotar para mirar siempre al jugador
    [SerializeField] private bool lookAtPlayer = true;

    // Se ejecuta después de Update() para movimiento más suave
    private void LateUpdate()
    {
        // Salir si no hay jugador asignado
        if (player == null) return;

        // Calcular posición deseada (jugador + offset)
        Vector3 desiredPosition = player.position + offset;
        
        // Interpolar suavemente hacia la posición deseada
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position, 
            desiredPosition, 
            smoothSpeed * Time.deltaTime
        );
        
        // Aplicar la nueva posición
        transform.position = smoothedPosition;

        // Rotar para mirar al jugador si está activado
        if (lookAtPlayer) 
        {
            transform.LookAt(player);
        }
    }
}