using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIController : MonoBehaviour
{
    private UIDocument uiDocument;

    private Button playButton;
    private Button leaveButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        playButton = root.Q<Button>("ButtonPlay");
        leaveButton = root.Q<Button>("ButtonLeave");

        playButton.clicked += PlayGame;
        leaveButton.clicked += Quit;

        GameManager.Instance.PlayPhaseMusic();
    }

    private void PlayGame()
    {
        SceneLoader.LoadScene("Phase_1_Scene");
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
