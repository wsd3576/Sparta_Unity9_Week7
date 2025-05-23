using System;
using UnityEngine;

public enum ItemType //다른 아이템 종류 추가시 사용할 열거형
{
    Consumable
}

public enum ConsumableType //소모품의 종류 열거형
{
    Stamina,
    SpeedUp,
    JumpBoost,
    Health
}

[Serializable]
public class InventoryItem //인벤토리에 저장용
{
    public ItemData itemData;
    public int quantity;

    public InventoryItem(ItemData itemData, int quantity)
    {
        this.itemData = itemData;
        this.quantity = quantity;
    }
}

[Serializable]
public class ItemDataConsumable //각 아이템별 추가할 속성값
{
    public ConsumableType type;
    public float value;
    public float duration;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject //아이템의 정보
{
    [Header("Info")]
    public string itemName;
    public string itemDescription;
    public ItemType type;
    public Sprite icon;
    
    public GameObject dropPrefab;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;
}