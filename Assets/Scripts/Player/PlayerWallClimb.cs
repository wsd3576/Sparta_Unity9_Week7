using System.Collections;
using UnityEngine;

public class PlayerWallClimb : MonoBehaviour
{
    private PlayerCondition playerCondition;
    
    public ConditionData stamina;
    
    [Header("Settings")]
    public float wallCheckDistance = 1f;
    public float offSetY = 1.7f;
    public float climbSpeed = 3f;
    public float climbStamina = 10f;
    public float hangingStamina = 5f;
    
    [Header("References")]
    public LayerMask wallLayer;
    public Transform cameraContainer;
    
    [SerializeField] private bool climbingInput = false;
    [SerializeField] private bool isClimbing = false;
    [SerializeField] private bool isHanging = false;
    [SerializeField] private Vector3 hangingPoint;
    [SerializeField] private float hangingDistance = 0.2f;
    [SerializeField] private float lastHangingTime = 5f;
    [SerializeField] private float hangingCooldown = 0.5f;
    
    private Rigidbody _rigidboy;

    private void Awake()
    {
        _rigidboy = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerCondition = GameManager.Instance.Player.playerCondition;
    }

    void Update()
    {
        climbingInput = Input.GetKey(KeyCode.Space);
        
        WallCheck();
        HangingCheck();
        
        if (isHanging) //매달린 상태일때
        {
            stamina.Subtract(hangingStamina * Time.deltaTime);
            //s키를 누르거나 지친 상태면 놓는다.
            if (Input.GetKeyDown(KeyCode.S) || stamina.exhausted) StopHanging();
            //오르기 키(스페이스바)를 누르면 다시 벽을 탄다.
            else if (Input.GetKeyDown(KeyCode.Space)) 
            {
                isHanging = false;
                    
                StartClimbing();
            }
            //만약 매달린 위치에서 일정거리 떨어지면 놓는다.
            float distance = Vector3.Distance(transform.position, hangingPoint);
            if (distance > hangingDistance)
            {
                StopHanging();
            }
            return;
        }
        
        if (isClimbing && climbingInput) //벽타는 상태에서 벽타기 입력을 유지한 상태라면 계속 벽을 탄다.
        {
            ClimbWall();
        }
        else if (!climbingInput && isClimbing) //벽타기 입력을 풀면 그만 탄다.
        {
            StopClimbing();
        }
    }

    (bool, RaycastHit) RayCast(float offSet, float distance) //WallCheck와 HangingCheck에서 공통적으로 사용하는 벽 감지함수
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + (Vector3.up * offSet);
        Vector3 rayDirection = cameraContainer.forward;
        
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, distance, wallLayer))
        {
            return (true, hit);
        }
        return (false, hit);
    }

    void WallCheck()
    {
        bool canClimb = RayCast(offSetY, wallCheckDistance).Item1;

        if (canClimb && climbingInput && !isClimbing) //벽이 감지되었고 벽타기 버튼을 누른 상태고, 벽타기 중이 아닐 때.
        {
            StartClimbing();
        }
    }
    
    void ClimbWall()
    {
        if (!stamina.exhausted) //지친 상태가 아닐 때 일정한 속도로 올라간다.
        {
            Vector3 targetVelocity = _rigidboy.velocity;
            targetVelocity.y = climbSpeed;
            
            _rigidboy.velocity = targetVelocity;
            
            stamina.Subtract(Time.deltaTime * climbStamina);
        }
        else
        {
            StopClimbing();
        }
    }
    
    void StartClimbing() //벽타기 상태를 처리하고 중력을 끄고 y속도를 없앤다.
    {
        isClimbing = true;
        _rigidboy.useGravity = false;
        Vector3 velocity = _rigidboy.velocity;
        velocity.y = 0f;
        _rigidboy.velocity = velocity;
    }
    
    
    void StopClimbing() //벽타기 상태를 처리하고 중력을 다시 킨다.
    {
        isClimbing = false;
        _rigidboy.useGravity = true;
    }
    
    void HangingCheck()
    {
        //벽타기 상태거나 매달린 상태, 매달리기 쿨타임이면 넘어간다.
        if (isClimbing || isHanging) return;
        if (Time.time < lastHangingTime + hangingCooldown) return;
        //벽감지로 감지되었는지와 감지된 곳을 받아온다.
        (bool canHang, RaycastHit hit) = RayCast(offSetY, wallCheckDistance);
        //감지된 부분의 위치를 받아서 그 위치가 매달릴수 있는 곳(위쪽)인지 확인한다.
        if ( canHang && Physics.Raycast(hit.point + Vector3.up * 0.3f, Vector3.down, out RaycastHit hangPos, 1f, wallLayer))
        {
            StartHanging();
        }
    }
    
    void StartHanging() //매달린 상태와 벽타기 상태를 처리하고 중력끄고, 움직임을 멈춘뒤 해당 위치를 매달린 위치로 지정한다.
    {
        isHanging = true;
        isClimbing = false;
        _rigidboy.useGravity = false;
        _rigidboy.velocity = Vector3.zero;
        
        hangingPoint = transform.position;
    }

    void StopHanging() //매달린 상태를 처리하고 중력을 다시 킨 뒤 마지막으로 매달린 시간을 할당한다.
    {
        isHanging = false;
        _rigidboy.useGravity = true;
        lastHangingTime = Time.time;
    }
}
