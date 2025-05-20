using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour, IDamageable
{
    public ConditionData[] conditions;

    private void Start()
    {
        foreach (ConditionData condition in conditions)
        {
            condition.ResetCondition();
        }
    }

    private void Update()
    {
        foreach (ConditionData condition in conditions)
        {
            condition.Add(condition.passiveValue * Time.deltaTime);
        }
    }

    public ConditionData GetCondition(ConditionType type)
    {
        foreach (ConditionData condition in conditions)
        {
            if (condition.type == type)
            {
                return condition;
            }
        }
        return null;
    }

    public void AddToCondition(ConditionType type, float value)
    {
        GetCondition(type)?.Add(value);
    }

    public void SubtractFromCondition(ConditionType type, float value)
    {
        GetCondition(type)?.Subtract(value);
    }

    public void Damage(ConditionType type, float damage)
    {
        foreach (ConditionData condition in conditions)
        {
            if (condition.type == type)
            {
                condition.Subtract(damage);
            }
        }
    }
}
