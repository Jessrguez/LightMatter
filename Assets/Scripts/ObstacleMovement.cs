// ObstacleMovement.cs
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [SerializeField]
    public float speed;

    [Header("Configuración de Destrucción")]
    [SerializeField] private bool showDebugGizmo = true;

    [Header("Límites de Pantalla")]
    [SerializeField] private float outOfBoundsX = -10f;
    [SerializeField] private float persistTimeOffScreen = 3f;

    private float offScreenTimer = 0f;
    private bool isOffScreen = false;
    private Vector3 initialScale;

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        if (transform.position.x < outOfBoundsX && !isOffScreen)
        {
            isOffScreen = true;
            offScreenTimer = 0f;
        }

        if (isOffScreen)
        {
            offScreenTimer += Time.deltaTime;
            if (offScreenTimer >= persistTimeOffScreen)
            {
                ReturnToPool();
            }
        }
    }

    public void HandlePlayerCollision()
    {
        PlayCollisionEffect();
        ReturnToPool();
    }

    private void PlayCollisionEffect()
    {
        // Agregar efecto visual o de sonido aquí
        Debug.Log("Efecto de colisión reproducido.");
    }

    private void ReturnToPool()
    {
        if (ObstacleSpawner.Instance != null)
        {
            isOffScreen = false;
            offScreenTimer = 0f;
            transform.localScale = initialScale;

            gameObject.SetActive(false);
            // Corrected line:
            ObstacleSpawner.Instance.ReturnObstacleToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmo) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(outOfBoundsX, transform.position.y, transform.position.z));
        Gizmos.DrawWireCube(new Vector3(outOfBoundsX, transform.position.y, transform.position.z), new Vector3(1f, 2f, 1f));
    }
}