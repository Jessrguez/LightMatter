using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class MainMenu : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("Nombre de la escena del juego.")]
    [SerializeField] private string gameSceneName = "GameScene"; // Escena principal del juego

    [Header("UI Elements")]
    [SerializeField] private Button startButton;         // Botón para iniciar el juego
    [SerializeField] private Button howToPlayButton;    // Botón para mostrar tutorial
    [SerializeField] private Button backButton;         // Botón para volver al menú
    [SerializeField] private GameObject mainMenuPanel;  // Panel del menú principal
    [SerializeField] private GameObject howToPlayPanel; // Panel del tutorial

    [Header("Tutorial Elements")]
    [SerializeField] private TextMeshProUGUI tutorialText; // Texto del tutorial
    [SerializeField] private float imageTransitionTime = 1.5f; // Tiempo entre pasos del tutorial

    [Header("Tutorial Content")]
    [SerializeField] private string[] tutorialSteps = new string[4] // Pasos del tutorial con formato
    {
        // Explicación del concepto de dualidad
        "PRINCIPIO DE DUALIDAD\n\n" +
        "Tu esfera existe en dos estados:\n" +
        "• <color=#4FC3F7>MODO ONDA</color>: Inmune a obstáculos azules\n" +
        "• <color=#FF8A65>MODO PARTÍCULA</color>: Inmune a obstáculos azules\n\n" +
        "<size=22>¡Cambia de estado para sobrevivir!</size>",

        // Instrucciones de controles
        "CONTROLES DEL EXPERIMENTO\n\n" +
        "• Movimiento: ← → (Teclas flecha)\n" +
        "• Cambio de modo: Tecla <color=#FFFFFF>M</color>\n\n" +
        "<size=22>El cambio es instantáneo y estratégico</size>",

        // Explicación de obstáculos
        "PELIGROS CUÁNTICOS\n\n" +
        "• <color=#4FC3F7>Obstáculos ONDA</color> (blancos):\n" +
        "  - Peligrosos en modo Partícula\n" +
        "  - Atraviesalos en modo Onda\n\n" +
        "• <color=#FF8A65>Obstáculos PARTÍCULA</color> (azules):\n" +
        "  - Peligrosos en modo Onda\n" +
        "  - Atraviesalos en modo Partícula",

        // Sistema de puntuación
        "SISTEMA DE PUNTUACIÓN\n\n" +
        "• +10 puntos por segundo sobrevivido\n" +
        "• +100 puntos por cada cambio de modo exitoso\n" +
        "• Bonus por proximidad a obstáculos\n\n" +
        "<size=26>¡Supera tu récord cuántico!</size>"
    };


    // Variables de estado
    private int currentStep = 0;              // Paso actual del tutorial
    private Coroutine tutorialCoroutine;      // Referencia a la corrutina del tutorial
    private CanvasGroup canvasGroup;          // Componente para efectos de transición

    private void Awake()
    {
        // Obtener referencia al CanvasGroup para transiciones
        canvasGroup = GetComponent<CanvasGroup>();

        // Buscar automáticamente los paneles si no están asignados
        if (mainMenuPanel == null)
        {
            mainMenuPanel = transform.Find("MainMenuPanel")?.gameObject;
            if (mainMenuPanel == null)
            {
                Debug.LogError("No se encontró mainMenuPanel.", this);
                enabled = false;
                return;
            }
        }

        if (howToPlayPanel == null)
        {
            howToPlayPanel = transform.Find("HowToPlayPanel")?.gameObject;
        }
    }

    private void Start()
    {
        // Configurar estado inicial de la UI
        mainMenuPanel.SetActive(true);
        howToPlayPanel.SetActive(false);

        // Configurar listeners de botones con validación
        SetupButton(startButton, StartGame, "startButton");
        SetupButton(howToPlayButton, ShowHowToPlay, "howToPlayButton");
        SetupButton(backButton, ShowMainMenu, "backButton");
    }

    /// <summary>
    /// Configura un botón con su listener y valida su existencia
    /// </summary>
    private void SetupButton(Button button, UnityEngine.Events.UnityAction action, string buttonName)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
        else
        {
            Debug.LogError($"{buttonName} no está asignado en el Inspector", this);
        }
    }

    /// <summary>
    /// Inicia el juego con transición de fade out
    /// </summary>
    private void StartGame()
    {
        if (Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            StartCoroutine(LoadGameSceneWithTransition());
        }
        else
        {
            Debug.LogError($"No se puede cargar la escena: {gameSceneName}. Verifica Build Settings.");
        }
    }

    /// <summary>
    /// Corrutina que realiza una transición de fade out antes de cargar la escena
    /// </summary>
    private IEnumerator LoadGameSceneWithTransition()
    {
        float fadeTime = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Muestra el panel de instrucciones y reinicia el tutorial
    /// </summary>
    private void ShowHowToPlay()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
        currentStep = 0;
        UpdateTutorialStep(currentStep);
    }

    /// <summary>
    /// Vuelve al menú principal desde cualquier pantalla
    /// </summary>
    private void ShowMainMenu()
    {
        howToPlayPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        // Detener cualquier corrutina de tutorial activa
        if (tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
            tutorialCoroutine = null;
        }
    }

    /// <summary>
    /// Actualiza el texto del tutorial según el paso actual
    /// </summary>
    private void UpdateTutorialStep(int stepIndex)
    {
        if (stepIndex >= 0 && stepIndex < tutorialSteps.Length)
        {
            currentStep = stepIndex;
            tutorialText.text = tutorialSteps[stepIndex];
        }
    }
}