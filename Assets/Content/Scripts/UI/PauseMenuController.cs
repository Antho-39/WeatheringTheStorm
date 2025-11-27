using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance;

    public Texture2D muteSprite;
    public Texture2D unmuteSprite;

    private VisualElement pauseMenu;
    private Button resumeButton;
    private Button quitButton;
    private Button volumeButton;


    public static event Action<bool> OnGamePaused;

    private bool isPaused = false;
    private bool isMuted = false;
    private bool saveCursorState;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        pauseMenu = root.Q<VisualElement>("PauseMenu");
        resumeButton = root.Q<Button>("ResumeButton");
        quitButton = root.Q<Button>("QuitButton");
        volumeButton = root.Q<Button>("VolumeButton");

        resumeButton.clicked += ResumeGame;
        quitButton.clicked += QuitToMenu;
        volumeButton.clicked += SwitchMuteState;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.timerRunning || isPaused)
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        saveCursorState = UnityEngine.Cursor.visible;
        UnityEngine.Cursor.visible = true;
        // Show pause menu UI
        pauseMenu.style.display = DisplayStyle.Flex;

        // Freeze game
        GameManager.Instance.timerRunning = false;
        Time.timeScale = 0f;

        OnGamePaused?.Invoke(true);

    }

    public void ResumeGame()
    {
        isPaused = false;

        // Cacher le panneau UI
        pauseMenu.style.display = DisplayStyle.None;

        // Reprendre le jeu
        Time.timeScale = 1f;

        GameManager.Instance.timerRunning = true;
        UnityEngine.Cursor.visible = saveCursorState;

        OnGamePaused?.Invoke(false);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;

        GameManager.Instance.timerRunning = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu_Scene");

        UnityEngine.Cursor.visible = true;
    }

    public void SwitchMuteState()
    {
        isMuted = !isMuted;
        GameManager.Instance.SetMuteState(isMuted);
        if (isMuted)
        {
            volumeButton.style.backgroundImage = new StyleBackground(muteSprite);
        }
        else
        {
            volumeButton.style.backgroundImage = new StyleBackground(unmuteSprite);
        }
    }
}