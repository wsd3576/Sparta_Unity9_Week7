using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _moveSpeed;
    [SerializeField] float _sprintSpeed;
    [SerializeField] float _jumpForce;
    [SerializeField] Vector2 _curMovementInput;
    [SerializeField] LayerMask _groundLayerMask;
    [SerializeField] private float _sprintStamina;
    [SerializeField] private float _jumpStamina;
    bool _isSprinting = false;

    [Header("Look")]
    [SerializeField] Transform _cameraContainer;
    [SerializeField] float _minXLook;
    [SerializeField] float _maxXLook;
    [SerializeField] float _camCurXRot;
    [SerializeField] float _lookSensitivity;
    [SerializeField] Vector2 _mouseDelta;
    [SerializeField] bool _canLook = true;
    
    [Header("Items")]
    [SerializeField] private ItemData _curItemData;
    [SerializeField] private Transform _dropPosition;
    [SerializeField] private Transform _objectPool;
    [SerializeField] private float _itemDuration;
    public List<InventoryItem> items = new List<InventoryItem>();
    private int _maxItemCount;
    
    private float _bonusSpeed = 0f;
    private float _bonusJumpForce = 0f;
    
    private Coroutine _speedUpCoroutine;
    private Coroutine _jumpBoostCoroutine;
    
    private Rigidbody _rigidbody;
    
    private readonly Dictionary<string, int> _itemButtonMap = new()
    {
        { "1", 0 },
        { "2", 1 },
        { "3", 2 },
        { "4", 3 }
    };
    
    private void Awake()
    {
        _cameraContainer = GetComponentInChildren<Camera>().transform.parent.transform;
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _maxItemCount = GameManager.Instance.UIManager.inventory.Length;
    }

    private void Update()
    {
        if (_isSprinting)
        {
            GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina).Subtract(Time.deltaTime * _sprintStamina);
            
            if (GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina).curValue <= 0)
            {
                _isSprinting = false;
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (_canLook) CameraLook();
    }

    private void Move()
    {
        Vector3 dir = transform.forward * _curMovementInput.y + transform.right * _curMovementInput.x;
        dir *= ((_moveSpeed + _bonusSpeed) * (_isSprinting ? _sprintSpeed : 1f));
        dir.y = _rigidbody.velocity.y;
        
        _rigidbody.velocity = dir;
    }

    private void CameraLook()
    {
        _camCurXRot += _mouseDelta.y * _lookSensitivity; //Y방향 회전은 X축을 회전하는 것이기 때문(마우스의 Y축 회전 => 오브젝트의 X방향 회전)
        _camCurXRot = Mathf.Clamp(_camCurXRot, _minXLook, _maxXLook);
        _cameraContainer.localEulerAngles = new Vector3(-_camCurXRot, 0, 0); //+방향 회전은 아래를 향하는 것이기 때문에 마우스 아래로 내림 = -값 이기 때문에
        
        transform.eulerAngles += new Vector3(0, _mouseDelta.x * _lookSensitivity, 0); //X방향 회전은 Y축을 회전하는 것이기 때문(마우스의 X축 회전 => 오브젝트의 Y방향 회전)
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase >= InputActionPhase.Canceled)
        {
            _curMovementInput = Vector2.zero;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina).curValue <= 0)
        {
            _isSprinting = false;
            return;
        }

        if (context.phase == InputActionPhase.Started)
        {
            _isSprinting = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isSprinting = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina).curValue <= _jumpStamina) return;
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina).Subtract(_jumpStamina);
            _rigidbody.AddForce(Vector2.up * (_jumpForce + _bonusJumpForce), ForceMode.Impulse);
        }
    }
    
    bool IsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, 0.2f, _groundLayerMask)) return true;
        
        return false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            bool toggle = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
            _canLook = !toggle;
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

    public void UseItem(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            var control = context.control;

            string keyName = control.name.ToLower();

            if (_itemButtonMap.TryGetValue(keyName, out int index))
            {
                UseItem(index);
            }
        }
    }
    
    public void AddItem(ItemData item, int amount)
    {
        _curItemData = item;
        
        if(items.Exists(i => i.itemData == item))
        {
            InventoryItem existingItem = items.Find(i => i.itemData == item);
            existingItem.quantity += amount;
        }
        else
        {
            if (items.Count == _maxItemCount)
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
        Instantiate(_curItemData.dropPrefab, _dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360), _objectPool);
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
                if (_speedUpCoroutine != null)
                {
                    StopCoroutine(_speedUpCoroutine);
                }
                _bonusSpeed = item.value;
                _speedUpCoroutine = StartCoroutine(HandleSpeedUp());
                break;

            case ConsumableType.JumpBoost:
                if (_jumpBoostCoroutine != null)
                {
                    StopCoroutine(_jumpBoostCoroutine);
                }
                _bonusJumpForce = item.value;
                _jumpBoostCoroutine = StartCoroutine(HandleJumpBoost());
                break;
            
        }
    }

    private IEnumerator HandleSpeedUp()
    {
        yield return new WaitForSeconds(_itemDuration);
        _bonusSpeed = 0f;
    }

    private IEnumerator HandleJumpBoost()
    {
        yield return new WaitForSeconds(_itemDuration);
        _bonusJumpForce = 0f;
    }
}
