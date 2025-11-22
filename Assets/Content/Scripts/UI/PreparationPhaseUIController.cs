using UnityEngine;
using UnityEngine.UIElements;

public class PreparationPhaseUIController : MonoBehaviour
{
    private VisualElement cursor;
    private UIDocument uiDocument;

    private Label moneyLabel;
    private Label timeLabel;
    private Label scoreLabel;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        cursor = root.Q<VisualElement>("CustomCursor");
        moneyLabel = root.Q<Label>("MoneyLabel");
        timeLabel = root.Q<Label>("TimeLabel");

        UnityEngine.Cursor.visible = false;
    }

    void Update()
    {
        moneyLabel.text = GameManager.Instance.money.ToString() + " $";
        var time = Mathf.Max(0, GameManager.Instance.gameTime);
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timeLabel.text = $"{minutes:00}:{seconds:00}";

        if (cursor == null) return;

        Vector2 mousePos = Input.mousePosition;

        mousePos.y = Screen.height - mousePos.y;

        cursor.style.left = mousePos.x;
        cursor.style.top = mousePos.y;

        // DEBUG ! 
        if(Input.GetKeyDown(KeyCode.D))
        {
            SceneLoader.LoadScene("Phase_2_Scene");
        }
    }
}