using System;
using UnityEngine;

public enum ConsumableType
{
    Stamina,
    SpeedUp,
    JumpBoost,
    Health
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
    public int amount;
}