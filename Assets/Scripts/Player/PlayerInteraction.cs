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
    [SerializeField] private ItemObjectPool itemObjectPool;
    
    [SerializeField] private Transform dropPosition;
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

    public void AddItem(GameObject item, int amount)
    {
        //얻으려는 아이템 데이터를 받아온다.
        ItemObject curItemObject = item.GetComponent<ItemObject>();
        ItemData curItemData = curItemObject.data;

        //기존 아이템과 일치하는 아이템이면 습득
        if (items.Exists(i => i.itemData == curItemData))
        {
            InventoryItem existingItem = items.Find(i => i.itemData == curItemData);
            existingItem.quantity += amount;
        }
        else
        {
            //기존 아이템과 일치하지 않을때
            if (items.Count == maxItemCount) //인벤토리가 가득 찼다면 버린다.
            {
                Debug.Log("Dropped");
                DropItem(curItemObject);
            }
            else //아니면 새로 추가한다.
            {
                items.Add(new InventoryItem(curItemData, amount));
            }
        }

        GameManager.Instance.UIManager.UpdateInventory(items);
    }

    private void DropItem(ItemObject dropItem)
    {
        //기존의 비활성화 시킨 아이템을 오브젝트 풀에서 받아온다.
        GameObject obj = itemObjectPool.Get(dropItem);
        //버린다.
        obj.transform.position = dropPosition.position;
        obj.transform.rotation = Quaternion.Euler(Vector3.one * Random.value * 360);
    }

    public void UseItem(int index)
    {
        if (index >= items.Count) return;
        Debug.Log($"{index}");
        if (items[index].itemData.type == ItemType.Consumable)
        {
            items[index].quantity--;
            
            foreach (var con in items[index].itemData.consumables)
            {
                //단순 회복은 이곳에서 처리.
                switch (con.type)
                {
                    case ConsumableType.Stamina:
                        playerCondition.GetCondition(ConditionType.Stamina).Add(con.value);
                        break;
                    case ConsumableType.Health:
                        playerCondition.GetCondition(ConditionType.Health).Add(con.value);
                        break;
                    //속도 및 점프 증가는 PlayerController에서 처리
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