using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startButton;

    private void Start()
    {
        // Asegúrate de que el botón esté asignado
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
    }

    void StartGame()
    {
        // Cargar la escena del juego (asegúrate de que el nombre de la escena sea correcto)
        SceneManager.LoadScene("GameScene"); // Cambia "GameScene" por el nombre de tu escena de juego
    }
}