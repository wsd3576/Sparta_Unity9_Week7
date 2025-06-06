using UnityEngine;
using UnityEngine.UI;

public class ConditionUI : MonoBehaviour
{
    [SerializeField] private ConditionType type;
    [SerializeField] private Image uiBar;
    private ConditionData condition;

    private void Start()
    {
        condition = GameManager.Instance.Player.playerCondition.GetCondition(type); //본인의 타입과 같은 플레이어의 상태값을 가져온다.
    }

    private void Update()
    {
        if (condition != null) //UI에서는 읽어와서 표시만 한다.
        {
            uiBar.fillAmount = condition.GetPercentage();
        }
    }
}
