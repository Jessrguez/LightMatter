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
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject howToPlayPanel;

    [Header("Tutorial Elements")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private float imageTransitionTime = 1.5f;

    [Header("Tutorial Content")]
    [SerializeField] private string[] tutorialSteps = new string[4]
    {
        // Paso 1 - Concepto básico
        "PRINCIPIO DE DUALIDAD\n\n" +
        "Tu esfera existe en dos estados:\n" +
        "• <color=#4FC3F7>MODO ONDA</color>: Inmune a obstáculos azules\n" +
        "• <color=#FF8A65>MODO PARTÍCULA</color>: Inmune a obstáculos rojos\n\n" +
        "<size=22>¡Cambia de estado para sobrevivir!</size>",

        // Paso 2 - Controles
        "CONTROLES DEL EXPERIMENTO\n\n" +
        "• Movimiento: ← → (Teclas flecha)\n" +
        "• Cambio de modo: <color=#FFFFFF>Barra espaciadora</color> o Tecla <color=#FFFFFF>M</color>\n\n" +
        "<size=22>El cambio es instantáneo y estratégico</size>",

        // Paso 3 - Obstáculos
        "PELIGROS CUÁNTICOS\n\n" +
        "• <color=#4FC3F7>Obstáculos ONDA</color> (azules):\n" +
        "  - Peligrosos en modo Partícula\n" +
        "  - Atraviesalos en modo Onda\n\n" +
        "• <color=#FF8A65>Obstáculos PARTÍCULA</color> (rojos):\n" +
        "  - Peligrosos en modo Onda\n" +
        "  - Atraviesalos en modo Partícula",

        // Paso 4 - Puntuación
        "SISTEMA DE PUNTUACIÓN\n\n" +
        "• +10 puntos por segundo sobrevivido\n" +
        "• +100 puntos por cada cambio de modo exitoso\n" +
        "• Bonus por proximidad a obstáculos\n\n" +
        "<size=26>¡Supera tu récord cuántico!</size>"
    };

    private int currentStep = 0;
    private Coroutine tutorialCoroutine;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

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
        mainMenuPanel.SetActive(true);
        howToPlayPanel.SetActive(false);

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("startButton no está asignado en el Inspector", this);
        }

        if (howToPlayButton != null)
        {
            howToPlayButton.onClick.AddListener(ShowHowToPlay);
        }
        else
        {
            Debug.LogError("howToPlayButton no está asignado en el Inspector", this);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(ShowMainMenu);
        }
        else
        {
            Debug.LogError("backButton no está asignado en el Inspector", this);
        }
    }

    private void StartGame()
    {
        // Asegúrate de que la escena existe en Build Settings
        if (Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            StartCoroutine(LoadGameSceneWithTransition());
        }
        else
        {
            Debug.LogError($"No se puede cargar la escena: {gameSceneName}. Verifica Build Settings.");
        }
    }

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

    private void ShowHowToPlay()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);

        currentStep = 0;
        UpdateTutorialStep(currentStep);
    }

    private void ShowMainMenu()
    {
        howToPlayPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        if (tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
            tutorialCoroutine = null;
        }
    }

    private void UpdateTutorialStep(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= tutorialSteps.Length)
            return;

        currentStep = stepIndex;
        tutorialText.text = tutorialSteps[stepIndex];
    }
}
