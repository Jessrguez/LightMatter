using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner Instance;

    [Header("Prefabs de Obstáculos")]
    public GameObject waveObstaclePrefab;
    public GameObject particleObstaclePrefab;

    [Header("Configuración de Spawn")]
    public float spawnIntervalInitial = 1.5f;
    public float spawnIntervalMin = 0.5f;
    public float difficultyIncreaseInterval = 10f;
    public float spawnDuration = 30f;
    public float spawnXMin = -4f;
    public float spawnXMax = 4f;
    public float spawnZStart = 30f;

    [Header("Configuración de Movimiento")]
    public float obstacleSpeedMin = 8f;
    public float obstacleSpeedMax = 12f;
    public float obstacleDestroyZ = -15f;

    private float timerDifficulty = 0f;
    private float spawnIntervalCurrent;
    private float spawnTimer = 0f;
    private bool isSpawning = true;
    private float timer = 0f;
    private int ultimoTipoGenerado = -1;

    private ObjectPool waveObstaclePool;
    private ObjectPool particleObstaclePool;

    public void UpdateSpawnInterval(float newInterval)
    {
        spawnIntervalCurrent = newInterval;
        Debug.Log("Spawn interval updated to " + spawnIntervalCurrent);
    }

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
        if (waveObstaclePrefab != null)
            waveObstaclePool = new ObjectPool(waveObstaclePrefab, 10, 20, transform);
        else
            Debug.LogError("¡No se ha asignado el prefab de obstáculo de ondas!");

        if (particleObstaclePrefab != null)
            particleObstaclePool = new ObjectPool(particleObstaclePrefab, 10, 20, transform);
        else
            Debug.LogError("¡No se ha asignado el prefab de obstáculo de partículas!");

        isSpawning = true;
        spawnTimer = 0f;
    }

    private void Update()
    {
        if (!isSpawning) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnDuration)
        {
            isSpawning = false;
            Debug.Log("Spawning de obstáculos detenido.");
            return;
        }

        timerDifficulty += Time.deltaTime;
        if (timerDifficulty >= difficultyIncreaseInterval)
        {
            timerDifficulty = 0f;
            IncreaseDifficulty();
        }

        timer += Time.deltaTime;
        if (timer >= spawnIntervalCurrent)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void IncreaseDifficulty()
    {
        spawnIntervalCurrent = Mathf.Max(spawnIntervalCurrent - 0.15f, spawnIntervalMin);
        UpdateSpawnInterval(spawnIntervalCurrent); // Usar la función pública para el log
        if (GameManager.Instance != null)
            GameManager.Instance.ShowFeedback("¡Dificultad aumentada!");
    }

    void SpawnObstacle()
    {
        if (waveObstaclePrefab == null || particleObstaclePrefab == null)
        {
            Debug.LogError("¡No se pueden generar obstáculos porque faltan prefabs!");
            return;
        }

        float spawnX = Random.Range(spawnXMin, spawnXMax);
        Vector3 spawnPosition = new Vector3(spawnX, 1f, spawnZStart);
        GameObject obstacle = null;
        GameObject prefabToInstantiate = null;

        if (ultimoTipoGenerado == 0)
        {
            obstacle = particleObstaclePool.GetObject();
            prefabToInstantiate = particleObstaclePrefab;
            ultimoTipoGenerado = 1;
        }
        else
        {
            obstacle = waveObstaclePool.GetObject();
            prefabToInstantiate = waveObstaclePrefab;
            ultimoTipoGenerado = 0;
        }

        if (obstacle != null)
        {
            obstacle.transform.position = spawnPosition;
            obstacle.SetActive(true); // Asegurarse de que el objeto esté activo

            PooledObject pooledObject = obstacle.GetComponent<PooledObject>();
            if (pooledObject == null)
            {
                pooledObject = obstacle.AddComponent<PooledObject>();
            }
            pooledObject.OriginalPrefab = prefabToInstantiate;

            ObstacleMovement obstacleMover = obstacle.GetComponent<ObstacleMovement>();
            if (obstacleMover == null)
            {
                obstacleMover = obstacle.AddComponent<ObstacleMovement>();
            }

            float playerForwardSpeed = (PlayerController.FindObjectOfType<PlayerController>() != null) ? PlayerController.FindObjectOfType<PlayerController>().forwardSpeed : 10f;
            float speedOffset = Random.Range(-2f, 2f);
            obstacleMover.speed = playerForwardSpeed + speedOffset;
            obstacleMover.destroyZ = obstacleDestroyZ;
        }
    }

    public void ReturnObstacle(GameObject obstacle)
    {
        if (obstacle != null)
        {
            obstacle.SetActive(false);
            PooledObject pooledObject = obstacle.GetComponent<PooledObject>();
            if (pooledObject != null && pooledObject.OriginalPrefab == waveObstaclePrefab)
            {
                waveObstaclePool.ReturnObject(obstacle);
            }
            else if (pooledObject != null && pooledObject.OriginalPrefab == particleObstaclePrefab)
            {
                particleObstaclePool.ReturnObject(obstacle);
            }
            else
            {
                Debug.LogError("Tried to return an obstacle that doesn't belong to either pool or has no PooledObject component.");
                Destroy(obstacle); // Fallback
            }
        }
    }
}