using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    private VisualElement pauseMenu;
    private Button resumeButton;
    private Button quitButton;

    private bool isPaused = false;

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

        resumeButton.clicked += ResumeGame;
        quitButton.clicked += QuitToMenu;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
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
}