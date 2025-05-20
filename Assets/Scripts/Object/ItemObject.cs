using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public (string, string) GetObjectInfo()
    {
        string str1 = $"{data.itemName}";
        string str2 = $"{data.itemDescription}";
        return (str1, str2);
    }

    public void OnInteractInput()
    {
        GameManager.Instance.Player.AddItem(data.type, data.amount);
        Destroy(gameObject);
    }
}
