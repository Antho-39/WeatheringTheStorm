using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class ReconstructionPhaseUIController : MonoBehaviour
{
    private UIDocument uiDocument;

    private VisualElement phase_3_UI;
    private Button skipButton;
    private Button introNextButton;
    private Button controlsNextButton;
    private Button nextPhaseButton;

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

    private VisualElement controls_UI;
    private VisualElement intro_UI;
    private Label introLabel;
    private string fullText;

    private Label moneyLabel;
    private float score;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        moneyLabel = root.Q<Label>("MoneyLabel");
        introLabel = root.Q<Label>("IntroText");
        phase_3_UI = root.Q<VisualElement>("Phase_3");
        intro_UI = root.Q<VisualElement>("Intro");
        controls_UI = root.Q<VisualElement>("Controls");

        skipButton = root.Q<Button>("SkipButton");
        introNextButton = root.Q<Button>("IntroNextButton");
        controlsNextButton = root.Q<Button>("ControlsNextButton");
        nextPhaseButton = root.Q<Button>("NextPhaseButton");

        moneyLabel = root.Q<Label>("MoneyLabel");
        score = GameManager.Instance.money + GameManager.Instance.score;

        skipButton.clicked += SkipText;
        introNextButton.clicked += IntroNextUI;
        controlsNextButton.clicked += ControlsNextUI;
        nextPhaseButton.clicked += () => SceneLoader.LoadScene("Final_Scene");

        phase_3_UI.style.display = DisplayStyle.None;
        controls_UI.style.display = DisplayStyle.None;
        intro_UI.style.display = DisplayStyle.Flex;

        // Get the full text and clear the label
        fullText = introLabel.text;
        introLabel.text = "";

        GameManager.Instance.StopTimer();
        StartCoroutine(TypeText());
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

        phase_3_UI.style.display = DisplayStyle.Flex;
        controls_UI.style.display = DisplayStyle.None;

        GameManager.Instance.PlayPhaseMusic();
    }
}