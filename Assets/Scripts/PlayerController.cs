using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public enum LightMode { Wave, Particle }

    [Header("Configuración de modalidad luz")]
    [SerializeField] private LightMode currentMode = LightMode.Wave;
    public LightMode CurrentMode => currentMode;

    [Header("Movimiento")]
    [SerializeField] private float lateralSpeed = 10f;
    [SerializeField] private float initialForwardSpeed = 10f;
    [SerializeField] private float accelerationDelay = 2f;
    [SerializeField] private float forwardAccelerationRate = 0.5f;
    [SerializeField] private float maxForwardSpeed = 200f;
    [SerializeField] private float xMin = -5f;
    [SerializeField] private float xMax = 5f;

    public float ForwardSpeed { get; private set; }

    [Header("Visuales")]
    [SerializeField] private Text modeText;
    [SerializeField] private Animator animator;
    [SerializeField] private Color waveColor = Color.white;
    [SerializeField] private Color particleColor = Color.cyan;
    [SerializeField] private float emissionIntensity = 7f;

    private Vector3 targetPosition;
    private Renderer playerRenderer;
    private Material playerMaterial;
    private bool isActive = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"{nameof(PlayerController)}: instancia duplicada. Eliminando {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Opcional: Descomenta si quieres que persista entre escenas
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer == null)
        {
            Debug.LogError($"{nameof(PlayerController)} no encontró Renderer. Desactivando.");
            isActive = false;
            return;
        }

        playerMaterial = new Material(playerRenderer.material);
        playerRenderer.material = playerMaterial;
        playerMaterial.EnableKeyword("_EMISSION");

        targetPosition = transform.position;
        ForwardSpeed = initialForwardSpeed;
        UpdateVisuals();
    }

    private void Update()
    {
        if (!isActive) return;

        HandleMovement();
        HandleModeChange();

        // Aumentar la velocidad hacia adelante gradualmente
        ForwardSpeed += forwardAccelerationRate * Time.deltaTime;
        ForwardSpeed = Mathf.Clamp(ForwardSpeed, initialForwardSpeed, maxForwardSpeed);
    }

    private void HandleMovement()
    {
        MoveForward();
        SmoothLateralMovement();

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput < 0)
            MoveLeft();
        else if (horizontalInput > 0)
            MoveRight();
        else
            StopMovement();
    }

    private void HandleModeChange()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SwitchLightMode();
        }
    }

    public void MoveLeft()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x - lateralSpeed * Time.deltaTime, xMin, xMax);
        SetAnimatorMoving(true);
    }

    public void MoveRight()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x + lateralSpeed * Time.deltaTime, xMin, xMax);
        SetAnimatorMoving(true);
    }

    public void StopMovement()
    {
        SetAnimatorMoving(false);
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * ForwardSpeed * Time.deltaTime);
    }

    private void SmoothLateralMovement()
    {
        Vector3 currentPos = transform.position;
        Vector3 newPosition = Vector3.Lerp(currentPos, new Vector3(targetPosition.x, currentPos.y, currentPos.z), 7f * Time.deltaTime);
        transform.position = newPosition;
    }

    private void SetAnimatorMoving(bool moving)
    {
        if (animator != null)
            animator.SetBool("IsMoving", moving);
    }

    public void SwitchLightMode()
    {
        if (!isActive) return;

        currentMode = currentMode == LightMode.Wave ? LightMode.Particle : LightMode.Wave;
        UpdateVisuals();
        
        if (GameManager.Instance != null)
            GameManager.Instance.IncrementModeSwitchCount();
            
        Debug.Log($"Modo de luz cambiado a: {currentMode}");
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayModeSwitchSound();
    }

    private void UpdateVisuals()
    {
        if (!isActive) return;

        if (modeText != null)
        {
            modeText.text = "Modo: " + currentMode.ToString();
        }

        Color color = currentMode == LightMode.Wave ? waveColor : particleColor;
        if (playerMaterial != null)
        {
            playerMaterial.color = color;
            playerMaterial.SetColor("_EmissionColor", color * emissionIntensity);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}