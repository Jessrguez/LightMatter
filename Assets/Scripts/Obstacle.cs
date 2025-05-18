// Obstacle.cs
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Tooltip("Posición en X donde el obstáculo se considera fuera de la pantalla por la izquierda.")]
    public float outOfBoundsX = -30f;

    private void Update()
    {
        // Desactivar y devolver el obstáculo al pool si se sale de la pantalla por la izquierda
        if (transform.position.x < outOfBoundsX)
        {
            gameObject.SetActive(false);
            ObstacleSpawner.Instance?.ReturnObstacleToPool(gameObject); // Corrected method call
        }
    }
}