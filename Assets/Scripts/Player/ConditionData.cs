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
    public bool exhausted = false;

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

    public void GetExhausted()
    {
        if (curValue / maxValue < 0.01f)
        {
            exhausted = true;
        }
        else if (curValue / maxValue >= 0.3f)
        {
            exhausted = false;
        }
    }
}