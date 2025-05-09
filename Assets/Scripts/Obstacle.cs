using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float destroyZ = -10f;
private void Update()
{
    if (transform.position.x < -30f) // Si el obstáculo se sale por la izquierda
    {
        gameObject.SetActive(false);
        ObstacleSpawner.Instance.ReturnObstacle(gameObject);
    }
}

}
