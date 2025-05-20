using System;
using UnityEngine;

public enum ConsumableType
{
    Health,
    Stamina,
    SpeedUp,
    JumpBoost
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
    public Sprite icon;
    public GameObject dropPrefab;
    
    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;
    
    [Header("Consumable")]
    public ItemDataConsumable[] consumables;
}