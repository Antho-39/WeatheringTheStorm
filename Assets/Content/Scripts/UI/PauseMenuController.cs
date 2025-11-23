using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    public Texture2D muteSprite;
    public Texture2D unmuteSprite;

    private VisualElement pauseMenu;
    private Button resumeButton;
    private Button quitButton;
    private Button volumeButton;

    private bool isPaused = false;
    private bool isMuted = false;

    private void Awake()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
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

        // Show pause menu UI
        pauseMenu.style.display = DisplayStyle.Flex;

        // Freeze game
        GameManager.Instance.timerRunning = false;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        // Cacher le panneau UI
        pauseMenu.style.display = DisplayStyle.None;

        // Reprendre le jeu
        Time.timeScale = 1f;

        GameManager.Instance.timerRunning = true;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;

        GameManager.Instance.timerRunning = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
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