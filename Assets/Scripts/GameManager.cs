using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Puntuación y vidas")]
    public int score = 0;
    public int lives = 3;
    public int maxLives = 5;

    [Header("UI")]
    public Text scoreText;
    public Text livesText;
    public Text modeText;
    public GameObject gameOverPanel;
    public Text gameOverText;
    public Text feedbackText;

    [Header("Configuración de juego")]
    public float difficultyIncreaseInterval = 15f;
    private float timerDifficulty = 0f;
    public float spawnIntervalInitial = 2f;
    public float spawnIntervalMin = 0.8f;
    private float spawnIntervalCurrent;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        spawnIntervalCurrent = spawnIntervalInitial;
        UpdateUI();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (feedbackText != null)
            feedbackText.text = "";
        if (modeText != null)
            modeText.text = "";
    }

    private void Update()
    {
        if (isGameOver) return;

        timerDifficulty += Time.deltaTime;
        if (timerDifficulty >= difficultyIncreaseInterval)
        {
            timerDifficulty = 0f;
            IncreaseDifficulty();
        }
    }

    void IncreaseDifficulty()
    {
        spawnIntervalCurrent = Mathf.Max(spawnIntervalCurrent - 0.2f, spawnIntervalMin);
        ObstacleSpawner.Instance.UpdateSpawnInterval(spawnIntervalCurrent);
        ShowFeedback("¡Dificultad aumentada!");
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        UpdateUI();
        ShowFeedback("+ " + amount + " puntos");
    }

    public void GainLife(int amount)
    {
        if (isGameOver) return;

        lives = Mathf.Min(lives + amount, maxLives);
        UpdateUI();
        ShowFeedback("¡Vida recuperada!");
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        lives--;
        UpdateUI();
        ShowFeedback("¡Perdiste una vida!");
        if (lives <= 0)
            GameOver();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntuación: " + score;
        if (livesText != null)
            livesText.text = "Vidas: " + lives;
    }

    void GameOver()
    {
        isGameOver = true;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        if (gameOverText != null)
            gameOverText.text = "¡Juego Terminado!\nPuntuación Final: " + score + "\nPulsa para reiniciar";
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            StopAllCoroutines();
            StartCoroutine(FeedbackCoroutine(message));
        }
    }

    private IEnumerator FeedbackCoroutine(string message)
    {
        feedbackText.text = message;
        yield return new WaitForSecondsRealtime(1.5f);
        feedbackText.text = "";
    }
}
