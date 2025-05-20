using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el comportamiento del jugador, incluyendo movimiento, cambio de modo de luz y actualizaciones visuales.
/// Implementa el patrón Singleton para asegurar que solo haya una instancia de PlayerController.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Patrón Singleton: Permite acceder a esta instancia desde cualquier script.
    public static PlayerController Instance { get; private set; }

    /// <summary>
    /// Define los modos de luz disponibles para el jugador.
    /// </summary>
    public enum LightMode { Wave, Particle }

    [Header("Configuración de modalidad luz")]
    [Tooltip("El modo de luz actual del jugador.")]
    [SerializeField] private LightMode currentMode = LightMode.Wave;
    /// <summary>
    /// Propiedad pública para obtener el modo de luz actual.
    /// </summary>
    public LightMode CurrentMode => currentMode;

    [Header("Movimiento")]
    [Tooltip("Velocidad de movimiento lateral del jugador.")]
    [SerializeField] private float lateralSpeed = 10f;
    [Tooltip("Velocidad inicial de avance del jugador.")]
    [SerializeField] private float initialForwardSpeed = 10f;
    [Tooltip("Retraso antes de que la aceleración hacia adelante comience (actualmente no se usa directamente en Update).")]
    [SerializeField] private float accelerationDelay = 2f; // Este campo está declarado pero no se usa directamente en Update para un retraso inicial. La aceleración es constante.
    [Tooltip("Tasa de aceleración de la velocidad de avance por segundo.")]
    [SerializeField] private float forwardAccelerationRate = 0.5f;
    [Tooltip("Velocidad máxima a la que el jugador puede avanzar.")]
    [SerializeField] private float maxForwardSpeed = 200f;
    [Tooltip("Límite mínimo en el eje X para el movimiento lateral.")]
    [SerializeField] private float xMin = -5f;
    [Tooltip("Límite máximo en el eje X para el movimiento lateral.")]
    [SerializeField] private float xMax = 5f;

    /// <summary>
    /// La velocidad actual de avance del jugador.
    /// </summary>
    public float ForwardSpeed { get; private set; }

    [Header("Visuales")]
    [Tooltip("Referencia al componente Text de la UI para mostrar el modo actual.")]
    [SerializeField] private Text modeText;
    [Tooltip("Referencia al componente Animator del jugador para controlar animaciones.")]
    [SerializeField] private Animator animator;
    [Tooltip("Color del material del jugador cuando está en modo 'Wave'.")]
    [SerializeField] private Color waveColor = Color.white;
    [Tooltip("Color del material del jugador cuando está en modo 'Particle'.")]
    [SerializeField] private Color particleColor = Color.cyan;
    [Tooltip("Intensidad de emisión del material del jugador para el efecto de luz.")]
    [SerializeField] private float emissionIntensity = 7f;

    // Posición objetivo para el movimiento lateral suave.
    private Vector3 targetPosition;
    // Referencia al componente Renderer del jugador.
    private Renderer playerRenderer;
    // Material instanciado para el jugador, permitiendo modificarlo sin afectar otros objetos con el mismo material.
    private Material playerMaterial;
    // Bandera para controlar si el controlador del jugador está activo.
    private bool isActive = true;

    /// <summary>
    /// Se llama cuando se carga el script y antes de que se llame a Start.
    /// Implementa la lógica del Singleton para asegurar una única instancia.
    /// </summary>
    private void Awake()
    {
        // Si ya existe una instancia y no es esta, destruir el objeto duplicado.
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"{nameof(PlayerController)}: instancia duplicada. Eliminando {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        // Asignar esta instancia como la única.
        Instance = this;
        // Opcional: Descomenta si quieres que persista entre escenas.
        // DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Se llama en el primer frame en que el script está activo.
    /// Inicializa las propiedades del jugador.
    /// </summary>
    private void Start()
    {
        InitializePlayer();
    }

    /// <summary>
    /// Inicializa el renderer, el material del jugador y establece la velocidad inicial.
    /// </summary>
    private void InitializePlayer()
    {
        // Obtener el componente Renderer del jugador.
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer == null)
        {
            Debug.LogError($"{nameof(PlayerController)} no encontró Renderer. Desactivando.");
            isActive = false; // Desactiva el controlador si no hay Renderer.
            return;
        }

        // Crear una nueva instancia del material para evitar modificar el asset original.
        playerMaterial = new Material(playerRenderer.material);
        playerRenderer.material = playerMaterial;
        // Habilitar la emisión en el material para el efecto de luz.
        playerMaterial.EnableKeyword("_EMISSION");

        // Establecer la posición objetivo inicial a la posición actual del jugador.
        targetPosition = transform.position;
        // Establecer la velocidad inicial de avance.
        ForwardSpeed = initialForwardSpeed;
        // Actualizar los visuales del jugador según el modo inicial.
        UpdateVisuals();
    }

    /// <summary>
    /// Se llama una vez por frame.
    /// Maneja el movimiento y el cambio de modo de luz, además de la aceleración constante.
    /// </summary>
    private void Update()
    {
        // Si el controlador no está activo, no hacer nada.
        if (!isActive) return;

        HandleMovement(); // Gestiona la entrada de movimiento y el movimiento lateral.
        HandleModeChange(); // Gestiona el cambio de modo de luz.

        // Aumentar la velocidad hacia adelante gradualmente hasta el límite máximo.
        ForwardSpeed += forwardAccelerationRate * Time.deltaTime;
        ForwardSpeed = Mathf.Clamp(ForwardSpeed, initialForwardSpeed, maxForwardSpeed);
    }

    /// <summary>
    /// Procesa la entrada del usuario para el movimiento lateral y actualiza la posición objetivo.
    /// </summary>
    private void HandleMovement()
    {
        MoveForward(); // Mueve el jugador hacia adelante constantemente.
        SmoothLateralMovement(); // Suaviza la transición a la posición lateral objetivo.

        float horizontalInput = Input.GetAxisRaw("Horizontal"); // Obtener la entrada horizontal (teclas A/D o flechas izquierda/derecha).
        if (horizontalInput < 0)
            MoveLeft(); // Mover a la izquierda si la entrada es negativa.
        else if (horizontalInput > 0)
            MoveRight(); // Mover a la derecha si la entrada es positiva.
        else
            StopMovement(); // Detener el movimiento lateral si no hay entrada.
    }

    /// <summary>
    /// Maneja la detección de la pulsación de la tecla 'M' para cambiar el modo de luz.
    /// </summary>
    private void HandleModeChange()
    {
        if (Input.GetKeyDown(KeyCode.M)) // Si se presiona la tecla 'M'.
        {
            SwitchLightMode(); // Cambiar el modo de luz.
        }
    }

    /// <summary>
    /// Mueve la posición objetivo del jugador hacia la izquierda, respetando los límites X.
    /// </summary>
    public void MoveLeft()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x - lateralSpeed * Time.deltaTime, xMin, xMax);
        SetAnimatorMoving(true); // Indica al animador que el jugador se está moviendo.
    }

    /// <summary>
    /// Mueve la posición objetivo del jugador hacia la derecha, respetando los límites X.
    /// </summary>
    public void MoveRight()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x + lateralSpeed * Time.deltaTime, xMin, xMax);
        SetAnimatorMoving(true); // Indica al animador que el jugador se está moviendo.
    }

    /// <summary>
    /// Detiene el movimiento lateral estableciendo la bandera de movimiento en el animador a falso.
    /// </summary>
    public void StopMovement()
    {
        SetAnimatorMoving(false); // Indica al animador que el jugador no se está moviendo lateralmente.
    }

    /// <summary>
    /// Mueve el jugador hacia adelante basado en la ForwardSpeed actual.
    /// </summary>
    private void MoveForward()
    {
        transform.Translate(Vector3.forward * ForwardSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Suaviza el movimiento lateral del jugador hacia la posición objetivo.
    /// </summary>
    private void SmoothLateralMovement()
    {
        Vector3 currentPos = transform.position;
        // Interpola la posición actual a la posición objetivo X, manteniendo Y y Z.
        Vector3 newPosition = Vector3.Lerp(currentPos, new Vector3(targetPosition.x, currentPos.y, currentPos.z), 7f * Time.deltaTime);
        transform.position = newPosition;
    }

    /// <summary>
    /// Establece el parámetro "IsMoving" en el animador del jugador.
    /// </summary>
    /// <param name="moving">True si el jugador se está moviendo lateralmente, false en caso contrario.</param>
    private void SetAnimatorMoving(bool moving)
    {
        if (animator != null)
            animator.SetBool("IsMoving", moving);
    }

    /// <summary>
    /// Cambia el modo de luz actual entre 'Wave' y 'Particle' y actualiza los visuales.
    /// También notifica al GameManager si existe.
    /// </summary>
    public void SwitchLightMode()
    {
        if (!isActive) return; // Si el controlador no está activo, no cambiar el modo.

        // Alterna entre Wave y Particle.
        currentMode = currentMode == LightMode.Wave ? LightMode.Particle : LightMode.Wave;
        UpdateVisuals(); // Actualiza el color y el texto del modo.

        // Si hay una instancia de GameManager, incrementa el contador de cambios de modo.
        if (GameManager.Instance != null)
            GameManager.Instance.IncrementModeSwitchCount();

        Debug.Log($"Modo de luz cambiado a: {currentMode}");
    }

    /// <summary>
    /// Actualiza el texto de la UI y el color del material del jugador según el modo de luz actual.
    /// </summary>
    private void UpdateVisuals()
    {
        if (!isActive) return; // Si el controlador no está activo, no actualizar visuales.

        // Actualiza el texto del modo en la UI.
        if (modeText != null)
        {
            modeText.text = "Modo: " + currentMode.ToString();
        }

        // Determina el color basado en el modo actual.
        Color color = currentMode == LightMode.Wave ? waveColor : particleColor;
        if (playerMaterial != null)
        {
            // Aplica el color base al material.
            playerMaterial.color = color;
            // Aplica el color de emisión con la intensidad definida.
            playerMaterial.SetColor("_EmissionColor", color * emissionIntensity);
        }
    }

    /// <summary>
    /// Se llama cuando el objeto es destruido.
    /// Limpia la referencia de la instancia Singleton para evitar referencias nulas.
    /// </summary>
    private void OnDestroy()
    {
        // Si esta instancia es la que está asignada al Singleton, la desvincula.
        if (Instance == this)
        {
            Instance = null;
        }
    }
}