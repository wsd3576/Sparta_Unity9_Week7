using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] GameObject _healthBar;

    private void Start()
    {
        GameManager.Instance.Player.controller.pauseMenu += TogglePauseMenu;
        TogglePauseMenu();
    }

    public void TogglePauseMenu()
    {
        if (_pauseMenu.activeInHierarchy)
        {
            _pauseMenu.SetActive(false);
        }
        else
        {
            _pauseMenu.SetActive(true);
        }
    }
}
