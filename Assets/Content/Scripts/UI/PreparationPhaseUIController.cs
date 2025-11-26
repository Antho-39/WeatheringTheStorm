using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class PreparationPhaseUIController : MonoBehaviour
{
    private VisualElement cursor;
    private UIDocument uiDocument;

    private VisualElement phase_1_UI;
    private Label moneyLabel;
    private Label timeLabel;
    private Label scoreLabel;
    private Button skipButton;
    private Button introNextButton;
    private Button controlsNextButton;
    private Button nextPhaseButton;

    [Header("Speed Settings")]
    public float letterDelay = 0.05f;  // Time between each letter
    public float punctuationDelay = 0.2f;       // Delay extra for . , ! ?
    public bool instantSkip = true;
    public float soundCooldown = 0.05f;

    public Sprite dragCursorSprite;
    public Sprite defaultCursorSprite;

    private bool isTyping = false;
    private bool isFinished = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip letterSound_1;
    public AudioClip letterSound_2;
    private AudioClip currentAudioClip;

    private float lastSoundTime = 0;

    private VisualElement controls_UI;
    private VisualElement intro_UI;
    private Label introLabel;
    private string fullText;
    
    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        cursor = root.Q<VisualElement>("CustomCursor");
        moneyLabel = root.Q<Label>("MoneyLabel");
        timeLabel = root.Q<Label>("TimeLabel");
        introLabel = root.Q<Label>("IntroText");
        phase_1_UI = root.Q<VisualElement>("Phase_1");
        intro_UI = root.Q<VisualElement>("Intro");
        controls_UI = root.Q<VisualElement>("Controls");

        skipButton = root.Q<Button>("SkipButton");
        introNextButton = root.Q<Button>("IntroNextButton");
        controlsNextButton = root.Q<Button>("ControlsNextButton");
        nextPhaseButton = root.Q<Button>("NextPhaseButton");

        var btnFireCrew = root.Q<Button>("FireCrewButton");
        var btnFireLine = root.Q<Button>("FireLineButton");
        var btnSafeZone = root.Q<Button>("SafeZoneButton");

        btnFireCrew.clicked += () =>
        {
            var def = GameManager.Instance.database.GetById("FIRE_CREW");
            PlacementManager.Instance.StartPlacing(def);
        };

        btnFireLine.clicked += () =>
        {
            var def = GameManager.Instance.database.GetById("FIRE_LINE");
            PlacementManager.Instance.StartPlacing(def);
        };

        btnSafeZone.clicked += () =>
        {
            var def = GameManager.Instance.database.GetById("SAFE_ZONE");
            PlacementManager.Instance.StartPlacing(def);
        };

        skipButton.clicked += SkipText;
        introNextButton.clicked += IntroNextUI;
        controlsNextButton.clicked += ControlsNextUI;
        nextPhaseButton.clicked += () => SceneLoader.LoadScene("Phase_2_Scene");

        phase_1_UI.style.display = DisplayStyle.None;
        controls_UI.style.display = DisplayStyle.None;
        intro_UI.style.display = DisplayStyle.Flex;

        // Get the full text and clear the label
        fullText = introLabel.text;
        introLabel.text = "";

        cursor.style.backgroundImage = new StyleBackground(defaultCursorSprite);

        GameManager.Instance.StopTimer();
        StartCoroutine(TypeText());
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

    }

    private IEnumerator TypeText()
    {
        isTyping = true;

        float currentDelay = letterDelay;

        introLabel.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            char c = fullText[i];

            //  Skip via bouton
            if (!isTyping) yield break;

            if (c == '<')
            {
                string tag = "<";
                i++;

                // Read full tag
                while (i < fullText.Length && fullText[i] != '>')
                {
                    tag += fullText[i];
                    i++;
                }

                tag += ">";

                // Add full tag
                introLabel.text += tag;

                continue; // On passe au caractère suivant
            }
            // Add letter
            introLabel.text += c;

            // Play sound
            if (audioSource && Time.unscaledTime - lastSoundTime > soundCooldown)
            {
                currentAudioClip = (Random.value > 0.5f) ? letterSound_1 : letterSound_2;
                audioSource.PlayOneShot(currentAudioClip);
                lastSoundTime = Time.unscaledTime;
            }

            // Pause ponctuation
            if (".,!?".Contains(c))
                yield return new WaitForSeconds(punctuationDelay);
            else
                yield return new WaitForSeconds(currentDelay);
        }

        isTyping = false;
        isFinished = true;
    }

    public void SetDragCursor(bool isDragging)
    {
        if (isDragging)
        {
            cursor.style.backgroundImage = new StyleBackground(dragCursorSprite);
        }
        else
        {
            cursor.style.backgroundImage = new StyleBackground(defaultCursorSprite);
        }
    }

    private void SkipText()
    {
        if (!isTyping) return;

        introLabel.text = fullText;
        isTyping = false;
        isFinished = true;
    }

    private void IntroNextUI()
    {
        if (!isFinished) return;

        controls_UI.style.display = DisplayStyle.Flex;
        intro_UI.style.display = DisplayStyle.None;
    }

    private void ControlsNextUI()
    {
        if (!isFinished) return;

        phase_1_UI.style.display = DisplayStyle.Flex;
        controls_UI.style.display = DisplayStyle.None;
        UnityEngine.Cursor.visible = false;

        GameManager.Instance.StartTimer();
        GameManager.Instance.PlayPhaseMusic();
    }
}