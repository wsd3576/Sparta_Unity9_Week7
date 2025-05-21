using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    
    [Header("Object Info")]
    [SerializeField] private GameObject infoPanel;
    public GameObject InfoPanel
    {
        get{return infoPanel;}
    }
    [SerializeField] private TextMeshProUGUI objectNameText;
    [SerializeField] private TextMeshProUGUI objectDescriptionText;
    
    [Header("Conditions")]
    public ConditionUI _health;
    public ConditionUI _stamina;
    
    [Header("Inventory")]
    [SerializeField] private Transform inventoryPanel;
    public ItemSlot[] inventory;

    private void Awake()
    {
        GameManager.Instance.UIManager = this;
        inventory = new ItemSlot[inventoryPanel.childCount];
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = inventoryPanel.GetChild(i).GetComponent<ItemSlot>();
            inventory[i].index = i;
            inventory[i].uiManager = this;
            inventory[i].ClearSlot();
        }
        TogglePauseMenu();
        infoPanel.SetActive(false);
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
        }
        else
        {
            pauseMenu.SetActive(true);
        }
    }

    public void ShowObjectInfo(IInteractable interactable)
    {
        infoPanel.SetActive(true); 
        objectNameText.text = interactable.GetObjectInfo().Item1;
        objectDescriptionText.text = interactable.GetObjectInfo().Item2;
    }
    
    public void UpdateInventory(List<InventoryItem> items)
    {
        foreach (var item in items)
        {
            ItemSlot slot = GetCurItemSlot(item);
            if (slot != null)
            {
                slot.currentItem.quantity = item.quantity;
                continue;
            }
        
            ItemSlot emptySlot = GetEmptySlot();

            if (emptySlot != null)
            {
                emptySlot.currentItem = item;
                emptySlot.currentItem.quantity = item.quantity;
            }
        }
        
        foreach (ItemSlot slot in inventory)
        {
            if (slot.currentItem != null)
            {
                slot.SetSlot();
            }
            else
            {
                slot.ClearSlot();
            }
        }
    }
    
    ItemSlot GetCurItemSlot(InventoryItem data)
    {
        foreach (ItemSlot slot in inventory)
        {
            if (slot.currentItem == data)
            {
                return slot;
            }
        }
        return null;
    }
    
    ItemSlot GetEmptySlot()
    {
        foreach (ItemSlot slot in inventory)
        {
            if (slot.currentItem == null)
            {
                return slot;
            }
        }
        return null;
    }
}
