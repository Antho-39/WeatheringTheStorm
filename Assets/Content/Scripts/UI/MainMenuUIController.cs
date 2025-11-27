using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIController : MonoBehaviour
{
    private UIDocument uiDocument;

    private Button playButton;
    private Button leaveButton;
    private Button howtoscoreButton;
    private Button howtoscorebackButton;
    private VisualElement HowToScoreUI;
    private VisualElement MainMenuUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        playButton = root.Q<Button>("ButtonPlay");
        leaveButton = root.Q<Button>("ButtonLeave");
	howtoscoreButton = root.Q<Button>("ButtonHowtoscore");
	howtoscorebackButton = root.Q<Button>("Buttonhowtoscoreback");
	HowToScoreUI = root.Q<VisualElement>("HowToScoreUI");
	MainMenuUI = root.Q<VisualElement>("MainMenuUI");


if (HowToScoreUI == null)
    Debug.LogError("HowToScoreUI not found. Check UXML name and hierarchy.");
if (MainMenuUI == null)
    Debug.LogError("MainMenuUI not found. Check UXML name and hierarchy.");

        playButton.clicked += PlayGame;
        leaveButton.clicked += Quit;
	howtoscoreButton.clicked += () =>
	{
    		HowToScoreUI.style.display = DisplayStyle.Flex;
    		MainMenuUI.style.display = DisplayStyle.None;
	};	
	howtoscorebackButton.clicked += () =>
	{
    		HowToScoreUI.style.display = DisplayStyle.None;
    		MainMenuUI.style.display = DisplayStyle.Flex;
	};

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
