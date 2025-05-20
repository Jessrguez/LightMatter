// ObstacleBase.cs
using UnityEngine;

/// <summary>
/// Clase base abstracta para todos los obstáculos del juego
/// Implementa lógica común de interacción con el jugador
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class ObstacleBase : MonoBehaviour
{
    // Estado del obstáculo
    protected bool passed = false;  // Indica si el jugador ya interactuó con este obstáculo

    // Configuración de efectos
    [SerializeField] protected AudioSource successSound;  // Sonido al pasar correctamente
    [SerializeField] protected AudioSource failSound;     // Sonido al fallar
    [SerializeField] protected int scoreAmount = 10;      // Puntos por superar el obstáculo

    // Componentes
    protected Collider obstacleCollider;  // Referencia al collider del obstáculo

    /// <summary>
    /// Inicialización del obstáculo
    /// </summary>
    protected virtual void Awake()
    {
        obstacleCollider = GetComponent<Collider>();
        if (obstacleCollider == null)
        {
            Debug.LogError($"{nameof(ObstacleBase)}: No Collider found.");
        }
    }

    /// <summary>
    /// Método abstracto que determina si el modo actual del jugador es correcto para este obstáculo
    /// </summary>
    protected abstract bool IsCorrectMode(PlayerController.LightMode currentMode);

    /// <summary>
    /// Lógica de interacción cuando el jugador entra en contacto con el obstáculo
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (passed) return;  // Evitar múltiples interacciones

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;  // Solo interactuar con el jugador

        passed = true;  // Marcar como interactuado

        // Verificar modo del jugador y aplicar consecuencia
        if (IsCorrectMode(player.CurrentMode))
        {
            GameManager.Instance?.AddScore(scoreAmount);
            successSound?.Play();
        }
        else
        {
            GameManager.Instance?.LoseLife();
            failSound?.Play();
        }

        // Desactivar collider para evitar más interacciones
        if (obstacleCollider != null)
            obstacleCollider.enabled = false;
    }

    /// <summary>
    /// Actualización por frame - Maneja la desactivación del obstáculo cuando queda atrás
    /// </summary>
    protected virtual void Update()
    {
        if (PlayerController.Instance == null) return;

        // Desactivar cuando está suficientemente detrás del jugador
        if (transform.position.z < PlayerController.Instance.transform.position.z - 10f)
        {
            gameObject.SetActive(false);
            ObstacleSpawner.Instance?.ReturnObstacleToPool(gameObject);
            
            // Contar como evitado si no se interactuó
            if (!passed)
                GameManager.Instance?.IncrementObstaclesAvoidedCount();
        }
    }

    /// <summary>
    /// Reinicia el estado del obstáculo para reutilización
    /// </summary>
    public virtual void ResetObstacle()
    {
        passed = false;
        if (obstacleCollider != null)
        {
            obstacleCollider.enabled = true;
        }
    }
}