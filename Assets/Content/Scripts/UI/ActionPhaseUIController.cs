using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;


public class ActionPhaseUIController : MonoBehaviour
{
    private UIDocument uiDocument;

    private VisualElement phase_2_UI;
    private VisualElement controls_UI;
    private VisualElement intro_UI;

    private Label moneyLabel;
    private Label timeLabel;
    private Label scoreLabel;

    private Button skipButton;
    private Button introNextButton;
    private Button controlsNextButton;

    public WeatherController weatherController;

    [Header("Speed Settings")]
    public float letterDelay = 0.05f;  // Time between each letter
    public float punctuationDelay = 0.2f;       // Delay extra for . , ! ?
    public bool instantSkip = true;
    public float soundCooldown = 0.05f;

    private bool isTyping = false;
    private bool isFinished = false;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip letterSound_1;
    public AudioClip letterSound_2;
    private AudioClip currentAudioClip;
    private float lastSoundTime = 0;

    private Label introLabel;
    private string fullText;

    void Start()
    {
        UnityEngine.Cursor.visible = true;

        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        phase_2_UI = root.Q<VisualElement>("Phase_2");
        intro_UI = root.Q<VisualElement>("Intro");
        controls_UI = root.Q<VisualElement>("Controls");
        introLabel = root.Q<Label>("IntroText");

        skipButton = root.Q<Button>("SkipButton");
        introNextButton = root.Q<Button>("IntroNextButton");
        controlsNextButton = root.Q<Button>("ControlsNextButton");

        skipButton.clicked += SkipText;
        introNextButton.clicked += IntroNextUI;
        controlsNextButton.clicked += ControlsNextUI;

        phase_2_UI.style.display = DisplayStyle.None;
        controls_UI.style.display = DisplayStyle.None;
        intro_UI.style.display = DisplayStyle.Flex;

        timeLabel = root.Q<Label>("TimeLabel");

        // Get the full text and clear the label
        fullText = introLabel.text;
        introLabel.text = "";

        GameManager.Instance.StopTimer();
        StartCoroutine(TypeText());
    }

    void Update()
    {
        var time = Mathf.Max(0, GameManager.Instance.gameTime);
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timeLabel.text = $"{minutes:00}:{seconds:00}";

        // DEBUG ! 
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneLoader.LoadScene("Phase_3_Scene");
        }
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

        phase_2_UI.style.display = DisplayStyle.Flex;
        controls_UI.style.display = DisplayStyle.None;
        UnityEngine.Cursor.visible = false;

        if (weatherController != null)
        {
            weatherController.StartWeatherCycle();
        }
        GameManager.Instance.StartTimer();
        GameManager.Instance.PlayPhaseMusic();
    }
}