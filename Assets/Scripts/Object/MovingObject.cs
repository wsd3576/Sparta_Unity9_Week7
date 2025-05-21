using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [Header("Platform")]
    private Transform platform;
    [Header("Target")]
    [SerializeField] private Transform targetParent;
    private Transform[] targets;
    [SerializeField] private float moveDelay = 1f;
    [SerializeField] private float moveSpeed = 2f;
    
    private int currentTargetIndex = 0;
    private int direction = 1;
    private bool isMoving = true;

    private void Start()
    {
        //플렛폼을 지정하고 목표위치들을 받아와서 배열로 넣는다.
        platform = gameObject.transform;
        targets = new Transform[targetParent.childCount];
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = targetParent.GetChild(i);
        }
        
        StartCoroutine(MovePlatform());
    }

    private IEnumerator MovePlatform()
    {
        while (true)
        {
            Vector3 targetPos = targets[currentTargetIndex].position;
            
            while (Vector3.Distance(platform.position, targetPos) > 0.01f)
            {
                platform.position = Vector3.MoveTowards(
                    platform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }
            
            platform.position = targetPos;
            
            yield return new WaitForSeconds(moveDelay);
            
            currentTargetIndex += direction;
            
            if (currentTargetIndex >= targets.Length)
            {
                direction = -1;
                currentTargetIndex = targets.Length - 2;
            }
            else if (currentTargetIndex < 0)
            {
                direction = 1;
                currentTargetIndex = 1;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(platform.transform);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
