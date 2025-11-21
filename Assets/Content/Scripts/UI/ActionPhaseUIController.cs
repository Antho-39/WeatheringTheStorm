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

        moneyLabel = root.Q<Label>("MoneyLabel");
        timeLabel = root.Q<Label>("TimeLabel");
        scoreLabel = root.Q<Label>("ScoreLabel");
    }

    void Update()
    {
        moneyLabel.text = GameManager.Instance.money.ToString() + " $";
        scoreLabel.text = GameManager.Instance.score.ToString();
        var time = Mathf.Max(0, GameManager.Instance.gameTime);
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timeLabel.text = $"{minutes:00}:{seconds:00}";

        // DEBUG ! 
        if (Input.GetKeyDown(KeyCode.D))
        {
            SceneLoader.LoadScene("Phase_3_Scene");
        }
    }
}