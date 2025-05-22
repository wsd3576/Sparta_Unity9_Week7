using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerCondition playerCondition;
    private PlayerController playerController;
    private new Camera camera;
    
    [Header("Setting")]
    [SerializeField] private float maxCheckDistance = 5f;
    [SerializeField] private float rayOriginOffsetY = 1.7f;
    [SerializeField] LayerMask layerMask;

    private float checkRate = 0.05f;
    private float lastCheckTime;
    
    private Vector3 rayOrigin;
    private Vector3 rayDirection;

    private GameObject curInteractGameObject;
    private IInteractable curInteractable;

    [Header("Items")]
    [SerializeField] private ItemData curItemData;
    [SerializeField] private Transform dropPosition;
    [SerializeField] private Transform objectPool;
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();
    [SerializeField] private int maxItemCount = 4;
    
    
    private void Start()
    {
        camera = Camera.main;
        playerController = GetComponent<PlayerController>();
        playerCondition = GameManager.Instance.Player.playerCondition;
    }
    
    private void Update()
    {
        if (Time.time - lastCheckTime < checkRate) return;
        
        lastCheckTime = Time.time;
        
        rayOrigin = gameObject.transform.position + (Vector3.up * rayOriginOffsetY);
        rayDirection = camera.transform.forward;

        Ray ray = new Ray(rayOrigin, rayDirection);
        
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
        {
            if (hit.collider.gameObject != curInteractGameObject)
            {
                curInteractGameObject = hit.collider.gameObject;
                curInteractable = hit.collider.GetComponent<IInteractable>();
                if (curInteractable != null) GameManager.Instance.UIManager.ShowObjectInfo(curInteractable);
            }
        }
        else
        {
            curInteractGameObject = null;
            curInteractable = null;
            GameManager.Instance.UIManager.InfoPanel.SetActive(false);
        }
    }

    public void OnInteractInput()
    {
        if (curInteractable != null)
        {
            curInteractable.OnInteractInput();
            curInteractGameObject = null;
            curInteractable = null;
            GameManager.Instance.UIManager.InfoPanel.SetActive(false);
        }
    }

    public void AddItem(ItemData item, int amount)
    {
        curItemData = item;

        if (items.Exists(i => i.itemData == item))
        {
            InventoryItem existingItem = items.Find(i => i.itemData == item);
            existingItem.quantity += amount;
        }
        else
        {
            if (items.Count == maxItemCount)
            {
                DropItem();
            }
            else
            {
                items.Add(new InventoryItem(item, amount));
            }
        }

        GameManager.Instance.UIManager.UpdateInventory(items);
    }

    private void DropItem()
    {
        Instantiate
            (
                curItemData.dropPrefab, 
                dropPosition.position, 
                Quaternion.Euler(Vector3.one * Random.value * 360), 
                objectPool
            );
    }

    public void UseItem(int index)
    {
        if (index >= items.Count) return;
        if (items[index].itemData.type == ItemType.Consumable)
        {
            items[index].quantity--;

            foreach (var con in items[index].itemData.consumables)
            {
                switch (con.type)
                {
                    case ConsumableType.Stamina:
                        playerCondition.GetCondition(ConditionType.Stamina).Add(con.value);
                        break;
                    case ConsumableType.Health:
                        playerCondition.GetCondition(ConditionType.Health).Add(con.value);
                        break;
                    case ConsumableType.SpeedUp:
                    case ConsumableType.JumpBoost:
                        playerController.ItemBoost(con);
                        break;
                }
            }

            if (items[index].quantity <= 0)
            {
                items.Remove(items[index]);
            }
        }

        GameManager.Instance.UIManager.UpdateInventory(items);
    }
}