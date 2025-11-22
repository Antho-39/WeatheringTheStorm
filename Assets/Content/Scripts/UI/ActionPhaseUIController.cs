using UnityEngine;
using UnityEngine.UIElements;

public class ActionPhaseUIController : MonoBehaviour
{
    private UIDocument uiDocument;

    private Label moneyLabel;
    private Label timeLabel;
    private Label scoreLabel;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        timeLabel = root.Q<Label>("TimeLabel");
    }

    void Update()
    {
        var time = Mathf.Max(0, GameManager.Instance.gameTime);
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timeLabel.text = $"{minutes:00}:{seconds:00}";

        // DEBUG ! 
        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneLoader.LoadScene("Phase_3_Scene");
        }
    }
}