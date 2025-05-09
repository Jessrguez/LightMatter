using UnityEngine;

public class EmissionMaterialSetter : MonoBehaviour
{
    public Color emissionColor = Color.white; // Color de emisión

    private Material material;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("No Renderer found on this GameObject");
            return;
        }

        material = renderer.material;  // Usamos el material del renderer
        material.EnableKeyword("_EMISSION");
        SetEmissionColor(emissionColor);
    }

    public void SetEmissionColor(Color color)
    {
        Color finalEmissionColor = color * 5.0f;  // Aumentamos la intensidad de la emisión
        material.SetColor("_EmissionColor", finalEmissionColor);
    }
}
