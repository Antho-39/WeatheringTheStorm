using UnityEngine;
using UnityEngine.UIElements;

public class FinalSceneUIController : MonoBehaviour
{
    private UIDocument uiDocument;

    private Button replayButton;
    private Button leaveButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        replayButton = root.Q<Button>("ButtonPlay");
        leaveButton = root.Q<Button>("ButtonLeave");

        replayButton.clicked += ReplayGame;
        leaveButton.clicked += Quit;
    }

    private void ReplayGame()
    {
        SceneLoader.LoadScene("Menu_Scene");
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
