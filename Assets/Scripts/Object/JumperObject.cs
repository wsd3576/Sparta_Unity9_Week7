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
            
            if (other.relativeVelocity.y < -1f)
            {
                Vector3 velocity = _rigidbody.velocity;
                velocity.y = jumpPower;
                _rigidbody.velocity = velocity;
            }
        }
    }
}
