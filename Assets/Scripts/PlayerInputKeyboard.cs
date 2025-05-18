using UnityEngine;
using UnityEngine;

public class PlayerInputKeyboard : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController no asignado en PlayerInputKeyboard.");
            enabled = false;
        }
    }

    private void Update()
    {
        if (playerController == null) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput < 0) playerController.MoveLeft();
        else if (horizontalInput > 0) playerController.MoveRight();
        else playerController.StopMovement();

        if (Input.GetKeyDown(KeyCode.M)) playerController.SwitchLightMode();
    }
}
