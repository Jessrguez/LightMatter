using UnityEngine;

public class EmissionMaterialSetter : MonoBehaviour
{
    [Tooltip("Color de emisión del material.")]
    [SerializeField] private Color emissionColor = Color.white;

    [Tooltip("Multiplicador de la intensidad de la emisión.")]
    [SerializeField, Range(1f, 10f)] private float emissionIntensityMultiplier = 5f;

    private Material material;
    private Renderer targetRenderer;

    private void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer == null)
        {
            Debug.LogWarning($"No se encontró Renderer en {gameObject.name}. Deshabilitando script.");
            enabled = false;
            return;
        }

        material = new Material(targetRenderer.material);
        targetRenderer.material = material;

        material.EnableKeyword("_EMISSION");
        SetEmissionColor(emissionColor);
    }

    public void SetEmissionColor(Color color)
    {
        Color finalColor = color * emissionIntensityMultiplier;
        if (material != null)
        {
            material.SetColor("_EmissionColor", finalColor);
            targetRenderer?.SetPropertyBlock(null);
        }
    }
}
