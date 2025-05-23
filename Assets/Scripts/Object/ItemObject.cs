using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable //인벤토리에 습득 가능한 오브젝트
{
    public ItemObjectPool pool;
    public ItemData data;
    public int amount;
    public int index;

    public (string, string, string) GetObjectInfo() //PlayerInteraction에서 IInteractable 감지시 UI매니저로 호출하는 정보 반환 함수
    {
        string str1 = $"{data.itemName}";
        string str2 = $"{data.itemDescription}";
        string str3 = $"x{amount}";
        return (str1, str2, str3);
    }

    public void OnInteractInput() //PlayerInteraction에서 상호작용시 호출되는 함수
    {
        pool = FindObjectOfType<ItemObjectPool>();
        if (pool != null)
        {
            pool.Return(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        GameManager.Instance.Player.playerInteraction.AddItem(gameObject, amount);
    }
}
