using UnityEngine;

public class PlayerInputKeyboard : MonoBehaviour
{
    public PlayerController playerController;

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput < 0)
        {
            playerController.MoveLeft();
        }
        else if (horizontalInput > 0)
        {
            playerController.MoveRight();
        }
        else
        {
            playerController.StopMovement();
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.M))
        {
            playerController.ToggleMode();
        }
    }
}
