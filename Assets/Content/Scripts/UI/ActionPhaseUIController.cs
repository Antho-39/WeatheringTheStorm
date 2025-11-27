using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

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

    public FireManager fireManager;

    [Header("Speed Settings")]
    public float letterDelay = 0.05f;  // Time between each letter
    public float punctuationDelay = 0.2f;       // Delay extra for . , ! ?
    public bool instantSkip = true;
    public float soundCooldown = 0.05f;

    private bool isTyping = false;
    private bool isFinished = false;
    [Header("Audio")]
    public AudioSource audioSource;
    [Header("Typing Sounds")]
    public List<AudioClip> typingSounds = new List<AudioClip>();
    private AudioClip currentAudioClip;
    private float lastSoundTime = 0;

    private Label rescueAlertLabel;
    private VisualElement rescueDirectionArrow;
    private Slider rescueTimerSlider;
    private VisualElement gaugeCross;
    private GameObject currentVictimTarget;

    private Coroutine currentAlertCoroutine;

    private Label introLabel;
    private string fullIntroText;

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

        rescueAlertLabel = root.Q<Label>("RescueAlertLabel");
        rescueDirectionArrow = root.Q<VisualElement>("RescueDirectionArrow");
        gaugeCross = root.Q<VisualElement>("GaugeCross");
        rescueTimerSlider = root.Q<Slider>("RescueTimerSlider");
        rescueAlertLabel.style.display = DisplayStyle.None;
        rescueDirectionArrow.style.display = DisplayStyle.None;
        rescueTimerSlider.style.display = DisplayStyle.None;
        gaugeCross.style.display = DisplayStyle.None;

        timeLabel = root.Q<Label>("TimeLabel");
        fullIntroText = introLabel.text;

        GameManager.Instance.StopTimer();
        StartCoroutine(TypeText(introLabel, typingSounds));
    }

    void Update()
    {
        var time = Mathf.Max(0, GameManager.Instance.gameTime);
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timeLabel.text = $"{minutes:00}:{seconds:00}";

        if (currentVictimTarget != null)
        {
            UpdateDirectionArrow(currentVictimTarget.transform.position);
        }
    }

    private IEnumerator TypeText(Label label, List<AudioClip> sounds = null)
    {
        // Get the full text and clear the label
        isTyping = true;

        float currentDelay = letterDelay;
        string labelText = label.text;
        label.text = "";

        for (int i = 0; i < labelText.Length; i++)
        {
            char c = labelText[i];

            //  Skip via bouton
            if (!isTyping) yield break;

            if (c == '<')
            {
                string tag = "<";
                i++;

                // Read full tag
                while (i < labelText.Length && labelText[i] != '>')
                {
                    tag += labelText[i];
                    i++;
                }

                tag += ">";

                // Add full tag
                label.text += tag;

                continue; // On passe au caract re suivant
            }
            // Add letter
            label.text += c;

            if(sounds != null && sounds.Count > 0)
            {
                // Play sound
                if (audioSource && Time.unscaledTime - lastSoundTime > soundCooldown)
                {
                    currentAudioClip = sounds[Random.Range(0, sounds.Count)];
                    audioSource.PlayOneShot(currentAudioClip);
                    lastSoundTime = Time.unscaledTime;
                }
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

        introLabel.text = fullIntroText;
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

        
        if (fireManager != null)
        {
            fireManager.StartFireCycle();
        }
        
        GameManager.Instance.StartTimer();
        GameManager.Instance.PlayPhaseMusic();
        RescueManager.Instance.StartRescueCycle();
    }

    public void ShowRescueAlert(RescueVictim victim)
    {
        currentVictimTarget = victim.gameObject;
        //Show it with voice
        rescueAlertLabel.text = victim.alertText;
        StartCoroutine(TypeText(rescueAlertLabel, victim.voices));
        rescueAlertLabel.style.color = Color.yellow;
        rescueAlertLabel.style.display = DisplayStyle.Flex;

        rescueDirectionArrow.style.display = DisplayStyle.Flex;
        rescueTimerSlider.style.display = DisplayStyle.Flex;
        StartCoroutine(RunRescueTimer(20f));

        if (currentAlertCoroutine != null)
            StopCoroutine(currentAlertCoroutine);
        currentAlertCoroutine = StartCoroutine(HideLabelAfterSeconds(rescueAlertLabel, 20f));
    }

    public void ShowRescueFailed()
    {
        rescueAlertLabel.text = "Oh no ! We are too late for the rescue !";
        rescueAlertLabel.style.color = Color.red;
        rescueAlertLabel.style.display = DisplayStyle.Flex;
        rescueDirectionArrow.style.display = DisplayStyle.None;
        rescueTimerSlider.style.display = DisplayStyle.None;

        if (currentAlertCoroutine != null)
            StopCoroutine(currentAlertCoroutine);
        currentAlertCoroutine = StartCoroutine(HideLabelAfterSeconds(rescueAlertLabel, 3f));
    }

    public void ShowRescueCarryMessage()
    {
        StopAllCoroutines();
        rescueTimerSlider.style.display = DisplayStyle.None;
        rescueAlertLabel.text = "Nice ! Bring this person in a safe place !";
        rescueAlertLabel.style.color = Color.green;
        rescueAlertLabel.style.display = DisplayStyle.Flex;
        rescueDirectionArrow.style.display = DisplayStyle.None;
        gaugeCross.style.display = DisplayStyle.Flex;

        if (currentAlertCoroutine != null)
            StopCoroutine(currentAlertCoroutine);
        currentAlertCoroutine = StartCoroutine(HideLabelAfterSeconds(rescueAlertLabel, 3f));
    }

    public void ShowRescueSuccess(RescueVictim victim)
    {
        //Show it with voice
        rescueAlertLabel.text = victim.rescuedText;
        StartCoroutine(TypeText(rescueAlertLabel, victim.voices));
        rescueAlertLabel.style.color = Color.green;
        rescueAlertLabel.style.display = DisplayStyle.Flex;
        gaugeCross.style.display = DisplayStyle.None;

        if (currentAlertCoroutine != null)
            StopCoroutine(currentAlertCoroutine);
        currentAlertCoroutine = StartCoroutine(HideLabelAfterSeconds(rescueAlertLabel, 3f));
    }

    public void UpdateDirectionArrow(Vector3 targetPos)
    {
        if (RescueManager.Instance == null) return;
        Transform helicopter = RescueManager.Instance.playerTransform;
        if (!helicopter) return;

        Vector3 dir = (targetPos - helicopter.position).normalized;
        dir.y = -dir.y;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        
	rescueDirectionArrow.style.rotate =
    		new Rotate(new Angle(angle), new Vector3(0, 0, 1));

    }

    private IEnumerator RunRescueTimer(float duration)
    {
        rescueTimerSlider.lowValue = 0;
        rescueTimerSlider.highValue = duration;
        float t = duration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            rescueTimerSlider.value = t;
            yield return null;
        }
        ShowRescueFailed();
    }

    private IEnumerator HideLabelAfterSeconds(Label label, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        label.style.display = DisplayStyle.None;
    }
}