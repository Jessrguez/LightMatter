using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum LightMode { Wave, Particle }
    public LightMode currentMode = LightMode.Wave;

    public float lateralSpeed = 8f; // Velocidad movimiento horizontal
    public float forwardSpeed = 10f; // Velocidad hacia adelante
    public float xMin = -4f;
    public float xMax = 4f;

    public Text modeText;
    public Animator animator;

    private Vector3 targetPosition;

    private Renderer playerRenderer;   // Renderer para cambiar el color

    // Colores definidos para modos
    public Color waveColor = Color.white;
    public Color particleColor = Color.blue;

    private void Start()
    {
        targetPosition = transform.position;
        playerRenderer = GetComponent<Renderer>();
        UpdateModeVisuals();   // Inicializar visuales y texto al inicio
    }

    private void Update()
    {
        MoveForward();
        SmoothMove();

        // Detectar la pulsación de la tecla "M" para cambiar el modo
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMode();
        }
    }

    public void MoveLeft()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x - lateralSpeed * Time.deltaTime, xMin, xMax);
        SetAnimatorMovement(true);
    }

    public void MoveRight()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x + lateralSpeed * Time.deltaTime, xMin, xMax);
        SetAnimatorMovement(true);
    }

    public void StopMovement()
    {
        SetAnimatorMovement(false);
    }

    void MoveForward()
    {
        transform.position += new Vector3(0, 0, forwardSpeed * Time.deltaTime);
    }

    void SmoothMove()
    {
        Vector3 currentPos = transform.position;
        Vector3 smoothPos = Vector3.Lerp(new Vector3(currentPos.x, currentPos.y, currentPos.z), new Vector3(targetPosition.x, currentPos.y, currentPos.z), 10f * Time.deltaTime);
        transform.position = smoothPos;
    }

    void SetAnimatorMovement(bool moving)
    {
        if (animator != null)
            animator.SetBool("IsMoving", moving);
    }

    public void ToggleMode()
    {
        Debug.Log("ToggleMode called!");
        currentMode = (currentMode == LightMode.Wave) ? LightMode.Particle : LightMode.Wave;
        Debug.Log("Current Mode: " + currentMode);
        UpdateModeVisuals();
    }

    // Actualiza el texto y el color del jugador según el modo actual
    void UpdateModeVisuals()
    {
        // Actualizar texto
        if (modeText != null)
            modeText.text = "Modo: " + currentMode.ToString();

        // Cambiar color del material para modo visible
        if (playerRenderer != null)
        {
            if (currentMode == LightMode.Wave)
                playerRenderer.material.color = waveColor;
            else
                playerRenderer.material.color = particleColor;
        }
    }
}