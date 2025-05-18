using UnityEngine;

public class ObstacleWave : ObstacleBase
{
    protected override bool IsCorrectMode(PlayerController.LightMode currentMode)
    {
        return currentMode == PlayerController.LightMode.Wave;
    }
}
