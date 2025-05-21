using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private new Camera camera;
    private PlayerController controller;
    
    private float checkRate = 0.05f;
    private float lastCheckTime;
    
    [SerializeField] private float maxCheckDistance = 5f;
    [SerializeField] private float rayOriginOffsetY = 1.7f;
    [SerializeField] LayerMask layerMask;
    
    private Vector3 rayOrigin;
    private Vector3 rayDirection;

    [SerializeField] GameObject curInteractGameObject;
    private IInteractable curInteractable;
    
    private void Start()
    {
        camera = Camera.main;
        controller = GetComponent<PlayerController>();
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
                GameManager.Instance.UIManager.ShowObjectInfo(curInteractable);
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
}
