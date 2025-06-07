using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // Necesario para IEnumerator

public class GameManager : MonoBehaviour
{
    // Singleton pattern para acceso global
    public static GameManager Instance;

    [Header("Puntuación y vidas")]
    [SerializeField] private int score = 0; // Puntuación actual del jugador
    [SerializeField] private int lives = 3; // Vidas actuales del jugador
    [SerializeField] private int maxLives = 5; // Máximo de vidas posibles

    [Header("UI")]
    // Estas variables NO DEBEN ASIGNARSE en el Inspector. Serán buscadas dinámicamente.
    private TMP_Text scoreText; // Texto UI para mostrar puntuación
    private TMP_Text livesText; // Texto UI para mostrar vidas
    private TMP_Text modeText;  // Texto UI para modo de juego

    // ESTE panel y texto SÍ pueden ser asignados en el Inspector si están en la GameScene.
    // Si gameOverPanel está en la GameOverScene, entonces se debería buscar allí o ser gestionado por otro script.
    [SerializeField] private GameObject gameOverPanel; // Panel de fin de juego (si está en la GameScene)
    [SerializeField] private TMP_Text gameOverText;    // Texto de fin de juego (si está en la GameScene)

    [Header("Configuración de juego")]
    [SerializeField] public float spawnIntervalInitial = 3f; // Intervalo inicial de spawn de obstáculos
    [SerializeField] private string gameSceneName = "GameScene"; // Nombre de la escena principal del juego
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
    private float timeElapsed = 0f;          // Tiempo transcurrido en la partida
    private int modeSwitchCount = 0;         // Contador de cambios de modo
    private int obstaclesAvoidedCount = 0;   // Contador de obstáculos evitados
    private int totalScore = 0;              // Puntuación acumulada total

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

