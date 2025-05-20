using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema de pool de objetos para reutilizar instancias de GameObjects
/// </summary>
public class ObjectPool
{
    // Configuración del pool
    private GameObject prefab;           // Prefab a instanciar
    private Queue<GameObject> pool = new Queue<GameObject>(); // Cola de objetos disponibles
    private Transform parent;            // Padre jerárquico para organización en escena
    private int maxSize;                 // Tamaño máximo del pool

    /// <summary>
    /// Crea un nuevo pool de objetos
    /// </summary>
    /// <param name="prefab">Prefab a clonar</param>
    /// <param name="initialSize">Cantidad inicial de objetos</param>
    /// <param name="maxSize">Límite máximo de objetos</param>
    /// <param name="parent">Transform padre para organización</param>
    public ObjectPool(GameObject prefab, int initialSize, int maxSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.maxSize = maxSize;

        // Población inicial del pool
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateNewObject();
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Instancia un nuevo objeto y lo configura para el pool
    /// </summary>
    private GameObject CreateNewObject()
    {
        GameObject obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false); // Objeto inactivo por defecto
        return obj;
    }

    /// <summary>
    /// Obtiene un objeto disponible del pool
    /// </summary>
    /// <returns>Objeto activado o null si se alcanzó el máximo</returns>
    public GameObject GetObject()
    {
        // Prioriza objetos existentes en el pool
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        // Crea nuevo objeto si no hay disponibles pero no se supera el máximo
        else if (pool.Count + 1 <= maxSize)
        {
            return CreateNewObject();
        }
        // Advertencia si se alcanza capacidad máxima
        else
        {
            Debug.LogWarning("Pool max size reached. No new objects created.");
            return null;
        }
    }

    /// <summary>
    /// Devuelve un objeto al pool para reutilización
    /// </summary>
    /// <param name="obj">Objeto a devolver</param>
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}