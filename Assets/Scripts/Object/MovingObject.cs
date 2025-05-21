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
            //0번째부터 목표 지정
            Vector3 targetPos = targets[currentTargetIndex].position;
            
            //목표지점에 오차 0.01까지 다가가고 오차범위 내 일 경우엔 해당 위치로 맞춘다.
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
            
            //현재 방향 1이면 1-2-3 방향 -1이면 3-2-1방향 의 목표 순서를 지정
            currentTargetIndex += direction;
            
            if (currentTargetIndex >= targets.Length) //순서가 마지막 순서 이상이면 역방향으로 만들고 마지막-1번째 순서 목표를 향하게 한다.
            {
                direction = -1;
                currentTargetIndex = targets.Length - 2;
            }
            else if (currentTargetIndex < 0) //역방향으로 가던 중 순서가 0보다 낮으면 다시 정방향으로 진행
            {
                direction = 1;
                currentTargetIndex = 1;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //플레이어가 들어오면 발판을 따라가게 하기 위해 자신으로 오브젝트 위치를 옮긴다.
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
