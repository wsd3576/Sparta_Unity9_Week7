using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData data;
    [SerializeField] private int amount;

    public (string, string) GetObjectInfo()
    {
        string str1 = $"{data.itemName}";
        string str2 = $"{data.itemDescription}";
        return (str1, str2);
    }

    public void OnInteractInput()
    {
        InventoryItemData item = new InventoryItemData(data.type, data.amount, data.value);
        GameManager.Instance.Player.controller.AddItem(item, amount);
        Destroy(gameObject);
    }
}
