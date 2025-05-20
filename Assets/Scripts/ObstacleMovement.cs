// ObstacleMovement.cs
using UnityEngine;

/// <summary>
/// Controla el movimiento y comportamiento de destrucción de los obstáculos
/// </summary>
public class ObstacleMovement : MonoBehaviour
{
    // Configuración de movimiento
    [SerializeField] public float speed;  // Velocidad de desplazamiento del obstáculo

    [Header("Configuración de Destrucción")]
    [SerializeField] private bool showDebugGizmo = true;  // Mostrar ayuda visual en el editor

    [Header("Límites de Pantalla")]
    [SerializeField] private float outOfBoundsX = -10f;  // Posición X para considerar fuera de pantalla
    [SerializeField] private float persistTimeOffScreen = 3f;  // Tiempo antes de destruir al salir de pantalla

    // Variables de estado
    private float offScreenTimer = 0f;    // Temporizador para destrucción
    private bool isOffScreen = false;     // Flag de estado fuera de pantalla
    private Vector3 initialScale;         // Escala inicial para resetear al reciclar

    private void Awake()
    {
        // Guardar escala inicial para resetear al reciclar
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        // Movimiento constante hacia atrás (eje Z negativo)
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        // Detección de salida de pantalla
        if (transform.position.x < outOfBoundsX && !isOffScreen)
        {
            isOffScreen = true;
            offScreenTimer = 0f;
        }

        // Temporizador de destrucción cuando está fuera de pantalla
        if (isOffScreen)
        {
            offScreenTimer += Time.deltaTime;
            if (offScreenTimer >= persistTimeOffScreen)
            {
                ReturnToPool();
            }
        }
    }

    /// <summary>
    /// Maneja la colisión con el jugador (llamado desde ObstacleBase)
    /// </summary>
    public void HandlePlayerCollision()
    {
        ReturnToPool();
    }

    /// <summary>
    /// Devuelve el obstáculo al pool o lo destruye si no hay pool disponible
    /// </summary>
    private void ReturnToPool()
    {
        if (ObstacleSpawner.Instance != null)
        {
            // Resetear estado
            isOffScreen = false;
            offScreenTimer = 0f;
            transform.localScale = initialScale;

            gameObject.SetActive(false);
            ObstacleSpawner.Instance.ReturnObstacleToPool(gameObject);
        }
        else
        {
            // Fallback si no hay sistema de pooling
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Dibuja gizmos en el editor para visualizar el límite de destrucción
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showDebugGizmo) return;

        Gizmos.color = Color.red;
        // Línea hasta el límite de destrucción
        Gizmos.DrawLine(transform.position, new Vector3(outOfBoundsX, transform.position.y, transform.position.z));
        // Marcador en la posición de destrucción
        Gizmos.DrawWireCube(new Vector3(outOfBoundsX, transform.position.y, transform.position.z), new Vector3(1f, 2f, 1f));
    }
}