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
    private Button validateButton;
    private Button continueButton;
    private Button skipButtonstarring;
    private Button continueButtonstarring;

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
    private VisualElement score_UI;
    private Label introLabel;
    private string fullIntroText;
    private Label currentScoreLabel;
    private string fullScoreText;

    private Label ashesandregrest;
    private Label singedbutstanding;
    private Label holdingtheline;
    private Label communityChampion;
    private Label heroofMontparisBay;

    private Label labelCostHome;
    private Label labelCostCompliantHome;
    private Label labelCostTree;
    private Label labelCostTotal;

    private SliderInt homesSlider;
    private SliderInt compliantHomesSlider;
    private SliderInt treesSlider;
    private Label labelNumberSliderHome;
    private Label labelNumberSliderCompliantHome;
    private Label labelNumberSliderTree;

    private Label labelBurntHomes;
    private Label labelBurntTrees;
    private Label labelInjuries;
    private Label labelRescued;

    private Label labelError;

    private Label labelPreparationScore;
    private Label labelInjurieScore;
    private Label labelRescuedScore;
    private Label labelDamageScore;
    private Label labelPhase2Score;
    private Label labelTotalScore;
    private Label totalScoreLabel;

    private VisualElement oneStarrating;
    private VisualElement twoStarrating;
    private VisualElement threeStarrating;
    private VisualElement fourStarrating;
    private VisualElement fiveStarrating;
    private VisualElement starratingScreens;

    public int buildingReparationCost = 3000;
    public int homeReparationCost = 500;
    public int compliantHomeReparationCost = 1000;
    public int treeCost = 50;

    public int repairedHomePointScale = 30;
    public int repairedCompliantHomePointScale = 90;
    public int plantedTreePointScale = 4;

    public int ratingOneStar;
    public int ratingTwoStar;
    public int ratingThreeStar;
    public int ratingFourStar;

    private Label moneyLabel;
    private float score;

    private int phase3Cost;
    private int burntHomes;
    private int burntTrees;
    private int rescuedPeople;

    private int fireCrews;
    private int fireLines;
    private int safeZones;

    private int fastSuppression;
    private int slowSuppression;
    private int injuries;

    private int repairedHome;
    private int repairedCompliantHome;
    private int plantedTrees;

    void Start()
    {
        burntHomes = GameManager.Instance.homesDestroyed;
        burntTrees = GameManager.Instance.treesDestroyed;
        fireCrews = GameManager.Instance.fireCrews;
        fireLines = GameManager.Instance.fireLines;
        safeZones = GameManager.Instance.safeZones;
        fastSuppression = GameManager.Instance.fastFireSuppression;
        slowSuppression = GameManager.Instance.slowFireSuppression;

        phase3Cost = 0;
        repairedHome = 0;
        repairedCompliantHome = 0;
        plantedTrees = 0;
        UnityEngine.Cursor.visible = true;

        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        moneyLabel = root.Q<Label>("MoneyLabel");
        introLabel = root.Q<Label>("IntroText");
        phase_3_UI = root.Q<VisualElement>("Phase_3");
        intro_UI = root.Q<VisualElement>("Intro");
        controls_UI = root.Q<VisualElement>("Controls");
        score_UI = root.Q<VisualElement>("Score");

        ashesandregrest = root.Q<Label>("Ashesandregrest");
        singedbutstanding = root.Q<Label>("Singedbutstanding");
        holdingtheline = root.Q<Label>("Holdingtheline");
        communityChampion = root.Q<Label>("CommunityChampion");
        heroofMontparisBay = root.Q<Label>("HeroofMontparisBay");

        skipButton = root.Q<Button>("SkipButton");
        introNextButton = root.Q<Button>("IntroNextButton");
        controlsNextButton = root.Q<Button>("ControlsNextButton");
        validateButton = root.Q<Button>("ValidateButton");
        continueButton = root.Q<Button>("ContinueButton");
        skipButtonstarring = root.Q<Button>("SkipButtonstarring");
        continueButtonstarring = root.Q<Button>("ContinueButtonstarring");

        skipButton.clicked += SkipIntroText;
        introNextButton.clicked += IntroNextUI;
        controlsNextButton.clicked += ControlsNextUI;
        validateButton.clicked += ShowFinalScore;
        continueButton.clicked += () => SceneLoader.LoadScene("Final_Scene");
        skipButtonstarring.clicked += SkipScoreText;
        continueButtonstarring.clicked += ShowScoreDetails;

        skipButtonstarring.style.display = DisplayStyle.None;
        continueButtonstarring.style.display = DisplayStyle.None;

        labelCostHome = root.Q<Label>("CostHomeLabel");
        labelCostCompliantHome = root.Q<Label>("CostCompliantHomeLabel");
        labelCostTree = root.Q<Label>("CostTreeLabel");
        labelCostTotal = root.Q<Label>("CostTotalLabel");

        homesSlider = root.Q<SliderInt>("SliderHome");
        compliantHomesSlider = root.Q<SliderInt>("SliderCompliantHome");
        treesSlider = root.Q<SliderInt>("SliderTree");
        labelNumberSliderHome = root.Q<Label>("NumberforsliderHome");
        labelNumberSliderCompliantHome = root.Q<Label>("NumberforsliderCompliantHome");
        labelNumberSliderTree = root.Q<Label>("NumberforsliderTree");

        labelPreparationScore = root.Q<Label>("PreparationScore");
        labelInjurieScore = root.Q<Label>("InjuriesScore");
        labelRescuedScore = root.Q<Label>("RescuedScore");
        labelDamageScore = root.Q<Label>("DamagesScore");
        labelPhase2Score = root.Q<Label>("Phase2Score");
        labelTotalScore = root.Q<Label>("TotalScore");
        totalScoreLabel = root.Q<Label>("TotalScoreLabel");

        oneStarrating = root.Q<VisualElement>("OneStarrating");
        twoStarrating = root.Q<VisualElement>("TwoStarrating");
        threeStarrating = root.Q<VisualElement>("ThreeStarrating");
        fourStarrating = root.Q<VisualElement>("FourStarrating");
        fiveStarrating = root.Q<VisualElement>("FiveStarrating");
        starratingScreens = root.Q<VisualElement>("StarratingScreens");

        oneStarrating.style.display = DisplayStyle.None;
        twoStarrating.style.display = DisplayStyle.None;
        threeStarrating.style.display = DisplayStyle.None;
        fourStarrating.style.display = DisplayStyle.None;
        fiveStarrating.style.display = DisplayStyle.None;
        starratingScreens.style.display = DisplayStyle.None;

        labelError = root.Q<Label>("ErrorLabel");
        labelError.style.opacity = 0;
        labelError.style.display = DisplayStyle.None;

        labelBurntHomes = root.Q<Label>("HomeLabel");
        labelBurntHomes.text = "- " + burntHomes.ToString() + " burnt home(s)";
        labelBurntTrees = root.Q<Label>("TreeLabel");
        labelBurntTrees.text = "- " + burntTrees.ToString() + " burnt tree(s)";
        labelInjuries = root.Q<Label>("InjuriesLabel");
        injuries = GameManager.Instance.notRescuedVictim;
        labelInjuries.text = "- " + injuries.ToString() + " injuries";

        labelNumberSliderHome.text = "Max " + burntHomes.ToString();
        labelNumberSliderCompliantHome.text = "Max " + burntHomes.ToString();
        labelNumberSliderTree.text = "Max 100";
        //labelRescued = root.Q<Label>("RescuedLabel");
        rescuedPeople = GameManager.Instance.rescuedVictim;
        //labelRescued.text = "- " + rescuedPeople.ToString() + " victim(s) rescued";

        homesSlider.lowValue = 0;
        homesSlider.highValue = burntHomes;
        compliantHomesSlider.lowValue = 0;
        compliantHomesSlider.highValue = burntHomes;
        treesSlider.lowValue = 0;
        treesSlider.highValue = 100;
        homesSlider.RegisterCallback<ChangeEvent<int>>(OnHomesChanged);
        compliantHomesSlider.RegisterCallback<ChangeEvent<int>>(OnCompliantHomesChanged);
        treesSlider.RegisterCallback<ChangeEvent<int>>(OnTreesChanged);

        labelTotalScore.style.opacity = 0f;
        phase_3_UI.style.display = DisplayStyle.None;
        controls_UI.style.display = DisplayStyle.None;
        score_UI.style.display = DisplayStyle.None;
        intro_UI.style.display = DisplayStyle.Flex;

        // Get the full text and clear the label
        fullIntroText = introLabel.text;

        GameManager.Instance.StopTimer();
        StartCoroutine(TypeText(introLabel));
    }


    private IEnumerator TypeText(Label label)
    {
        isTyping = true;

        float currentDelay = letterDelay;

        string labelText = label.text;
        introLabel.text = "";

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
                introLabel.text += tag;

                continue; // On passe au caract re suivant
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

    private void SkipIntroText()
    {
        if (!isTyping) return;

        introLabel.text = fullIntroText;
        isTyping = false;
        isFinished = true;
    }

    private void SkipScoreText()
    {
        if (!isTyping) return;

        currentScoreLabel.text = fullScoreText;
        isTyping = false;
        isFinished = true;
    }


    private void ShowFinalScore()
    {
        CountScore();
        phase_3_UI.style.display = DisplayStyle.None;
    }

    private void ShowScoreDetails()
    {
        score_UI.style.display = DisplayStyle.Flex;
        oneStarrating.style.display = DisplayStyle.None;
        twoStarrating.style.display = DisplayStyle.None;
        threeStarrating.style.display = DisplayStyle.None;
        fourStarrating.style.display = DisplayStyle.None;
        fiveStarrating.style.display = DisplayStyle.None;
        starratingScreens.style.display = DisplayStyle.None;
        labelTotalScore.style.opacity = 0f;
        skipButtonstarring.style.display = DisplayStyle.None;
        continueButtonstarring.style.display = DisplayStyle.None;
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
        AddDonationMoney();
        moneyLabel.text = GameManager.Instance.money.ToString() + " $";
    }

    private void AddDonationMoney()
    {
        int donation = Random.Range(4990, 5010);
        GameManager.Instance.AddMoney(donation);
        ShowError("You received a donation of " + donation.ToString() + " $ to help with the reconstruction !", 5f, 0.4f);
    }

    private void CountScore()
    {
        //int injuriesScore = (injuries * -20);
        
        int fireCrewScore = fireCrews * 10;
        int fireLineScore = fireLines * 20;
        int safeZoneScore = safeZones * 20;
        int phase1Score = fireCrewScore + fireLineScore + safeZoneScore;

        int fireScore = (fastSuppression * 10) + (slowSuppression * -1);
        int rescuedScore = rescuedPeople * 50;
        
        int damageScore = (burntHomes * -5) + (burntTrees * -1) + (repairedHome * 30) + (repairedCompliantHome * 90) + (plantedTrees * 4);

        score = phase1Score + fireScore + damageScore + rescuedScore;
        
        // Debug logs to help with balancing!
        print("Phase 1 score: " + phase1Score);
        print("Fire score: " + fireScore);
        print("Rescue Score: " + rescuedScore);        
        print("Damage Score: " + damageScore);
        print("Final Score: " + score);

        labelPreparationScore.text = phase1Score.ToString();
        labelPhase2Score.text = fireScore.ToString();
        labelRescuedScore.text = rescuedScore.ToString();
        labelDamageScore.text = damageScore.ToString();
        labelTotalScore.text = "SCORE " + score.ToString();
        totalScoreLabel.text = score.ToString();
        SetStarRating(score);
    }

    private void SetStarRating(float finalScore)
    {
        labelTotalScore.style.opacity = 1f;
        skipButtonstarring.style.display = DisplayStyle.Flex;
        continueButtonstarring.style.display = DisplayStyle.Flex;
        starratingScreens.style.display = DisplayStyle.Flex;

        if (finalScore < ratingOneStar)
        {
            oneStarrating.style.display = DisplayStyle.Flex;
            currentScoreLabel = ashesandregrest;
        }
        else if(finalScore < ratingTwoStar)
        {
            twoStarrating.style.display = DisplayStyle.Flex;
            currentScoreLabel = singedbutstanding;
        }
        else if(finalScore < ratingThreeStar)
        {
            threeStarrating.style.display = DisplayStyle.Flex;
            currentScoreLabel = holdingtheline;
        }
        else if(finalScore < ratingFourStar)
        {
            fourStarrating.style.display = DisplayStyle.Flex;
            currentScoreLabel = communityChampion;
        }
        else
        {
            fiveStarrating.style.display = DisplayStyle.Flex;
            currentScoreLabel = heroofMontparisBay;
        }
        fullScoreText = currentScoreLabel.text;
        TypeText(currentScoreLabel);
    }

    private void OnHomesChanged(ChangeEvent<int> evt)
    {
        repairedHome = evt.newValue;
        ApplyHomeCostLimit();
        UpdateCosts();
    }

    private void OnCompliantHomesChanged(ChangeEvent<int> evt)
    {
        repairedCompliantHome = evt.newValue;
        ApplyCompliantHomesCostLimit();
        UpdateCosts();
    }

    private void OnTreesChanged(ChangeEvent<int> evt)
    {
        plantedTrees = evt.newValue;
        ApplyTreeCostLimit();
        UpdateCosts();
    }

    private void UpdateCosts()
    {
        int costHomes = repairedHome * homeReparationCost;
        int costCompliantHomes = repairedCompliantHome * compliantHomeReparationCost;
        int costTrees = plantedTrees * treeCost;

	phase3Cost = costHomes + costCompliantHomes + costTrees;

        int totalCost = costHomes + costTrees + costCompliantHomes;

        labelCostHome.text = "-" + costHomes.ToString() + " $";
        labelCostCompliantHome.text = "-" + costCompliantHomes.ToString() + " $";
        labelCostTree.text = "-" + costTrees.ToString() + " $";
        labelCostTotal.text = "-" + totalCost.ToString() + " $";
    }

    private void ApplyHomeCostLimit()
    {
        int maxAffordable = GameManager.Instance.money / homeReparationCost;

        int hardLimit = Mathf.Min(maxAffordable, (homesSlider.highValue - compliantHomesSlider.value));

        if (homesSlider.value + compliantHomesSlider.value > hardLimit)
        {
            repairedHome = hardLimit;
            homesSlider.SetValueWithoutNotify(hardLimit);
        }
    }

    private void ApplyCompliantHomesCostLimit()
    {
        int maxAffordable = GameManager.Instance.money / compliantHomeReparationCost;

        int hardLimit = Mathf.Min(maxAffordable, (compliantHomesSlider.highValue - homesSlider.value));

        if (compliantHomesSlider.value + homesSlider.value > hardLimit)
        {
            repairedCompliantHome = hardLimit;
            compliantHomesSlider.SetValueWithoutNotify(hardLimit);
        }
    }

    private void ApplyTreeCostLimit()
    {
        int maxAffordable = GameManager.Instance.money / treeCost;
        int hardLimit = Mathf.Min(maxAffordable, treesSlider.highValue);

        if (treesSlider.value > hardLimit)
        {
            plantedTrees = hardLimit;
            treesSlider.SetValueWithoutNotify(hardLimit);
        }
    }

    public void ShowError(string message, float duration = 5f, float fadeTime = 0.4f)
    {
        StopAllCoroutines();
        StartCoroutine(ErrorRoutine(message, duration, fadeTime));
    }

    private IEnumerator ErrorRoutine(string message, float duration, float fadeTime)
    {
        labelError.text = message;
        labelError.style.display = DisplayStyle.Flex;

        // -----------------------------
        // FADE IN
        // -----------------------------
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
            labelError.style.opacity = alpha;
            yield return null;
        }

        labelError.style.opacity = 1f;

        // -----------------------------
        // WAIT
        // -----------------------------
        yield return new WaitForSeconds(duration);

        // -----------------------------
        // FADE OUT
        // -----------------------------
        t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            labelError.style.opacity = alpha;
            yield return null;
        }

        labelError.style.opacity = 0f;
        labelError.style.display = DisplayStyle.None;
    }
}