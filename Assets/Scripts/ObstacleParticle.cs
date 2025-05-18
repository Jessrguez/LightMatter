using UnityEngine;

public class ObstacleParticle : ObstacleBase
{
    public GameObject electronEffectPrefab;

    protected override bool IsCorrectMode(PlayerController.LightMode currentMode)
    {
        if (currentMode == PlayerController.LightMode.Particle)
        {
            if (electronEffectPrefab != null)
                Instantiate(electronEffectPrefab, transform.position + Vector3.up, Quaternion.identity);
            return true;
        }
        return false;
    }
}
