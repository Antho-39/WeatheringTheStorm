using UnityEngine;
using UnityEngine.UIElements;

public class PreparationPhaseUIController : MonoBehaviour
{
    private VisualElement cursor;
    private UIDocument uiDocument;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        cursor = root.Q<VisualElement>("CustomCursor");

        UnityEngine.Cursor.visible = false;
    }

    void Update()
    {
        if (cursor == null) return;

        Vector2 mousePos = Input.mousePosition;

        mousePos.y = Screen.height - mousePos.y;

        cursor.style.left = mousePos.x;
        cursor.style.top = mousePos.y;

        // DEBUG ! 
        if(Input.GetKeyDown(KeyCode.D))
        {
            SceneLoader.LoadScene("ActionPhaseScene");
        }
    }
}