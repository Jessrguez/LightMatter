// ObstacleBase.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class ObstacleBase : MonoBehaviour
{
    protected bool passed = false;

    [SerializeField] protected AudioSource successSound;
    [SerializeField] protected AudioSource failSound;
    [SerializeField] protected int scoreAmount = 10;

    protected Collider obstacleCollider;

    protected virtual void Awake()
    {
        obstacleCollider = GetComponent<Collider>();
        if (obstacleCollider == null)
        {
            Debug.LogError($"{nameof(ObstacleBase)}: No Collider found.");
        }
    }

    protected abstract bool IsCorrectMode(PlayerController.LightMode currentMode);

    private void OnTriggerEnter(Collider other)
    {
        if (passed) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        passed = true;

        if (IsCorrectMode(player.CurrentMode))
        {
            GameManager.Instance?.AddScore(scoreAmount);
            AudioManager.Instance?.PlayObstaclePassedSound();
        }
        else
        {
            GameManager.Instance?.LoseLife();
            AudioManager.Instance?.PlayObstacleHitSound();
        }

        if (obstacleCollider != null)
            obstacleCollider.enabled = false;
    }

    protected virtual void Update()
    {
        if (PlayerController.Instance == null) return;

        // Desactivar el obstáculo solo si ha pasado una cierta distancia DETRÁS del jugador
        if (transform.position.z < PlayerController.Instance.transform.position.z - 10f)
        {
            gameObject.SetActive(false);
            ObstacleSpawner.Instance?.ReturnObstacleToPool(gameObject); // Corrected method call
            if (!passed)
                GameManager.Instance?.IncrementObstaclesAvoidedCount();
        }
    }
    public virtual void ResetObstacle()
{
    passed = false;
    if (obstacleCollider != null)
    {
        obstacleCollider.enabled = true;
    }
}
}