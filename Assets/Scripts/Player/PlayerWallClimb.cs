using UnityEngine;

public class PlayerWallClimb : MonoBehaviour
{
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

    void Update()
    {
        climbingInput = Input.GetKey(KeyCode.Space);
        
        WallCheck();

        if (isClimbing && climbingInput)
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
        Debug.DrawRay(rayOrigin, direction, Color.red);
        if (Physics.Raycast(transform.position, direction, out hit, wallCheckDistance, wallLayer))
        {
            if (climbingInput && !isClimbing)
            {
                Debug.Log("Inputted");
                StartClimbing();
            }
        }
        else if (!Physics.Raycast(transform.position, direction, out hit, wallCheckDistance, wallLayer)) StopClimbing();
    }
    
    void ClimbWall()
    {
        ConditionData stamina = GameManager.Instance.Player.condition.GetCondition(ConditionType.Stamina);
        if (stamina.curValue >= climbStamina)
        {
            rb.AddForce(Vector3.up * climbSpeed, ForceMode.Force);
            stamina.Subtract(Time.deltaTime * climbStamina);
        }
    }
    
    void StartClimbing()
    {
        Debug.Log("Starting climbing");
        isClimbing = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }
    
    
    void StopClimbing()
    {
        Debug.Log("StopClimbing");
        isClimbing = false;
        rb.useGravity = true;
    }
}
