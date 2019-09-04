using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PoolManager
{
    //private static PoolManager instance = null;
    //public static PoolManager GetInstance() { return instance; }


    public PoolCategory[] poolsCategory;

    public void Initialize()
    {
        int length = poolsCategory.Length;
        for (int i = 0; i < length; i++)
        {
            poolsCategory[i].InitializePools();
        }
    }

    /// <summary>
    /// Spawn from the specified pool inside the specified category
    /// </summary>
    public GameObject Spawn(string categoryName, string poolTag, Vector3 position, Quaternion rotation)
    {
        PoolCategory poolCategory = Array.Find(poolsCategory, category => category.name == categoryName);
        if(poolCategory != null)
        {
            return poolCategory.SpawnFromPool(poolTag, position, rotation);
        }
        return null;
    }

    /// <summary>
    /// Spawn from a random Pool inside the specified category based on Pools spawn probability
    /// </summary>
    public GameObject Spawn(string categoryName, Vector3 position, Quaternion rotation)
    {
        PoolCategory poolCategory = Array.Find(poolsCategory, category => category.name == categoryName);
        if (poolCategory != null)
        {
            return poolCategory.SpawnFromPool(null, position, rotation);
        }
        return null;
    }

    public string GetRandomCategoryPoolTag(string categoryName)
    {
        PoolCategory poolCategory = Array.Find(poolsCategory, category => category.name == categoryName);
        if (poolCategory != null)
        {
            return poolCategory.GetRandomPoolTag();
        }
        return null;
    }

    public void DeactivateObject(GameObject objectToDeactivate)
    {
        objectToDeactivate.SetActive(false);
    }
}
