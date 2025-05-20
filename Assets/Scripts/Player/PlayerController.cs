using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _moveSpeed;
    [SerializeField] float _jumpForce;
    [SerializeField] Vector2 _curMovementInput;
    [SerializeField] LayerMask _groundLayerMask;

    [Header("Look")]
    [SerializeField] Transform _cameraContainer;
    [SerializeField] float _minXLook;
    [SerializeField] float _maxXLook;
    [SerializeField] float _camCurXRot;
    [SerializeField] float _lookSensitivity;
    [SerializeField] Vector2 _mouseDelta;
    [SerializeField] bool _canLook = true;
    
    [Header("Items")]
    [SerializeField] private float _itemDuration;
    [SerializeField] private List<InventoryItemData> items = new List<InventoryItemData>();
    
    private float _bonusSpeed = 0f;
    private float _bonusJumpForce = 0f;
    
    private bool _isSpeedUp = false;
    private bool _isJumpBoost = false;
    
    private Rigidbody _rigidbody;
    
    private Dictionary<string, int> itemButtonMap = new Dictionary<string, int>()
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
        dir *= _moveSpeed + _bonusSpeed;
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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * (_jumpForce + _bonusJumpForce), ForceMode.Impulse);
        }
    }
    
    bool IsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
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
        var control = context.control;

        string keyName = control.name.ToLower();

        if (itemButtonMap.TryGetValue(keyName, out int index))
        {
            UseItem(index);
        }
    }
    
    public void AddItem(InventoryItemData item, int amount)
    {
        foreach (var i in items)
        {
            if (i.type == item.type)
            {
                i.amount += amount;
                return;
            }
        }
        InventoryItemData newItem = new InventoryItemData(item.type, amount, item.value);
        items.Add(newItem);
    }

    public void UseItem(int index)
    {
        if (index >= items.Count) return;
        switch (items[index].type)
        {
            case ConsumableType.Stamina:
            case ConsumableType.Health:
                ConditionType type = ConditionType.Health;
                switch (items[index].type)
                {
                    case ConsumableType.Stamina:
                        type = ConditionType.Health;
                        break;
                    case ConsumableType.SpeedUp:
                        type = ConditionType.Stamina;
                        break;
                }
                GameManager.Instance.Player.condition.GetCondition(type).Add(items[index].value); 
                items[index].amount--;
                break;
            case ConsumableType.SpeedUp:
            case ConsumableType.JumpBoost:
                StartCoroutine(ItemBoost(items[index], items[index].value));
                break;
        }

        if (items[index].amount <= 0)
        {
            items.Remove(items[index]);
        }
    }
    
    public IEnumerator ItemBoost(InventoryItemData item ,float value)
    {
        ConsumableType type = item.type;
        switch (type)
        {
            case ConsumableType.SpeedUp:
                if(_isSpeedUp) yield break;
                _bonusSpeed = value;
                _isSpeedUp = true;
                break;
            case ConsumableType.JumpBoost:
                if(_isJumpBoost) yield break;
                _bonusJumpForce = value;
                _isJumpBoost = true;
                break;
        }
        item.amount--;
        
        yield return new WaitForSeconds(_itemDuration);
        
        switch (type)
        {
            case ConsumableType.SpeedUp:
                _bonusSpeed = 0f;
                _isSpeedUp = false;
                break;
            case ConsumableType.JumpBoost:
                _bonusJumpForce = 0f;
                _isJumpBoost = false;
                break;
        }
    }
}
