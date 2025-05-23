using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerCondition playerCondition;
    public PlayerInteraction playerInteraction;
    public PlayerWallClimb playerWallClimb;
    private ConditionData stamina;
    private void Awake()
    {
        GameManager.Instance.Player = this;
        playerController = GetComponent<PlayerController>();
        playerCondition = GetComponent<PlayerCondition>();
        playerInteraction = GetComponent<PlayerInteraction>();
        playerWallClimb = GetComponent<PlayerWallClimb>();
    }

    private void Update()
    {
        stamina = playerCondition.GetCondition(ConditionType.Stamina);
        playerController.stamina = stamina;
        playerWallClimb.stamina = stamina;
    }
}
