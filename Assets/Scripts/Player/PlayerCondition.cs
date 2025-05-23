using UnityEngine;

public class PlayerCondition : MonoBehaviour, IDamageable
{
    public ConditionData[] conditions;

    private void Start()
    {
        foreach (ConditionData condition in conditions) //초기화
        {
            condition.ResetCondition();
        }
    }

    private void Update()
    {
        foreach (ConditionData condition in conditions) //초당 회복
        {
            condition.Add(condition.passiveValue * Time.deltaTime);
        }
    }

    public ConditionData GetCondition(ConditionType type) //플레이어 상태 변화시 호출
    {
        foreach (ConditionData condition in conditions)
        {
            if (condition.type == type)
            {
                condition.GetExhausted();
                return condition;
            }
        }
        return null;
    }

    public void Damage(ConditionType type, float damage) //데미지
    {
        foreach (ConditionData condition in conditions)
        {
            if (condition.type == type)
            {
                if (condition.type == ConditionType.Health) GameManager.Instance.UIManager.damageEffect.Flash();
                condition.Subtract(damage);
            }
        }
    }
}
