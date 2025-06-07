using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Necesario para .ToList()

/// <summary>
/// Sistema de pool de objetos para reutilizar instancias de GameObjects
/// </summary>
public class ObjectPool
{
    // Configuración del pool
    private GameObject prefab;              // Prefab a instanciar
    private Queue<GameObject> pool = new Queue<GameObject>(); // Cola de objetos disponibles (inactivos)
    private List<GameObject> activeObjects = new List<GameObject>(); // <--- ¡NUEVO! Lista de objetos actualmente activos
    private Transform parent;               // Padre jerárquico para organización en escena
    private int maxSize;                    // Tamaño máximo del pool

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
        GameObject obj;
        // Prioriza objetos existentes en el pool (inactivos)
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        // Crea nuevo objeto si no hay disponibles y no se supera el máximo
        // La condición debe ser sobre el TOTAL de objetos (activos + inactivos)
        else if (activeObjects.Count + pool.Count < maxSize) // <--- Condición corregida
        {
            obj = CreateNewObject();
        }
        // Advertencia si se alcanza capacidad máxima
        else
        {
            Debug.LogWarning("Pool max size reached. No new objects created for " + prefab.name + ".");
            return null;
        }

        obj.SetActive(true);
        activeObjects.Add(obj); // <--- ¡Añadir a la lista de objetos activos!
        return obj;
    }

    /// <summary>
    /// Devuelve un objeto al pool para reutilización
    /// </summary>
    /// <param name="obj">Objeto a devolver</param>
    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return; // Asegurarse de que el objeto no es nulo

        // Opcional: Verifica si el objeto estaba en la lista de activos para evitar errores
        if (!activeObjects.Contains(obj))
        {
            Debug.LogWarning("Intentando devolver un objeto (" + obj.name + ") que no estaba en la lista de objetos activos del pool. Destruyendo.");
            Object.Destroy(obj); // Destruir si no fue 'tomado' por este pool
            return;
        }

        obj.SetActive(false);
        activeObjects.Remove(obj); // <--- ¡Eliminar de la lista de objetos activos!
        pool.Enqueue(obj);
    }

    /// <summary>
    /// Devuelve todos los objetos actualmente activos gestionados por este pool a la cola de inactivos.
    /// </summary>
    public void ReturnAllObjects()
    {
        // Crea una copia de la lista de objetos activos para poder iterar y modificar la original
        foreach (GameObject obj in activeObjects.ToList()) // <--- Necesita 'using System.Linq;'
        {
            if (obj != null) // Asegurarse de que el objeto no ha sido destruido por otra parte
            {
                ReturnObject(obj); // Esto mueve el objeto de 'activeObjects' a 'pool'
            }
        }
        Debug.Log($"ObjectPool: Devueltos todos los objetos activos para {prefab.name}.");
    }

    /// <summary>
    /// Destruye todos los objetos en el pool (activos e inactivos) y reinicia el pool.
    /// Útil para limpiar la memoria al cambiar de escena o al finalizar el juego.
    /// </summary>
    public void ClearPool()
    {
        // Primero, devuelve todos los objetos activos para que estén en la cola 'pool'
        ReturnAllObjects(); 

        // Luego, destruye todos los objetos que están en la cola de inactivos
        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }
        pool.Clear(); // Asegurarse de que la cola está vacía
        activeObjects.Clear(); // La lista de activos debería estar vacía en este punto
        Debug.Log($"ObjectPool: Limpiado completamente el pool para {prefab.name}.");
    }
}