    private void OnEnable()
    {
        // Suscribirse al evento de carga de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Desuscribirse para evitar fugas de memoria
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Este método se llamará CADA VEZ que una escena termine de cargar
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameManager: Escena cargada: {scene.name}");

        // Si la escena cargada es la escena de juego principal
        if (scene.name == gameSceneName) // Usamos gameSceneName para ser explícitos
        {
            // Intentar encontrar y asignar los elementos de la UI
            // Usamos una corrutina para asegurar que los objetos de UI estén completamente inicializados
            StartCoroutine(FindAndAssignGameUI());

            // Asegurarse de que el panel de Game Over esté oculto al inicio de la partida
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            
            Time.timeScale = 1f; // Asegura que el juego no esté pausado
            Cursor.visible = false; // Oculta el cursor durante el juego
            Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor en el centro

            // Reiniciar el spawner de obstáculos (esto es crucial para un reinicio limpio)
            if (ObstacleSpawner.Instance != null)
            {
                ObstacleSpawner.Instance.ResetSpawner(); // Implementaremos este método en ObstacleSpawner
                ObstacleSpawner.Instance.SetSpawnInterval(spawnIntervalInitial);
            }
        }
        else if (scene.name == gameOverSceneName)
        {
            // Si estamos en la escena de Game Over, ocultar la UI del juego principal si está persistiendo
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (livesText != null) livesText.gameObject.SetActive(false);
            if (modeText != null) modeText.gameObject.SetActive(false);
            // El gameOverPanel se debería manejar en la escena de Game Over directamente o por el script GameSummary
            if (gameOverPanel != null) gameOverPanel.SetActive(false); 

            // Asegurarse de que el cursor sea visible y desbloqueado en la escena de Game Over
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (scene.name == mainMenuSceneName)
        {
            // Si volvemos al menú principal, ocultar la UI del juego principal si está persistiendo
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (livesText != null) livesText.gameObject.SetActive(false);
            if (modeText != null) modeText.gameObject.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            
            // Asegúrate de que el cursor sea visible y desbloqueado en el menú
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Corrutina para buscar y asignar la UI de forma más robusta
    private IEnumerator FindAndAssignGameUI()
    {
        // Espera un frame para asegurar que todos los objetos de la UI están instanciados
        yield return null; 

        // Buscar los elementos de UI en la escena recién cargada por sus nombres.
        // ¡ES CRÍTICO que estos nombres coincidan exactamente con los de tus GameObjects de UI en el Canvas de la GameScene!
        scoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>(); 
        livesText = GameObject.Find("LivesText")?.GetComponent<TMP_Text>(); 
        modeText = GameObject.Find("ModeText")?.GetComponent<TMP_Text>();   

        // Mensajes de advertencia si no se encuentran los elementos.
        if (scoreText == null) Debug.LogWarning("GameManager: 'ScoreText' no encontrado en la escena. ¡Verifica el nombre y la existencia!");
        if (livesText == null) Debug.LogWarning("GameManager: 'LivesText' no encontrado en la escena. ¡Verifica el nombre y la existencia!");
        if (modeText == null) Debug.LogWarning("GameManager: 'ModeText' no encontrado en la escena. ¡Verifica el nombre y la existencia!");

        // Establece el texto del modo inicial si se encontró.
        if (modeText != null) modeText.text = "Modo: Wave"; // O el modo inicial por defecto de tu jugador

        // Una vez encontradas las referencias, actualiza la UI con los valores actuales del juego.
        UpdateUI(); // Llama a UpdateUI para mostrar los valores iniciales

        // Activa los GameObjects de texto de la UI, por si estaban ocultos.
        if (scoreText != null) scoreText.gameObject.SetActive(true);
        if (livesText != null) livesText.gameObject.SetActive(true);
        if (modeText != null) modeText.gameObject.SetActive(true);
    }

    // Start() se ejecuta solo una vez para el GameManager que persiste.
    private void Start()
    {
        // Aquí no necesitamos inicializar UI, eso lo hace OnSceneLoaded.
        // Aseguramos el intervalo de spawn inicial solo si el spawner ya está disponible.
        // Nota: Si el spawner se crea CON la GameScene, esta llamada en Start puede ser demasiado temprana
        // en el primer inicio desde MainMenu. OnSceneLoaded es más robusto para esto.
        // if (ObstacleSpawner.Instance != null)
        // {
        //     ObstacleSpawner.Instance.SetSpawnInterval(spawnIntervalInitial);
        // }
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
        UpdateUI(); // Llama a UpdateUI para reflejar el cambio inmediatamente
    }

    // Añade vidas al jugador (sin superar el máximo)
    public void GainLife(int amount)
    {
        if (isGameOver) return;

        lives = Mathf.Min(lives + amount, maxLives);
        UpdateUI(); // Llama a UpdateUI para reflejar el cambio inmediatamente
    }

    // Reduce una vida y verifica game over
    public void LoseLife()
    {
        if (isGameOver) return;

        lives--;
        UpdateUI(); // Llama a UpdateUI para reflejar el cambio inmediatamente

        if (lives <= 0)
        {
            TriggerGameOver();
        }
    }

    // Actualiza los elementos de la UI
    private void UpdateUI()
    {
        // Solo actualiza si las referencias de texto no son nulas (ya encontradas por FindAndAssignGameUI)
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
        SaveGameStats();     // Guarda estadísticas

        if (Application.CanStreamedLevelBeLoaded(gameOverSceneName))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
        else
        {
            Debug.LogError($"La escena {gameOverSceneName} no está en Build Settings o no existe.");
        }
    }

    // Reinicia el juego con valores iniciales
    public void RestartGame()
    {
        // Resetea los datos internos del GameManager (puntuación, vidas, etc.)
        ResetGame(); 
        
        // Asegúrate de que el tiempo esté corriendo para la carga de escena
        Time.timeScale = 1f;

        // Recarga la escena principal del juego. Esto activará OnSceneLoaded nuevamente.
        // Al usar gameSceneName, nos aseguramos de cargar la escena correcta, no la de Game Over.
        SceneManager.LoadScene(gameSceneName); 
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

    // Resetea valores del juego sin recargar escena (solo datos internos del GameManager)
public void ResetGame()
{
    score = 0;
    lives = maxLives; // Vidas se reestablecen al máximo
    timeElapsed = 0f;
    modeSwitchCount = 0;
    obstaclesAvoidedCount = 0;
    totalScore = 0;
    isGameOver = false; // El flag de Game Over debe resetearse
    Debug.Log($"GameManager: ResetGame() ejecutado. Vidas reestablecidas a: {lives}, Puntuación: {score}");
    // No llames a UpdateUI aquí si vas a cargar una escena inmediatamente,
    // ya que FindAndAssignGameUI en OnSceneLoaded se encargará de ello.
}

/// <summary>
/// Reinicia el juego actual: resetea las variables y carga la escena de juego.
/// Este método DEBERÍA llamarse desde el botón "Reiniciar Partida" de la pantalla de Game Over.
/// </summary>
public void StartNewGame() // Renombrado para mayor claridad, pero puedes seguir usando RestartGame si lo prefieres
{
    Debug.Log("GameManager: StartNewGame() llamado. Reseteando juego y cargando GameScene.");
    ResetGame(); // Primero, restablece el estado del juego
    Time.timeScale = 1f; // Asegura que el tiempo fluya normalmente
    SceneManager.LoadScene(gameSceneName); // Carga tu escena de juego (GameScene)
}

/// <summary>
/// Vuelve al menú principal: resetea las variables y carga la escena del menú principal.
/// Este método DEBERÍA llamarse desde el botón "Volver al Menú" de la pantalla de Game Over.
/// </summary>
public void ReturnToMainMenu()
{
    Debug.Log("GameManager: ReturnToMainMenu() llamado. Reseteando juego y cargando MainMenu.");
    ResetGame(); // Restablece el estado del juego antes de ir al menú
    Time.timeScale = 1f;
    SceneManager.LoadScene(mainMenuSceneName); // Carga tu escena del Menú Principal
}

// Puedes mantener este método LoadMainMenu si en alguna parte quieres ir al menú
// SIN resetear el estado del juego, pero esto es menos común.
// public void LoadMainMenu()
// {
//     Debug.Log("GameManager: Cargando menú principal (sin reset explícito de juego).");
//     Time.timeScale = 1f;
//     SceneManager.LoadScene(mainMenuSceneName);
// }

    // Carga el menú principal (alternativa, si se necesita un método más genérico sin resetear juego)
    public void LoadMainMenu() // Mantiene esta si se necesita como un método separado sin resetear el estado del juego.
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
        // El cursor se maneja en OnSceneLoaded para mainMenuSceneName
    }
}