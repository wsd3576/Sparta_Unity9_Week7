using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamageObject : MonoBehaviour
{
    public ConditionType conditionType;
    public int damage;
    public float damageRate;
    
    List<IDamageable> _things = new List<IDamageable>();

    private void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate);
    }

    void DealDamage()
    {
        for (int i = 0; i < _things.Count; i++)
        {
            _things[i].Damage(conditionType , damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable damageable))
        {
            _things.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            _things.Remove(damageable);
        }
    }
}
