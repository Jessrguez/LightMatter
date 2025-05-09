using UnityEngine;

public class ObstacleWave : MonoBehaviour
{
    private bool passed = false;
    public AudioSource successSound;
    public AudioSource failSound;

    private void OnTriggerEnter(Collider other)
    {
        if (passed) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            if (player.currentMode == PlayerController.LightMode.Wave)
            {
                GameManager.Instance.AddScore(10);
                if (successSound != null) successSound.Play();
            }
            else
            {
                GameManager.Instance.LoseLife();
                if (failSound != null) failSound.Play();
            }
            passed = true;
            Destroy(gameObject, 0.2f);
        }
    }
}
