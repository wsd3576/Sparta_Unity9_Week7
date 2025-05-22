using UnityEngine;

public class DescriptionObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string objectName;
    [SerializeField] private string objectDescription;
    
    public (string, string, int) GetObjectInfo()
    {
        return (objectName, objectDescription, 0);
    }

    public void OnInteractInput() { }
}
