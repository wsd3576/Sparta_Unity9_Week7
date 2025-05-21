using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperObject : MonoBehaviour
{
    [SerializeField] private float jumpPower;
    private Rigidbody _rigidbody;

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _rigidbody = other.gameObject.GetComponent<Rigidbody>();
            
            if (other.relativeVelocity.y < -1f) //플레이어가 밑으로 내려온 상황에만 점프시킨다.
            {
                _rigidbody.AddForce(Vector2.up * (jumpPower), ForceMode.Impulse);
            }
        }
    }
}
