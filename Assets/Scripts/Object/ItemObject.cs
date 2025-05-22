using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemObjectPool pool;
    public ItemData data;
    public int amount;
    public int index;

    public (string, string, int) GetObjectInfo()
    {
        string str1 = $"{data.itemName}";
        string str2 = $"{data.itemDescription}";
        return (str1, str2, amount);
    }

    public void OnInteractInput()
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
