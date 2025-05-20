// ObstacleSpawner.cs
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema que gestiona la generación y reciclaje de obstáculos usando Object Pooling
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    // Singleton para acceso global
    public static ObstacleSpawner Instance { get; private set; }

    [Header("Obstacle Prefabs")]
    [SerializeField] private GameObject waveObstaclePrefab;      // Prefab para obstáculos de onda
    [SerializeField] private GameObject particleObstaclePrefab; // Prefab para obstáculos de partícula

    [Header("Spawn Settings")]
    [SerializeField] private float[] spawnZPositions = { 30f, 40f, 50f, 60f }; // Posiciones Z posibles
    public float spawnIntervalCurrent { get; private set; }     // Intervalo actual (accesible públicamente)
    [SerializeField] private float initialSpawnInterval = 2f;   // Intervalo inicial de generación
    [SerializeField] private float minSpawnInterval = 2f;       // Intervalo mínimo (dificultad máxima)
    [SerializeField] private float spawnXRange = 4f;            // Rango horizontal de aparición
    [SerializeField, Range(0f, 1f)] private float oppositeSpawnProbability = 0.5f; // Probabilidad de cambiar tipo

    [Header("Movement Settings")]
    [SerializeField] private float minObstacleSpeed = 10f;      // Velocidad mínima
    [SerializeField] private float maxObstacleSpeed = 20f;      // Velocidad máxima

    // Variables de estado
    private float spawnTimer;               // Temporizador para generación
    private int lastSpawnedType = -1;       // Último tipo generado (-1 = ninguno)
    private Transform playerTransform;       // Referencia al jugador
    private ObjectPool waveObstaclePool;    // Pool para obstáculos onda
    private ObjectPool particleObstaclePool;// Pool para obstáculos partícula
    private bool isSpawningActive = true;   // Control de activación

    private void Awake()
    {
        // Configuración del singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        spawnIntervalCurrent = initialSpawnInterval;
        InitializePools();
        FindPlayer();
    }

    /// <summary>
    /// Inicializa los pools de objetos para cada tipo de obstáculo
    /// </summary>
    private void InitializePools()
    {
        if (waveObstaclePrefab != null)
            waveObstaclePool = new ObjectPool(waveObstaclePrefab, 5, 20, transform);
        if (particleObstaclePrefab != null)
            particleObstaclePool = new ObjectPool(particleObstaclePrefab, 5, 20, transform);
    }

    /// <summary>
    /// Busca y almacena referencia al jugador
    /// </summary>
    private void FindPlayer()
    {
        playerTransform = PlayerController.Instance?.transform;
        if (playerTransform == null)
        {
            var player = FindObjectOfType<PlayerController>();
            if (player != null) playerTransform = player.transform;
        }
    }

    private void Start()
    {
        spawnTimer = initialSpawnInterval; // Primer spawn inmediato
    }

    private void Update()
    {
        if (!isSpawningActive || playerTransform == null) return;

        // Lógica de generación por tiempo
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= initialSpawnInterval)
        {
            SpawnObstacle();
            spawnTimer = 0f;
            AdjustSpawnInterval(); // Aumenta dificultad
        }
    }

    /// <summary>
    /// Reduce gradualmente el intervalo de generación (aumenta dificultad)
    /// </summary>
    private void AdjustSpawnInterval()
    {
        initialSpawnInterval = Mathf.Max(minSpawnInterval, initialSpawnInterval * 0.98f);
    }

    /// <summary>
    /// Genera un nuevo obstáculo en posición aleatoria
    /// </summary>
    private void SpawnObstacle()
    {
        Vector3 spawnPos = CalculateSpawnPosition();
        GameObject obstacle = GetNextObstacle();

        if (obstacle != null)
        {
            SetupObstacle(obstacle, spawnPos);
        }
    }

    /// <summary>
    /// Calcula posición de aparición relativa al jugador
    /// </summary>
    private Vector3 CalculateSpawnPosition()
    {
        float spawnZ = playerTransform.position.z + spawnZPositions[Random.Range(0, spawnZPositions.Length)];
        float spawnX = Random.Range(-spawnXRange, spawnXRange);
        return new Vector3(spawnX, 1f, spawnZ); // Altura fija en Y=1
    }

    /// <summary>
    /// Decide y obtiene el próximo obstáculo a generar
    /// </summary>
    private GameObject GetNextObstacle()
    {
        int nextType = DetermineNextObstacleType();
        lastSpawnedType = nextType;

        return nextType == 0 ? 
            waveObstaclePool?.GetObject() : 
            particleObstaclePool?.GetObject();
    }

    /// <summary>
    /// Determina el tipo de obstáculo con probabilidad controlada
    /// </summary>
    private int DetermineNextObstacleType()
    {
        if (lastSpawnedType == -1) // Primera generación
            return Random.Range(0, 2);

        return Random.value < oppositeSpawnProbability ? 
            1 - lastSpawnedType : // Cambia tipo
            lastSpawnedType;       // Mismo tipo
    }

    /// <summary>
    /// Configura posición, escala y velocidad del obstáculo
    /// </summary>
    private void SetupObstacle(GameObject obstacle, Vector3 position)
    {
        obstacle.transform.position = position;
        obstacle.transform.localScale = Vector3.one;
        obstacle.SetActive(true);

        var mover = obstacle.GetComponent<ObstacleMovement>();
        if (mover != null)
        {
            mover.speed = Random.Range(minObstacleSpeed, maxObstacleSpeed);
        }
    }

    /// <summary>
    /// Devuelve un obstáculo al pool correspondiente según su tag
    /// </summary>
    public void ReturnObstacleToPool(GameObject obstacle)
    {
        if (obstacle == null) return;

        obstacle.SetActive(false);

        switch (obstacle.tag)
        {
            case "WaveObstacle":
                waveObstaclePool?.ReturnObject(obstacle);
                break;
            case "ParticleObstacle":
                particleObstaclePool?.ReturnObject(obstacle);
                break;
            default:
                Destroy(obstacle); // Fallback si no tiene tag válido
                break;
        }
    }

    // --- Métodos públicos para control externo ---
    
    /// <summary>
    /// Establece nuevo intervalo de generación (asegurando mínimo)
    /// </summary>
    public void SetSpawnInterval(float newInterval)
    {
        spawnIntervalCurrent = Mathf.Max(minSpawnInterval, newInterval);
    }

    /// <summary>
    /// Activa/desactiva la generación de obstáculos
    /// </summary>
    public void SetSpawningActive(bool active)
    {
        isSpawningActive = active;
    }

    /// <summary>
    /// Reinicia configuraciones al estado inicial
    /// </summary>
    public void ResetSpawner()
    {
        initialSpawnInterval = 2.5f;
        spawnTimer = 0f;
        lastSpawnedType = -1;
    }
}