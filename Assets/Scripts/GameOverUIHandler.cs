using UnityEngine;
using UnityEngine.UI;          // Necesario para usar la clase 'Button'
using UnityEngine.SceneManagement; // Necesario si usas SceneManager en los fallbacks

public class GameOverUIHandler : MonoBehaviour
{
    // Declara la variable pública para tu botón de Reiniciar.
    // Esta será visible en el Inspector para que puedas asignarla.
    [Header("Botones de UI de Game Over")]
    public Button restartGameButton; // Asigna tu botón "Reiniciar" aquí

    void Start()
    {
        // Nos aseguramos de que el botón de Reiniciar esté asignado en el Inspector.
        if (restartGameButton != null)
        {
            // Conecta el evento de clic del botón al método 'OnRestartGameClicked'.
            restartGameButton.onClick.AddListener(OnRestartGameClicked);
            Debug.Log("GameOverUIHandler: Listener añadido para el botón de Reiniciar Juego.");
        }
        else
        {
            Debug.LogWarning("GameOverUIHandler: ¡Advertencia! 'restartGameButton' no está asignado en el Inspector. El botón de Reiniciar no funcionará.");
        }

        // --- ¡ELIMINADAS LAS REFERENCIAS A returnToMainMenuButton Y OnReturnToMainMenuClicked! ---
        // Ya que solo tienes un botón de reiniciar.
    }

    void OnDestroy()
    {
        // Remueve el listener del botón de Reiniciar cuando este script se destruye.
        if (restartGameButton != null)
        {
            restartGameButton.onClick.RemoveListener(OnRestartGameClicked);
        }
    }

    // Este es el método que se ejecutará cuando el botón 'restartGameButton' sea clickeado.
    private void OnRestartGameClicked()
    {
        Debug.Log("GameOverUIHandler: Botón 'Reiniciar Juego' clickeado.");
        // Verificamos si la instancia del GameManager existe.
        if (GameManager.Instance != null)
        {
            // Llamamos al método 'StartNewGame' de tu GameManager,
            // que debería resetear las vidas, puntuación y cargar la escena de juego.
            GameManager.Instance.StartNewGame();
        }
        else
        {
            Debug.LogError("GameOverUIHandler: ¡ERROR! GameManager.Instance es NULO. No se pudo reiniciar el juego correctamente. Cargando GameScene directamente.");
            SceneManager.LoadScene("GameScene"); // Asegúrate de que "GameScene" es el nombre correcto de tu escena de juego
        }
    }

}