using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public PlayerCondition condition;
    public PlayerInteraction interaction;
    
    private void Awake()
    {
        GameManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        interaction = GetComponent<PlayerInteraction>();
    }

    public void AddItem(ConsumableType type, int amount)
    {
        switch (type)
        {
            case ConsumableType.Stamina:
                break;
            case ConsumableType.SpeedUp:
                break;
            case ConsumableType.JumpBoost:
                break;
            case ConsumableType.Health:
                break;
            default:
                break;
        }
    }
}
