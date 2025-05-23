using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerCondition playerCondition;
    private PlayerInteraction playerInteraction;
    private Rigidbody _rigidbody;
    
    public ConditionData stamina;
    
    [Header("Movement")] //기본이동
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 1.5f;
    [SerializeField] private float jumpForce = 100f;
    
    [SerializeField] private LayerMask groundLayerMask;
    
    private Vector2 curMovementInput;
    
    [Header("Movement Plus")] //스태미너 소비
    [SerializeField] private float sprintStamina = 10f;
    [SerializeField] private float jumpStamina = 10f;
    
    private bool isSprinting = false;

    [Header("Look")] //화면 회전 설정
    [SerializeField] private float minXLook = -85f;
    [SerializeField] private float maxXLook = 85f;
    [SerializeField] private float lookSensitivity = 0.5f;
    
    private new Camera camera;
    private Transform cameraContainer;
    private Vector2 mouseDelta;
    private float camCurXRot;
    private bool canLook = true;
    
    //3인칭 전환
    [SerializeField] private Transform fpsCameraTarget;
    [SerializeField] private Transform tpsCameraTarget;
    
    [SerializeField] private LayerMask thirdPersonCullingMask;
    [SerializeField] private LayerMask firstPersonCullingMask;
    
    private float smoothSpeed = 5f;
    private bool isThirdPerson = false;
    
    private Transform currentCameraTarget;
    
    //아이템 사용 관련
    private float bonusSpeed = 0f;
    private float bonusJumpForce = 0f;
    
    private Coroutine speedUpCoroutine;
    private Coroutine jumpBoostCoroutine;
    
    private readonly Dictionary<string, int> itemButtonMap = new()
    {
        { "1", 0 },
        { "2", 1 },
        { "3", 2 },
        { "4", 3 }
    };
    
    private void Awake()
    {
        camera = Camera.main;
        cameraContainer = GetComponentInChildren<Camera>().transform.parent.transform;
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void Start()
    {
        playerCondition = GameManager.Instance.Player.playerCondition;
        playerInteraction = GameManager.Instance.Player.playerInteraction;
        
        Cursor.lockState = CursorLockMode.Locked;
        currentCameraTarget = fpsCameraTarget;
        camera.cullingMask = firstPersonCullingMask;
    }

    private void Update()
    {
        if (isSprinting)
        {
            stamina.Subtract(Time.deltaTime * sprintStamina);
        }
        
        if (stamina.curValue <= 0f || stamina.exhausted)
        {
            isSprinting = false;
        }
    }

    private void FixedUpdate()
    {
        //3인칭 전환 부드러운 이동
        if (Vector3.Distance(camera.transform.position, currentCameraTarget.position) > 0.01f)
        {
            camera.transform.position = Vector3.Lerp
            (
                camera.transform.position,
                currentCameraTarget.position,
                Time.deltaTime * smoothSpeed
            );
        }
        else
        {
            camera.transform.position = currentCameraTarget.position;
        }
        
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
        if (stamina.curValue <= jumpStamina || stamina.exhausted) return;
        
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            playerCondition.GetCondition(ConditionType.Stamina).Subtract(jumpStamina);
            _rigidbody.AddForce(Vector2.up * (jumpForce + bonusJumpForce), ForceMode.Impulse);
        }
    }
    
    public bool IsGrounded()
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
            playerInteraction.OnInteractInput();
        }
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            var control = context.control;

            string keyName = control.name;

            if (itemButtonMap.TryGetValue(keyName, out int index))
            {
                playerInteraction.UseItem(index);
            }
        }
    }
    
    public void ItemBoost(ItemDataConsumable item) //속도 혹은 점프 증가 아이템 사용시 호출
    {
        //각각 중복되는 효과시 기존의 효과 코루틴을 종료시키고 새로운 코루틴 시작
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
            camera.cullingMask = isThirdPerson ? thirdPersonCullingMask : firstPersonCullingMask;
        }
    }
}
