// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton pattern para acceso global
    public static GameManager Instance;

    [Header("Puntuación y vidas")]
    [SerializeField] private int score = 0;         // Puntuación actual del jugador
    [SerializeField] private int lives = 3;        // Vidas actuales del jugador
    [SerializeField] private int maxLives = 5;      // Máximo de vidas posibles

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;    // Texto UI para mostrar puntuación
    [SerializeField] private TMP_Text livesText;    // Texto UI para mostrar vidas
    [SerializeField] private TMP_Text modeText;     // Texto UI para modo de juego
    [SerializeField] private GameObject gameOverPanel; // Panel de fin de juego
    [SerializeField] private TMP_Text gameOverText;  // Texto de fin de juego

    [Header("Configuración de juego")]
    [SerializeField] public float spawnIntervalInitial = 3f; // Intervalo inicial de spawn de obstáculos
    [SerializeField] private string gameOverSceneName = "GameOverScene"; // Escena de Game Over
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Escena del Menú Principal

    // Propiedad para acceder/modificar el intervalo de spawn actual
    public float SpawnInterval
    {
        get => ObstacleSpawner.Instance != null ? ObstacleSpawner.Instance.spawnIntervalCurrent : 0f;
        set { if (ObstacleSpawner.Instance != null) ObstacleSpawner.Instance.SetSpawnInterval(value); }
    }

    // Variables de estado del juego
    private bool isGameOver = false;
    private float timeElapsed = 0f;         // Tiempo transcurrido en la partida
    private int modeSwitchCount = 0;        // Contador de cambios de modo
    private int obstaclesAvoidedCount = 0;  // Contador de obstáculos evitados
    private int totalScore = 0;             // Puntuación acumulada total

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
        }
        else
        {
            Destroy(gameObject); // Evita duplicados
        }
    }

    private void Start()
    {
        // Inicialización del juego
        SpawnInterval = spawnIntervalInitial;
        UpdateUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (modeText != null) modeText.text = "";
        Time.timeScale = 1f; // Asegura que el juego no esté pausado
        Cursor.visible = false; // Oculta el cursor durante el juego
    }

    private void Update()
    {
        if (isGameOver) return; // No actualizar si el juego terminó
        
        timeElapsed += Time.deltaTime; // Actualiza el contador de tiempo
    }

    // Añade puntos al jugador
    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        totalScore += amount;
        UpdateUI();
    }

    // Añade vidas al jugador (sin superar el máximo)
    public void GainLife(int amount)
    {
        if (isGameOver) return;

        lives = Mathf.Min(lives + amount, maxLives);
        UpdateUI();
    }

    // Reduce una vida y verifica game over
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

    // Actualiza los elementos de la UI
    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Puntuación: {score}";

        if (livesText != null)
            livesText.text = $"Vidas: {lives}";
    }

    // Maneja la lógica de fin de juego
    private void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f; // Pausa el juego
        SaveGameStats(); // Guarda estadísticas

        if (Application.CanStreamedLevelBeLoaded(gameOverSceneName))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
        else
        {
            Debug.LogError($"La escena {gameOverSceneName} no está en Build Settings!");
        }
    }

    // Reinicia el juego con valores iniciales
    public void RestartGame()
    {
        Time.timeScale = 1f;
        // Reset de variables de estado
        score = 0;
        lives = maxLives;
        timeElapsed = 0f;
        modeSwitchCount = 0;
        obstaclesAvoidedCount = 0;
        totalScore = 0;
        isGameOver = false;
        // Recarga la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Incrementa contador de cambios de modo
    public void IncrementModeSwitchCount()
    {
        modeSwitchCount++;
    }

    // Incrementa contador de obstáculos evitados
    public void IncrementObstaclesAvoidedCount()
    {
        obstaclesAvoidedCount++;
    }

    // Guarda estadísticas en PlayerPrefs
    private void SaveGameStats()
    {
        PlayerPrefs.SetFloat("SurvivalTime", timeElapsed);
        PlayerPrefs.SetInt("ModeSwitches", modeSwitchCount);
        PlayerPrefs.SetInt("ObstaclesAvoided", obstaclesAvoidedCount);
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.Save();
    }

    // Lógica cuando el jugador choca con obstáculo
    public void PlayerHitObstacle()
    {
        LoseLife();
    }

    // Métodos de acceso a estadísticas
    public float GetSurvivalTime() { return timeElapsed; }
    public int GetModeSwitchCount() { return modeSwitchCount; }
    public int GetObstaclesAvoidedCount() { return obstaclesAvoidedCount; }
    public int GetTotalScore() { return totalScore; }

    // Resetea valores del juego sin recargar escena
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

    // Vuelve al menú principal
    public void ReturnToMainMenu()
    {
        ResetGame();
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Carga el menú principal (alternativa)
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
        // Restablece estado del cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}