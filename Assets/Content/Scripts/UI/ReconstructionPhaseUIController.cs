using UnityEngine;
using UnityEngine.UIElements;

public class ReconstructionPhaseUIController : MonoBehaviour
{
    private UIDocument uiDocument;

    private Label moneyLabel;
    private Label scoreLabel;
    private int score;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        moneyLabel = root.Q<Label>("MoneyLabel");
        scoreLabel = root.Q<Label>("ScoreLabel");
        score = GameManager.Instance.money + GameManager.Instance.score;
    }

    void Update()
    {
        moneyLabel.text = GameManager.Instance.money.ToString() + " $";
        scoreLabel.text = score.ToString();

        // DEBUG ! 
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneLoader.LoadScene("Phase_1_Scene");
        }
    }
}