using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerCondition playerCondition;
    public PlayerInteraction playerInteraction;
    
    private void Awake()
    {
        GameManager.Instance.Player = this;
        playerController = GetComponent<PlayerController>();
        playerCondition = GetComponent<PlayerCondition>();
        playerInteraction = GetComponent<PlayerInteraction>();
    }
}
