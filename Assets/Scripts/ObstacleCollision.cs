// ObstacleCollision.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObstacleCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Opción 1: Lógica directa
            ObstacleSpawner.Instance?.ReturnObstacleToPool(gameObject); // Corrected method call
            GameManager.Instance?.PlayerHitObstacle();

            // Opción 2: Si usas ObstacleMovement (recomendado)
            /*
            var obstacleMovement = GetComponent<ObstacleMovement>();
            if (obstacleMovement != null)
            {
                obstacleMovement.HandlePlayerCollision();
            }
            GameManager.Instance?.PlayerHitObstacle();
            */
        }
    }
}