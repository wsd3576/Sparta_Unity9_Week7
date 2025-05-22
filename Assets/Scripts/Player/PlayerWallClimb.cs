using System.Collections;
using UnityEngine;

public class PlayerWallClimb : MonoBehaviour
{
    private PlayerCondition playerCondition;
    private ConditionData stamina;
    
    [Header("Settings")]
    public float wallCheckDistance = 1f;
    public float offSetY = 1.7f;
    public float climbSpeed = 3f;
    public float climbStamina = 5f;
    public float hangingStamina = 5f;
    
    [Header("References")]
    public LayerMask wallLayer;
    public Transform cameraContainer;
    
    [SerializeField] private bool climbingInput = false;
    [SerializeField] private bool isClimbing = false;
    [SerializeField] private bool isHanging = false;
    [SerializeField] private Vector3 hangingPoint;
    [SerializeField] private float hangingDistance = 0.2f;
    
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerCondition = GameManager.Instance.Player.playerCondition;
    }

    void Update()
    {
        stamina = playerCondition.GetCondition(ConditionType.Stamina);
        
        climbingInput = Input.GetKey(KeyCode.Space);

        WallCheck();
        HangingCheck();
        
        if (isHanging)
        {
            stamina.Subtract(hangingStamina * Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.S) || stamina.exhausted) StopHanging();
            else if (Input.GetKeyDown(KeyCode.Space)) 
            {
                isHanging = false;
                    
                StartClimbing();
            }
            float distance = Vector3.Distance(transform.position, hangingPoint);
            if (distance > hangingDistance)
            {
                StopHanging();
            }
            return;
        }
        
        if (isClimbing && climbingInput)
        {
            ClimbWall();
        }
        else if (!climbingInput && isClimbing)
        {
            StopClimbing();
        }
    }

    (bool, RaycastHit) RayCast(float offSet, float distance)
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + (Vector3.up * offSet);
        Vector3 rayDirection = cameraContainer.forward;
        Debug.DrawRay(rayOrigin, rayDirection, Color.red, 1f);
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, distance, wallLayer))
        {
            return (true, hit);
        }
        return (false, hit);
    }

    void WallCheck()
    {
        bool canClimb = RayCast(offSetY, wallCheckDistance).Item1;

        if (canClimb && climbingInput && !isClimbing)
        {
            Debug.Log("벽탄당.");
            StartClimbing();
        }
    }
    
    void ClimbWall()
    {
        if (!stamina.exhausted)
        {
            Vector3 targetVelocity = rb.velocity;
            targetVelocity.y = climbSpeed;
            
            rb.velocity = targetVelocity;
            
            stamina.Subtract(Time.deltaTime * climbStamina);
        }
        else
        {
            StopClimbing();
        }
    }
    
    void StartClimbing()
    {
        isClimbing = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }
    
    
    void StopClimbing()
    {
        isClimbing = false;
        rb.useGravity = true;
    }
    
    void HangingCheck()
    {
        if (isClimbing || isHanging) return;

        (bool canHang, RaycastHit hit) = RayCast(offSetY, wallCheckDistance);
        
        if ( canHang && Physics.Raycast(hit.point + Vector3.up * 0.3f, Vector3.down, out RaycastHit hangPos, 1f, wallLayer))
        {
            Debug.DrawRay(hit.point+Vector3.up * 0.3f, Vector3.down, Color.red, 2f);
            Debug.Log("붙는다.");
            StartHanging(hangPos.point);
        }
    }
    
    void StartHanging(Vector3 point)
    {
        isHanging = true;
        isClimbing = false;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        
        //transform.position = point + (Vector3.down + (-cameraContainer.forward * 0.5f));
        hangingPoint = transform.position;
    }

    void StopHanging()
    {
        isHanging = false;
        rb.useGravity = true;
    }
}
