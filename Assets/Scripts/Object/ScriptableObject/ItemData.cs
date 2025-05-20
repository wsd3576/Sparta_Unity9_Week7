using System;
using UnityEngine;

public enum ConsumableType
{
    Stamina,
    SpeedUp,
    JumpBoost,
    Health
}

[Serializable]
public class InventoryItemData
{
    public ConsumableType type;
    public int amount;
    public float value;

    public InventoryItemData(ConsumableType type, int amount, float value)
    {
        this.type = type;
        this.amount = amount;
        this.value = value;
    }
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public string itemDescription;
    public Sprite icon;
    
    [Header("Consumable")]
    public ConsumableType type;
    public float value;
    public int amount;
}