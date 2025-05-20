using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionUI : MonoBehaviour
{
    [SerializeField] private ConditionType type;
    [SerializeField] private Image uiBar;
    [SerializeField] private PlayerCondition playerCondition;

    private void Start()
    {
        playerCondition = GameManager.Instance.Player.condition;
    }

    private void Update()
    {
        var condition = playerCondition.GetCondition(type);
        if (condition != null)
        {
            uiBar.fillAmount = condition.GetPercentage();
        }
    }
}
