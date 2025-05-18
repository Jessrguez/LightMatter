using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSummary : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text timeSurvivedText;
    [SerializeField] private TMP_Text modeSwitchesText;
    [SerializeField] private TMP_Text obstaclesAvoidedText;
    [SerializeField] private TMP_Text totalScoreText;
    
    [Header("Configuración")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        LoadGameData();
    }

    public void RestartGame()
    {
        // Opción 1: Si tienes GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
        // Opción 2: Si no tienes GameManager
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuScene);
        }
    }

    public void ReturnToMenu()
    {
        RestartGame(); // Reutiliza la misma función
    }

    private void LoadGameData()
    {
        if (GameManager.Instance != null)
        {
            timeSurvivedText.text = $"Tiempo: {GameManager.Instance.GetSurvivalTime():F2} segundos";
            modeSwitchesText.text = $"Cambios: {GameManager.Instance.GetModeSwitchCount()}";
            obstaclesAvoidedText.text = $"Obstáculos: {GameManager.Instance.GetObstaclesAvoidedCount()}";
            totalScoreText.text = $"PUNTUACIÓN: <color=yellow><size=40>{GameManager.Instance.GetTotalScore()}</size></color>";
        }
        else
        {
            float time = PlayerPrefs.GetFloat("SurvivalTime", 0f);
            int switches = PlayerPrefs.GetInt("ModeSwitches", 0);
            int obstacles = PlayerPrefs.GetInt("ObstaclesAvoided", 0);
            int score = PlayerPrefs.GetInt("TotalScore", 0);

            timeSurvivedText.text = $"Tiempo: <color=#FFD700>{time:F2}</color> segundos";
            modeSwitchesText.text = $"Cambios: <color=#00FF7F>{switches}</color>";
            obstaclesAvoidedText.text = $"Obstáculos: <color=#1E90FF>{obstacles}</color>";
            totalScoreText.text = $"Puntaje: <size=36><color=#FF4500>{score}</color></size>";
        }
    }
}