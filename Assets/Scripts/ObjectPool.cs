using UnityEngine;
using System.Collections.Generic;

public class ObjectPool
{
    private GameObject prefab;
    private Queue<GameObject> pool;
    private int maxPoolSize;
    private Transform poolParent;

    public ObjectPool(GameObject prefab, int initialSize, int maxSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.maxPoolSize = maxSize;
        this.poolParent = parent;
        pool = new Queue<GameObject>();

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateNewObject();
            pool.Enqueue(obj);
        }
    }

    private GameObject CreateNewObject()
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab no asignado en el ObjectPool");
            return null;
        }

        GameObject obj = GameObject.Instantiate(prefab);
        if (poolParent != null)
            obj.transform.SetParent(poolParent);

        obj.SetActive(false);
        return obj;
    }

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        return (pool.Count < maxPoolSize) ? CreateNewObject() : null;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
