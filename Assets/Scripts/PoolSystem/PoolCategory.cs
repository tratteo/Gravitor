using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolCategory
{
    public string name;
    public Pool[] pools;
    public Dictionary<string, Queue<GameObject>> poolsDictionary;


    public void InitializePools(Vector3 position)
    {
        poolsDictionary = new Dictionary<string, Queue<GameObject>>();
        int length = pools.Length;
        for (int i = 0; i < length; i++)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            int poolDim = pools[i].poolSize;
            for (int j = 0; j < poolDim; j++)
            {
                GameObject obj = GameObject.Instantiate(pools[i].prefab, position, Quaternion.identity);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolsDictionary.Add(pools[i].tag, objectPool);
        }
        NormalizeSpawnProbabilities();
    }

    public GameObject SpawnFromPool(string poolTag, Vector3 position, Quaternion rotation)
    {
        if (poolTag == null)
        {
            poolTag = GetRandomPoolTag();
        }
        if (poolsDictionary[poolTag] == null)
        {
            return null;
        }
        if (!poolsDictionary.ContainsKey(poolTag))
        {
            return null;
        }

        IPooledObject[] poolInterfaces;
        GameObject objectToSpawn = poolsDictionary[poolTag].Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolInterfaces = objectToSpawn.GetComponentsInChildren<IPooledObject>();
        objectToSpawn.SetActive(true);
        if (poolInterfaces != null)
        {
            int length = poolInterfaces.Length;
            for (int i = 0; i < length; i++)
            {
                poolInterfaces[i].OnObjectSpawn();
            }
        }
        poolsDictionary[poolTag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

    public string GetRandomPoolTag()
    {
        int index = -1;
        float radix = UnityEngine.Random.Range(0f, 1f);
        int count = pools.Length;
        while (radix > 0 && index < count)
        {
            radix -= pools[++index].spawnProbability;
        }
        return pools[index].tag;
    }

    private void NormalizeSpawnProbabilities()
    {
        float total = 0f;
        int length = pools.Length;
        for (int i = 0; i < length; i++)
        {
            total += pools[i].spawnProbability;
        }
        for (int i = 0; i < length; i++)
        {
            pools[i].spawnProbability /= total;
        }
    }
}

