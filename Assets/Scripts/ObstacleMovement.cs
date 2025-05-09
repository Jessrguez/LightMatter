using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [HideInInspector] public float speed = 10f;
    [HideInInspector] public float destroyZ = -15f;

    private void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        if (transform.position.z < destroyZ)
        {
            gameObject.SetActive(false);
            if (ObstacleSpawner.Instance != null)
            {
                ObstacleSpawner.Instance.ReturnObstacle(gameObject);
            }
            else
            {
                Debug.LogWarning("ObstacleSpawner instance not found. Destroying obstacle as fallback.");
                Destroy(gameObject);
            }
        }
    }
}