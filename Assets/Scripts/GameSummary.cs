using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSummary : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text timeSurvivedText;       // Muestra tiempo sobrevivido
    [SerializeField] private TMP_Text modeSwitchesText;       // Muestra cambios de modo
    [SerializeField] private TMP_Text obstaclesAvoidedText;   // Muestra obstáculos evitados
    [SerializeField] private TMP_Text totalScoreText;         // Muestra puntuación total
    
    [Header("Configuración")]
    [SerializeField] private string mainMenuScene = "MainMenu"; // Nombre de la escena del menú principal
    
    private void Start()
    {
        // Mostrar y liberar el cursor para interacción con UI
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Cargar y mostrar los datos del juego
        LoadGameData();
    }

    /// <summary>
    /// Reinicia el juego volviendo al menú principal
    /// </summary>
    public void RestartGame()
    {
        // Prioriza usar GameManager si existe
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
        // Fallback directo a carga de escena
        else
        {
            Time.timeScale = 1f; // Asegura que el tiempo fluya normalmente
            SceneManager.LoadScene(mainMenuScene);
        }
    }

    /// <summary>
    /// Alternativa para botón "Volver al menú" (reutiliza RestartGame)
    /// </summary>
    public void ReturnToMenu()
    {
        RestartGame(); // Misma funcionalidad que RestartGame
    }

    /// <summary>
    /// Carga y muestra las estadísticas del juego desde GameManager o PlayerPrefs
    /// </summary>
    private void LoadGameData()
    {
        // Intenta obtener datos del GameManager primero
        if (GameManager.Instance != null)
        {
            timeSurvivedText.text = $"Tiempo: {GameManager.Instance.GetSurvivalTime():F2} segundos";
            modeSwitchesText.text = $"Cambios: {GameManager.Instance.GetModeSwitchCount()}";
            obstaclesAvoidedText.text = $"Obstáculos: {GameManager.Instance.GetObstaclesAvoidedCount()}";
            totalScoreText.text = $"PUNTUACIÓN: <color=yellow><size=40>{GameManager.Instance.GetTotalScore()}</size></color>";
        }
        // Fallback a PlayerPrefs si no hay GameManager
        else
        {
            float time = PlayerPrefs.GetFloat("SurvivalTime", 0f);
            int switches = PlayerPrefs.GetInt("ModeSwitches", 0);
            int obstacles = PlayerPrefs.GetInt("ObstaclesAvoided", 0);
            int score = PlayerPrefs.GetInt("TotalScore", 0);

            // Asigna valores con formato visual mejorado
            timeSurvivedText.text = $"Tiempo: <color=#FFD700>{time:F2}</color> segundos";
            modeSwitchesText.text = $"Cambios: <color=#00FF7F>{switches}</color>";
            obstaclesAvoidedText.text = $"Obstáculos: <color=#1E90FF>{obstacles}</color>";
            totalScoreText.text = $"Puntaje: <size=36><color=#FF4500>{score}</color></size>";
        }
    }
}