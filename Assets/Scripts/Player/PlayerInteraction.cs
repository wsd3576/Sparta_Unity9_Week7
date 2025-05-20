using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private new Camera camera;
    private PlayerController controller;
    
    private float checkRate = 0.05f;
    private float lastCheckTime;
    
    [SerializeField] private float maxCheckDistance;
    [SerializeField] LayerMask layerMask;

    [SerializeField] GameObject curInteractGameObject;
    [SerializeField] private IInteractable curInteractable;
    
    private void Start()
    {
        camera = Camera.main;
        controller = GetComponent<PlayerController>();
    }
    
    private void Update()
    {
        if (Time.time - lastCheckTime < checkRate) return;
            
        lastCheckTime = Time.time;
        
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.yellow);
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
