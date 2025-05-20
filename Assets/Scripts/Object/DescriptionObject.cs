using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string objectName;
    [SerializeField] private string objectDescription;
    
    public (string, string) GetObjectInfo()
    {
        return (objectName, objectDescription);
    }

    public void OnInteractInput()
    {
        //활성화?
    }
}
