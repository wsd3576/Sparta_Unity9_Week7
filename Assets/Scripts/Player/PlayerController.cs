using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float jumpForce;
    private Vector2 curMovementInput;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float sprintStamina;
    [SerializeField] private float jumpStamina;
    private bool isSprinting = false;

    [Header("Look")]
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;
    [SerializeField] private float camCurXRot;
    [SerializeField] private float lookSensitivity;
    [SerializeField] private Vector2 mouseDelta;
    [SerializeField] private bool canLook = true;
    //3인칭 전환
    [SerializeField] private Transform fpsCameraTarget;
    [SerializeField] private Transform tpsCameraTarget;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask thirdPersonCullingMask;
    [SerializeField] private LayerMask firstPersonCullingMask;
    
    private float smoothSpeed = 10f;
    private bool isThirdPerson = false;
    
    private Transform currentCameraTarget;
    private Vector3 camVelocity;
    
    [Header("Items")]
    [SerializeField] private ItemData curItemData;
    [SerializeField] private Transform dropPosition;
    [SerializeField] private Transform objectPool;
    public List<InventoryItem> items = new List<InventoryItem>();
    private int maxItemCount;
    
    private float bonusSpeed = 0f;
    private float bonusJumpForce = 0f;
    
    private Coroutine speedUpCoroutine;
    private Coroutine jumpBoostCoroutine;
    
    private Rigidbody _rigidbody;
    
    private readonly Dictionary<string, int> itemButtonMap = new()
    {
        { "1", 0 },
        { "2", 1 },
        { "3", 2 },
        { "4", 3 }
    };
    
    private void Awake()
    {
        cameraContainer = GetComponentInChildren<Camera>().transform.parent.transform;
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        maxItemCount = GameManager.Instance.UIManager.inventory.Length;
        currentCameraTarget = fpsCameraTarget;
    }

    private void Update()
    {
        if (isSprinting)
        {
            GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina).Subtract(Time.deltaTime * sprintStamina);
            
            if (GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina).curValue <= 0)
            {
                isSprinting = false;
            }
        }
    }

    private void FixedUpdate()
    {
        cameraContainer.position = Vector3.Lerp
        (
            cameraContainer.position,
            currentCameraTarget.position,
            Time.deltaTime * smoothSpeed
        );

        cameraContainer.rotation = Quaternion.Slerp
        (
            cameraContainer.rotation,
            currentCameraTarget.rotation,
            Time.deltaTime * smoothSpeed
        );
        
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= ((moveSpeed + bonusSpeed) * (isSprinting ? sprintSpeed : 1f));
        dir.y = _rigidbody.velocity.y;
        
        _rigidbody.velocity = dir;
    }

    private void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity; //Y방향 회전은 X축을 회전하는 것이기 때문(마우스의 Y축 회전 => 오브젝트의 X방향 회전)
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0); //+방향 회전은 아래를 향하는 것이기 때문에 마우스 아래로 내림 = -값 이기 때문에
        
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); //X방향 회전은 Y축을 회전하는 것이기 때문(마우스의 X축 회전 => 오브젝트의 Y방향 회전)
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase >= InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina).curValue <= 0)
        {
            isSprinting = false;
            return;
        }

        if (context.phase == InputActionPhase.Started)
        {
            isSprinting = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isSprinting = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina).curValue <= jumpStamina) return;
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina).Subtract(jumpStamina);
            _rigidbody.AddForce(Vector2.up * (jumpForce + bonusJumpForce), ForceMode.Impulse);
        }
    }
    
    bool IsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, 0.2f, groundLayerMask)) return true;
        
        return false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            bool toggle = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
            canLook = !toggle;
            GameManager.Instance.PauseGame();
        }
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            GameManager.Instance.Player.interaction.OnInteractInput();
        }
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            var control = context.control;

            string keyName = control.name.ToLower();

            if (itemButtonMap.TryGetValue(keyName, out int index))
            {
                UseItem(index);
            }
        }
    }
    
    public void AddItem(ItemData item, int amount)
    {
        curItemData = item;
        
        if(items.Exists(i => i.itemData == item))
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
        Instantiate(curItemData.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360), objectPool);
    }

    private void UseItem(int index)
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
                        GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina).Add(con.value);
                        break;
                    case ConsumableType.Health:
                        GameManager.Instance.Player.condition.GetCondition(ConditionType.Health).Add(con.value);
                        break;
                    case ConsumableType.SpeedUp:
                    case ConsumableType.JumpBoost:
                        ItemBoost(con); 
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

    private void ItemBoost(ItemDataConsumable item)
    {
        switch (item.type)
        {
            case ConsumableType.SpeedUp:
                if (speedUpCoroutine != null)
                {
                    StopCoroutine(speedUpCoroutine);
                }
                bonusSpeed = item.value;
                speedUpCoroutine = StartCoroutine(HandleSpeedUp(item.duration));
                break;

            case ConsumableType.JumpBoost:
                if (jumpBoostCoroutine != null)
                {
                    StopCoroutine(jumpBoostCoroutine);
                }
                bonusJumpForce = item.value;
                jumpBoostCoroutine = StartCoroutine(HandleJumpBoost(item.duration));
                break;
            
        }
    }

    private IEnumerator HandleSpeedUp(float duration)
    {
        yield return new WaitForSeconds(duration);
        bonusSpeed = 0f;
    }

    private IEnumerator HandleJumpBoost(float duration)
    {
        yield return new WaitForSeconds(duration);
        bonusJumpForce = 0f;
    }

    public void OnToggleView(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isThirdPerson = !isThirdPerson;
            currentCameraTarget = isThirdPerson ? tpsCameraTarget : fpsCameraTarget;
            mainCamera.cullingMask = isThirdPerson ? thirdPersonCullingMask : firstPersonCullingMask;
        }
    }
}
