using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    
    [Header("Object Info")]
    [SerializeField] private GameObject infoPanel;
    public GameObject InfoPanel
    {
        get{return infoPanel;}
    }
    [SerializeField] private TextMeshProUGUI objectNameText;
    [SerializeField] private TextMeshProUGUI objectDescriptionText;
    
    [Header("Conditions")]
    public ConditionUI _health;
    public ConditionUI _stamina;

    private void Awake()
    {
        GameManager.Instance.UIManager = this;
    }

    private void Start()
    {
        TogglePauseMenu();
        infoPanel.SetActive(false);
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
        }
        else
        {
            pauseMenu.SetActive(true);
        }
    }

    public void ShowObjectInfo(IInteractable interactable)
    {
        infoPanel.SetActive(true); 
        objectNameText.text = interactable.GetObjectInfo().Item1;
        objectDescriptionText.text = interactable.GetObjectInfo().Item2;
    }
}
