using System.Collections.Generic;
using UnityEngine;

public class ItemObjectPool : MonoBehaviour
{
    private ItemObject[] itemPrefabs;
    private List<GameObject> pool = new();

    private void Start()
    {
        //ItemObejct가 있는 오브젝트만 받아오고 순서 부여
        itemPrefabs = GetComponentsInChildren<ItemObject>();
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            itemPrefabs[i].index = i;
        }
    }

    
    public GameObject Get(ItemObject obj) //오브젝트 요청. 현재는 상호작용했을때 인벤토리가 다 차있어서 다시 뱉어낼 때만 호출
    {
        //비활성화된 오브젝트들은 리스트에 저장되니 해당 아이템을 찾는다.
        GameObject foundObj = pool.Find
        (pooledObj =>
            pooledObj.TryGetComponent(out ItemObject item) &&
            item.index == obj.index
        );
        //찾았다면 해당 오브젝트를 활성화 하고 반환
        foundObj.SetActive(true);
        return foundObj;
    }
    

    public void Return(GameObject obj) //오브젝트 반환. 현재는 상호작용시 아이템을 비활성화 하는 기능밖에 없음.
    {
        obj.SetActive(false);
        if (!pool.Contains(obj))
            pool.Add(obj);
    }
}
