// ObstacleSpawner.cs
using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner Instance { get; private set; }

    [Header("Obstacle Prefabs")]
    [SerializeField] private GameObject waveObstaclePrefab;
    [SerializeField] private GameObject particleObstaclePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float[] spawnZPositions = { 30f, 40f, 50f, 60f };
    public float spawnIntervalCurrent { get; private set; }
    [SerializeField] private float initialSpawnInterval = 200f;
    [SerializeField] private float minSpawnInterval = 100f;
    [SerializeField] private float spawnXRange = 4f;
    [SerializeField, Range(0f, 1f)] private float oppositeSpawnProbability = 0.5f;

    [Header("Movement Settings")]
    [SerializeField] private float minObstacleSpeed = 10f;
    [SerializeField] private float maxObstacleSpeed = 20f;

    private float spawnTimer;
    private int lastSpawnedType = -1;
    private Transform playerTransform;
    private ObjectPool waveObstaclePool;
    private ObjectPool particleObstaclePool;
    private bool isSpawningActive = true;

    private void Awake()
    {
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

    private void InitializePools()
    {
        if (waveObstaclePrefab != null)
            waveObstaclePool = new ObjectPool(waveObstaclePrefab, 5, 20, transform);
        if (particleObstaclePrefab != null)
            particleObstaclePool = new ObjectPool(particleObstaclePrefab, 5, 20, transform);
    }

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
        spawnTimer = initialSpawnInterval; // Spawn inmediato al inicio
    }

    private void Update()
    {
        if (!isSpawningActive || playerTransform == null) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= initialSpawnInterval)
        {
            SpawnObstacle();
            spawnTimer = 0f;
            AdjustSpawnInterval();
        }
    }

    private void AdjustSpawnInterval()
    {
        // Disminuye gradualmente el intervalo hasta alcanzar el mínimo
        initialSpawnInterval = Mathf.Max(minSpawnInterval, initialSpawnInterval * 0.98f);
    }

    private void SpawnObstacle()
    {
        Vector3 spawnPos = CalculateSpawnPosition();
        GameObject obstacle = GetNextObstacle();

        if (obstacle != null)
        {
            SetupObstacle(obstacle, spawnPos);
        }
    }

    private Vector3 CalculateSpawnPosition()
    {
        float spawnZ = playerTransform.position.z + spawnZPositions[Random.Range(0, spawnZPositions.Length)];
        float spawnX = Random.Range(-spawnXRange, spawnXRange);
        return new Vector3(spawnX, 1f, spawnZ);
    }

    private GameObject GetNextObstacle()
    {
        int nextType = DetermineNextObstacleType();
        lastSpawnedType = nextType;

        return nextType == 0 ?
            waveObstaclePool?.GetObject() :
            particleObstaclePool?.GetObject();
    }

    private int DetermineNextObstacleType()
    {
        if (lastSpawnedType == -1)
            return Random.Range(0, 2);

        return Random.value < oppositeSpawnProbability ?
            1 - lastSpawnedType :
            lastSpawnedType;
    }

private void SetupObstacle(GameObject obstacle, Vector3 position)
{
    obstacle.transform.position = position;
    obstacle.transform.localScale = Vector3.one; // Asegura escala uniforme
    obstacle.SetActive(true);

    var mover = obstacle.GetComponent<ObstacleMovement>();
    if (mover != null)
    {
        mover.speed = Random.Range(minObstacleSpeed, maxObstacleSpeed);
    }
}

    public void ReturnObstacleToPool(GameObject obstacle) // Nombre consistente
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
                Destroy(obstacle);
                break;
        }
    }

    public void SetSpawnInterval(float newInterval)
    {
        spawnIntervalCurrent = Mathf.Max(minSpawnInterval, newInterval);
    }

    public void SetSpawningActive(bool active)
    {
        isSpawningActive = active;
    }

    public void ResetSpawner()
    {
        initialSpawnInterval = 2.5f;
        spawnTimer = 0f;
        lastSpawnedType = -1;
    }
}