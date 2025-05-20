using System;
using UnityEngine;

public enum ConditionType
{
    Health,
    Stamina
}

[Serializable]
public class ConditionData
{
    public ConditionType type;
    public float curValue;
    public float startValue;
    public float maxValue;
    public float passiveValue;

    public void ResetCondition()
    { 
        curValue = startValue;
    }

    public float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
    }

    public void Subtract(float value)
    {
        curValue = Mathf.Max(curValue - value, 0);
    }
}