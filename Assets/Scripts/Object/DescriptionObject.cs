using UnityEngine;

public class DescriptionObject : MonoBehaviour, IInteractable //설명을 띄우기만 할 용도의 오브젝트
{
    [SerializeField] private string objectName;
    [SerializeField] private string objectDescription;
    
    public (string, string, string) GetObjectInfo()
    {
        return (objectName, objectDescription, "");
    }

    public void OnInteractInput() { }
}
