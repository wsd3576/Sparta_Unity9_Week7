using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return _instance;
        }
    }

    public UIManager UIManager;
    public Player Player;
    private PlayerInput playerinput;
    
    private bool _paused = false;
    public bool Paused {get{return _paused;} private set{_paused = value;}}

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        if (Player == null)
        {
            Player = FindObjectOfType<Player>();
            playerinput = Player.GetComponent<PlayerInput>();
        }
        if (UIManager == null) UIManager = FindObjectOfType<UIManager>();
        
    }

    public void PauseGame()
    {
        if (!_paused)
        {
            _paused = true;
            Time.timeScale = 0;

            foreach (var animator in FindObjectsOfType<Animator>())
            {
                animator.enabled = false;  
            }
        
            playerinput.SwitchCurrentActionMap("UI");
        }
        else
        {
            _paused = false;
            Time.timeScale = 1;

            foreach (var animator in FindObjectsOfType<Animator>())
            {
                animator.enabled = true; 
            }
            
            playerinput.SwitchCurrentActionMap("Player");
        }
        
        UIManager.TogglePauseMenu();
    }
}
