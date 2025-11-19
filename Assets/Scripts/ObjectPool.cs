using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public GameObject collectiblePrefab;
    private Queue<GameObject> pool = new Queue<GameObject>();

    public GameObject GetGameObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);

            return obj;
        }

        return Instantiate(collectiblePrefab);
    }

    public void ReturnGameObject(GameObject obj)
    {
        obj.SetActive(false);

        pool.Enqueue(obj);
    }

    public void DestroyPool()
    {
        if (pool.Count > 0) pool.Clear();
    }
}
