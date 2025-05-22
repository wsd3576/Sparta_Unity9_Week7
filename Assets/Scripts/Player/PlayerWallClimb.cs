using System;
using UnityEngine;

public class PlayerWallClimb : MonoBehaviour
{
    private PlayerCondition playerCondition;
    private ConditionData stamina;
    
    [Header("Settings")]
    public float wallCheckDistance = 1f;

    public float rayOffSetY = 1.7f;
    public float climbSpeed = 3f;

    public float climbStamina = 5f;
    
    [Header("References")]
    public LayerMask wallLayer;
    public Transform cameraContainer;
    
    private bool climbingInput = false;
    private bool isClimbing = false;
    
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerCondition = GameManager.Instance.Player.playerCondition;
        stamina = playerCondition.GetCondition(ConditionType.Stamina);
    }

    void Update()
    {
        climbingInput = Input.GetKey(KeyCode.Space);
        
        WallCheck();

        if (isClimbing && climbingInput && !stamina.exhausted)
        {
            ClimbWall();
        }
        else StopClimbing();
    }
    
    void WallCheck()
    {
        RaycastHit hit;

        Vector3 rayOrigin = transform.position + (Vector3.up * rayOffSetY);
        Vector3 direction = cameraContainer.forward;
        
        if (Physics.Raycast(transform.position, direction, out hit, wallCheckDistance, wallLayer))
        {
            if (climbingInput && !isClimbing)
            {
                StartClimbing();
            }
        }
        else if (!Physics.Raycast(transform.position, direction, out hit, wallCheckDistance, wallLayer)) StopClimbing();
    }
    
    void ClimbWall()
    {
        if (stamina.curValue >= climbStamina && !stamina.exhausted)
        {
            Vector3 targetVelocity = rb.velocity;
            targetVelocity.y = climbSpeed;
            
            rb.velocity = targetVelocity;
            
            stamina.Subtract(Time.deltaTime * climbStamina);
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
}
