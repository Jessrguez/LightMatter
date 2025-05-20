using UnityEngine;

public class EmissionMaterialSetter : MonoBehaviour
{
    // Configuración del color de emisión desde el Inspector
    [Tooltip("Color de emisión del material.")]
    [SerializeField] private Color emissionColor = Color.white;

    // Control de intensidad de la emisión (rango ajustable en el Inspector)
    [Tooltip("Multiplicador de la intensidad de la emisión.")]
    [SerializeField, Range(1f, 10f)] private float emissionIntensityMultiplier = 5f;

    // Referencias a componentes
    private Material material;          // Material instanciado para modificar emisión
    private Renderer targetRenderer;    // Renderer del objeto

    private void Start()
    {
        // Obtener el componente Renderer del objeto
        targetRenderer = GetComponent<Renderer>();
        
        // Verificar si existe el Renderer, deshabilitar script si no se encuentra
        if (targetRenderer == null)
        {
            Debug.LogWarning($"No se encontró Renderer en {gameObject.name}. Deshabilitando script.");
            enabled = false;
            return;
        }

        // Crear una nueva instancia del material para modificarlo sin afectar el original
        material = new Material(targetRenderer.material);
        targetRenderer.material = material;

        // Habilitar emisión en el material y establecer el color inicial
        material.EnableKeyword("_EMISSION");
        SetEmissionColor(emissionColor);
    }

    /// Establece el color de emisión del material, aplicando el multiplicador de intensidad
    public void SetEmissionColor(Color color)
    {
        // Calcular color final aplicando el multiplicador de intensidad
        Color finalColor = color * emissionIntensityMultiplier;
        
        // Aplicar el color al material si existe
        if (material != null)
        {
            material.SetColor("_EmissionColor", finalColor);
            targetRenderer?.SetPropertyBlock(null); // Limpiar PropertyBlock para asegurar que se apliquen los cambios
        }
    }
}
