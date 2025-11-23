using UnityEngine;
using UnityEngine.UIElements;

public class ReconstructionPhaseUIController : MonoBehaviour
{
    private UIDocument uiDocument;

    private Label moneyLabel;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        moneyLabel = root.Q<Label>("MoneyLabel");
    }

    void Update()
    {
        moneyLabel.text = GameManager.Instance.money.ToString() + " $";

        // DEBUG ! 
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneLoader.LoadScene("Phase_1_Scene");
        }
    }
}