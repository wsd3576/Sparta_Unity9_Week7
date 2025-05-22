using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ItemObjectPool : MonoBehaviour
{
    [SerializeField] private ItemObject[] itemPrefabs;
    private List<GameObject> pool = new List<GameObject>();

    private void Start()
    {
        ItemObject[] foundItems = GetComponentsInChildren<ItemObject>();
        
        itemPrefabs = new ItemObject[foundItems.Length];
        for (int i = 0; i < foundItems.Length; i++)
        {
            itemPrefabs[i] = foundItems[i];
            itemPrefabs[i].index = i;
        }
    }

    
    public GameObject Get(ItemObject obj)
    {
        GameObject foundObj = pool.Find(pooledObj =>
            !pooledObj.activeInHierarchy && pooledObj.GetComponent<ItemObject>().index == obj.index);
        
        if (foundObj != null)
        {
            foundObj.SetActive(true);
            return foundObj;
        }
        
        GameObject newObj = Instantiate(itemPrefabs[obj.index].data.dropPrefab, transform);
        newObj.GetComponent<ItemObject>().index = obj.index;
        return newObj;
    }
    

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        if (!pool.Contains(obj))
            pool.Add(obj);
    }
}
