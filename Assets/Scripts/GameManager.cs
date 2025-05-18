// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Puntuación y vidas")]
    [SerializeField] private int score = 0;
    [SerializeField] private int lives = 3;
    [SerializeField] private int maxLives = 5;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text modeText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;

    [Header("Configuración de juego")]
    [SerializeField] public float spawnIntervalInitial = 3f;
    [SerializeField] private string gameOverSceneName = "GameOverScene"; // Nombre de la escena de Game Over
    [SerializeField] private string mainMenuSceneName = "MainMenu"; //Nombre de la escena del Menú Principal

    public float SpawnInterval
    {
        get => ObstacleSpawner.Instance != null ? ObstacleSpawner.Instance.spawnIntervalCurrent : 0f;
        set { if (ObstacleSpawner.Instance != null) ObstacleSpawner.Instance.SetSpawnInterval(value); }
    }
    private bool isGameOver = false;

    private float timeElapsed = 0f;
    private int modeSwitchCount = 0;
    private int obstaclesAvoidedCount = 0;
    private int totalScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Inicializa la propiedad SpawnInterval con el valor inicial
        SpawnInterval = spawnIntervalInitial;
        UpdateUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (modeText != null) modeText.text = "";
        Time.timeScale = 1f;
        Cursor.visible = false; // Asegúrate de que el cursor esté oculto durante el juego
    }

    private void Update()
    {
        if (isGameOver)
        {
            return; // Ya no actualizamos el tiempo si el juego ha terminado
        }

        timeElapsed += Time.deltaTime;
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        totalScore += amount;
        UpdateUI();
    }

    public void GainLife(int amount)
    {
        if (isGameOver) return;

        lives = Mathf.Min(lives + amount, maxLives);
        UpdateUI();
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            TriggerGameOver();
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Puntuación: {score}";

        if (livesText != null)
            livesText.text = $"Vidas: {lives}";
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        SaveGameStats();

        // Verifica si la escena está en el build
        if (Application.CanStreamedLevelBeLoaded(gameOverSceneName))
        {
            SceneManager.LoadScene(gameOverSceneName);
            AudioManager.Instance?.PlayGameOverSound();
        }
        else
        {
            Debug.LogError($"La escena {gameOverSceneName} no está en Build Settings!");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        // Reiniciar variables
        score = 0;
        lives = maxLives;
        timeElapsed = 0f;
        modeSwitchCount = 0;
        obstaclesAvoidedCount = 0;
        totalScore = 0;
        isGameOver = false;
        // Carga la escena del juego actual para reiniciar el juego.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void IncrementModeSwitchCount()
    {
        modeSwitchCount++;
    }

    public void IncrementObstaclesAvoidedCount()
    {
        obstaclesAvoidedCount++;
    }

    private void SaveGameStats()
    {
        PlayerPrefs.SetFloat("SurvivalTime", timeElapsed);
        PlayerPrefs.SetInt("ModeSwitches", modeSwitchCount);
        PlayerPrefs.SetInt("ObstaclesAvoided", obstaclesAvoidedCount);
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.Save();
    }

    // Método para cuando el jugador choca con un obstáculo
    public void PlayerHitObstacle()
    {
        LoseLife();
        // Aquí podrías añadir efectos visuales, sonido, vibraciones, etc.
        // Ejemplo: Efecto de sonido en otro script
    }

    // Métodos para acceder a las estadísticas (necesario para la pantalla de Game Over)
    public float GetSurvivalTime() { return timeElapsed; }
    public int GetModeSwitchCount() { return modeSwitchCount; }
    public int GetObstaclesAvoidedCount() { return obstaclesAvoidedCount; }
    public int GetTotalScore() { return totalScore; }

    // Añade estos métodos a tu GameManager.cs
    public void ResetGame()
    {
        score = 0;
        lives = maxLives;
        timeElapsed = 0f;
        modeSwitchCount = 0;
        obstaclesAvoidedCount = 0;
        totalScore = 0;
        isGameOver = false;
        SpawnInterval = spawnIntervalInitial;
    }

    public void ReturnToMainMenu()
    {
        ResetGame();
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Añade este método a tu GameManager
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);

        // Opcional: Resetear valores si es necesario
        // ResetGame();

        // Asegurar que el cursor sea visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}