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
    [SerializeField] private ConditionUI _health;
    [SerializeField] private ConditionUI _stamina;
    public DamageEffect damageEffect;
    
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
        //인벤토리에 아이템들을 넣고 상태 업데이트.(슬롯이 부족해도 가능 슬롯이 넘쳐나도 가능)
        for (int i = 0; i < inventory.Length; i++)
        {
            if (i < items.Count)
            {
                inventory[i].currentItem = items[i];
                inventory[i].SetSlot();
            }
            else
            {
                inventory[i].currentItem = null;
                inventory[i].ClearSlot();
            }
        }
    }
}
