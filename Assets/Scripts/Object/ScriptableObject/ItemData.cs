using System;
using UnityEngine;

public enum ItemType
{
    Equipable,
    Consumable
}

public enum ConsumableType
{
    Stamina,
    SpeedUp,
    JumpBoost,
    Health
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public string itemDescription;
    public ItemType type;
    public Sprite icon;
    public int amount;
    
    [Header("Consumable")]
    public ItemDataConsumable[] consumables;
    
    [Header("Equip")]
    public GameObject equipPrefab;
}