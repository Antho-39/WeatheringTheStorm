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
    private string fullText;

    //public TextField repairBuildingField;
    public TextField repairHomeField;
    public TextField repairCompliantHomeField;
    public TextField plantTreeField;
    //private Label labelCostBuilding;
    private Label labelCostHome;
    private Label labelCostCompliantHome;
    private Label labelCostTree;
    private Label labelCostTotal;

    //private Label labelBurntBuilding;
    private Label labelBurntHomes;
    private Label labelBurntTrees;
    //private Label labelInjuries;

    private Label labelError;

    private Label labelSocialScore;
    //private Label labelInjurieScore;
    private Label labelDamageScore;
    private Label labelPhase2Score;
    private Label labelTotalScore;

    public int buildingReparationCost = 3000;
    public int homeReparationCost = 1000;
    public int compliantHomeReparationCost = 1500;
    public int treeCost = 20;

    public int repairedHomePointScale = 50;
    public int repairedCompliantHomePointScale = 60;
    public int plantedTreePointScale = 30;

    private Label moneyLabel;
    private float score;

    private int phase3Cost;
    //private int burntBuildings;
    private int burntHomes;
    private int burntTrees;
    //private int injuries;

    //private int repairedBuilding;
    private int repairedHome;
    private int repairedCompliantHome;
    private int plantedTrees;

    void Start()
    {
        //burntBuildings = GameManager.Instance.buildingsDestroyed;
        burntHomes = GameManager.Instance.homesDestroyed;
        burntTrees = GameManager.Instance.treesDestroyed;
        phase3Cost = 0;
        //repairedBuilding = 0;
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

        skipButton = root.Q<Button>("SkipButton");
        introNextButton = root.Q<Button>("IntroNextButton");
        controlsNextButton = root.Q<Button>("ControlsNextButton");
        validateButton = root.Q<Button>("ValidateButton");
        continueButton = root.Q<Button>("ContinueButton");

        skipButton.clicked += SkipText;
        introNextButton.clicked += IntroNextUI;
        controlsNextButton.clicked += ControlsNextUI;
        validateButton.clicked += ShowFinalScore;
        continueButton.clicked += () => SceneLoader.LoadScene("Final_Scene");

        //repairBuildingField = root.Q<TextField>("InputBuilding");
        //repairBuildingField.RegisterValueChangedCallback(OnRepairBuildingChanged);
        repairHomeField = root.Q<TextField>("InputHome");
        repairHomeField.RegisterValueChangedCallback(OnRepairHomeChanged);
        repairCompliantHomeField = root.Q<TextField>("InputCompliantHome");
        repairCompliantHomeField.RegisterValueChangedCallback(OnRepairCompliantHomeChanged);
        plantTreeField = root.Q<TextField>("InputTree");
        plantTreeField.RegisterValueChangedCallback(OnPlantTreeChanged);

        //labelCostBuilding = root.Q<Label>("LabelCostBuilding");
        labelCostHome = root.Q<Label>("LabelCostHome");
        labelCostCompliantHome = root.Q<Label>("LabelCostCompliantHome");
        labelCostTree = root.Q<Label>("LabelCostTree");
        labelCostTotal = root.Q<Label>("LabelCostTotal");

        labelSocialScore = root.Q<Label>("SocialImpactScore");
        //labelInjurieScore = root.Q<Label>("InjuriesScore");
        labelDamageScore = root.Q<Label>("DamagesScore");
        labelTotalScore = root.Q<Label>("TotalScore");
        labelPhase2Score = root.Q<Label>("Phase2Score");

        labelError = root.Q<Label>("ErrorLabel");
        labelError.style.opacity = 0;
        labelError.style.display = DisplayStyle.None;

        //labelBurntBuilding = root.Q<Label>("BuildingLabel");
        //labelBurntBuilding.text = "- " + burntBuildings.ToString() + " burnt building(s)";
        labelBurntHomes = root.Q<Label>("HomeLabel");
        labelBurntHomes.text = "- " + burntHomes.ToString() + " burnt home(s)";
        labelBurntTrees = root.Q<Label>("TreeLabel");
        labelBurntTrees.text = "- " + burntTrees.ToString() + " burnt tree(s)";
        //labelInjuries = root.Q<Label>("InjurieLabel");
        //injuries = Random.Range(burntHomes, burntTrees);
        //labelInjuries.text = "- " + injuries.ToString() + " injuries";

        phase_3_UI.style.display = DisplayStyle.None;
        controls_UI.style.display = DisplayStyle.None;
        score_UI.style.display = DisplayStyle.None;
        intro_UI.style.display = DisplayStyle.Flex;

        // Get the full text and clear the label
        fullText = introLabel.text;
        introLabel.text = "";

        GameManager.Instance.StopTimer();
        StartCoroutine(TypeText());
    }

    void Update()
    {
        //moneyLabel.text = GameManager.Instance.money.ToString() + " $";
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

                continue; // On passe au caract�re suivant
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

    private void ShowFinalScore()
    {
        CountScore();
        phase_3_UI.style.display = DisplayStyle.None;
        score_UI.style.display = DisplayStyle.Flex;
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

        AddDonationMoney();
        moneyLabel.text = GameManager.Instance.money.ToString() + " $";
        GameManager.Instance.PlayPhaseMusic();
    }

    private void AddDonationMoney()
    {
        int donation = Random.Range(15000, 20000);
        GameManager.Instance.AddMoney(donation);
        ShowError("You received a donation of " + donation.ToString() + " $ to help with the reconstruction !", 5f, 0.4f);
    }

    private void CountScore()
    {
        int phase2Score = GameManager.Instance.phase2scoreBonus;
        int socialScore = repairedHome * repairedHomePointScale + repairedCompliantHome * repairedCompliantHomePointScale + /*repairedBuilding * 150*/ + plantedTrees * plantedTreePointScale;
        if(plantedTrees < burntTrees) socialScore -= 1000;
        //int injuriesScore = (injuries * -20);
        int damageScore = /*(burntBuildings * -50) + */(burntHomes * -20) + (burntTrees * -5);
        score = (socialScore/* + injuriesScore*/ + damageScore) + phase2Score;
        labelSocialScore.text = socialScore.ToString();
        //labelInjurieScore.text = injuriesScore.ToString();
        labelDamageScore.text = damageScore.ToString();
        labelPhase2Score.text = phase2Score.ToString();
        labelTotalScore.text = score.ToString();

    }
    /*
    void OnRepairBuildingChanged(ChangeEvent<string> evt)
    {
        string input = evt.newValue;
        if (!int.TryParse(input, out int value))
        {
            repairBuildingField.SetValueWithoutNotify(evt.previousValue);   // revert
            ShowError("Integer value are expected !");
            return;
        }
        if (value > burntBuildings)
        {
            repairBuildingField.SetValueWithoutNotify(evt.previousValue);
            ShowError("You try to repair more than the number of burnt element !");
            return;
        }

        int diff = value - repairedBuilding;
        int diffCost = diff * buildingReparationCost;
        if (diff > 0 && GameManager.Instance.money < diffCost)
        {
            repairBuildingField.SetValueWithoutNotify(repairedBuilding.ToString());
            ShowError("Not enough money !");
            return;
        }

        GameManager.Instance.AddMoney(-diffCost);
        phase3Cost += diffCost;
        labelCostBuilding.text = "- " + (value * buildingReparationCost).ToString() + " $";
        labelCostTotal.text = "- " + phase3Cost.ToString() + " $";
        repairedBuilding = value;
        repairBuildingField.SetValueWithoutNotify(repairedBuilding.ToString());
    }
    */
    void OnRepairHomeChanged(ChangeEvent<string> evt)
    {
        string input = evt.newValue;
        if (!int.TryParse(input, out int value))
        {
            repairHomeField.SetValueWithoutNotify(evt.previousValue);   // revert
            ShowError("Integer value are expected !");
        }
        else
        {
            if ((value + repairedCompliantHome) > burntHomes)
            {
                repairHomeField.SetValueWithoutNotify(evt.previousValue);
                ShowError("You try to repair more than the number of burnt element !");
                return;
            }

            int diff = value - repairedHome;
            int diffCost = diff * homeReparationCost;
            if (diff > 0 && GameManager.Instance.money < diffCost)
            {
                repairHomeField.SetValueWithoutNotify(repairedHome.ToString());
                ShowError("Not enough money !");
                return;
            }

            GameManager.Instance.AddMoney(-diffCost);
            phase3Cost += diffCost;
            labelCostHome.text = "- " + (value * homeReparationCost).ToString() + " $";
            labelCostTotal.text = "- " + phase3Cost.ToString() + " $";
            repairedHome = value;
            repairHomeField.SetValueWithoutNotify(repairedHome.ToString());
        }
    }

    void OnRepairCompliantHomeChanged(ChangeEvent<string> evt)
    {
        string input = evt.newValue;
        if (!int.TryParse(input, out int value))
        {
            repairCompliantHomeField.SetValueWithoutNotify(evt.previousValue);   // revert
            ShowError("Integer value are expected !");
        }
        else
        {
            if ((value + repairedHome) > burntHomes)
            {
                repairCompliantHomeField.SetValueWithoutNotify(evt.previousValue);
                ShowError("You try to repair more than the number of burnt element !");
                return;
            }

            int diff = value - repairedCompliantHome;
            int diffCost = diff * compliantHomeReparationCost;
            if (diff > 0 && GameManager.Instance.money < diffCost)
            {
                repairCompliantHomeField.SetValueWithoutNotify(repairedCompliantHome.ToString());
                ShowError("Not enough money !");
                return;
            }

            GameManager.Instance.AddMoney(-diffCost);
            phase3Cost += diffCost;
            labelCostCompliantHome.text = "- " + (value * compliantHomeReparationCost).ToString() + " $";
            labelCostTotal.text = "- " + phase3Cost.ToString() + " $";
            repairedCompliantHome = value;
            repairCompliantHomeField.SetValueWithoutNotify(repairedCompliantHome.ToString());
        }        
    }

    void OnPlantTreeChanged(ChangeEvent<string> evt)
    {
        string input = evt.newValue;
        if (!int.TryParse(input, out int value))
        {
            plantTreeField.SetValueWithoutNotify(evt.previousValue);   // revert
            ShowError("Integer value are expected !");
        }
        else
        {
            int diff = value - plantedTrees;
            int diffCost = diff * treeCost;
            if (diff > 0 && GameManager.Instance.money < diffCost)
            {
                plantTreeField.SetValueWithoutNotify(plantedTrees.ToString());
                ShowError("Not enough money !");
                return;
            }

            GameManager.Instance.AddMoney(-diffCost);
            phase3Cost += diffCost;
            labelCostTree.text = "- " + (value * treeCost).ToString() + " $";
            labelCostTotal.text = "- " + phase3Cost.ToString() + " $";
            plantedTrees = value;
            plantTreeField.SetValueWithoutNotify(plantedTrees.ToString());
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