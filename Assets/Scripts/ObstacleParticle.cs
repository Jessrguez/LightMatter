using UnityEngine;

public class ObstacleParticle : MonoBehaviour
{
    private bool hit = false;
    public GameObject electronEffectPrefab;
    public AudioSource successSound;
    public AudioSource failSound;

    private void OnTriggerEnter(Collider other)
    {
        if (hit) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            if (player.currentMode == PlayerController.LightMode.Particle)
            {
                GameManager.Instance.AddScore(15);
                if (electronEffectPrefab != null)
                    Instantiate(electronEffectPrefab, transform.position + Vector3.up, Quaternion.identity);
                if (successSound != null) successSound.Play();
            }
            else
            {
                GameManager.Instance.LoseLife();
                if (failSound != null) failSound.Play();
            }
            hit = true;
            Destroy(gameObject, 0.3f);
        }
    }
}
