using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum Phase { Phase1, Phase2, Phase3 }
    public Phase currentPhase;

    [Header("Timer")]
    public float preparationPhaseTime = 180f;
    public float actionPhaseTime = 300f;
    public float gameTime = 600f;
    public bool timerRunning = true;

    [Header("Score")]
    public int score = 0;

    [Header("Money")]
    public int money = 10000;

    [Header("Musique")]
    public AudioSource musicSource;
    public AudioClip phase_1Music;
    public AudioClip phase_2Music;
    public AudioClip phase_3Music;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        // Gestion du timer
        if (timerRunning)
        {
            gameTime -= Time.deltaTime;            
        }
        
        if (gameTime =< 0)
        {
            switch (currentPhase)
            {
                case Phase.Phase1:
                    SceneLoader.LoadScene("Phase_2_Scene");
                    break;
                case Phase.Phase2:
                    SceneLoader.LoadScene("Phase_3_Scene");
                    break;
                case Phase.Phase3:
                    SceneLoader.LoadScene("EndGame");
                    break;
                case default:
                    break;
            }
        }
    }

    // -------------------------------------------------------------------------
    //  MUSIQUE
    // -------------------------------------------------------------------------

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // On peut changer la musique automatiquement selon la scène
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("1")) currentPhase = Phase.Phase1;
        if (scene.name.Contains("2")) currentPhase = Phase.Phase2;
        if (scene.name.Contains("3")) currentPhase = Phase.Phase3;

        switch (currentPhase)
        {
            case Phase.Phase1:
                gameTime = preparationPhaseTime;
                PlayMusic(phase_1Music);
                break;

            case Phase.Phase2:
                gameTime = actionPhaseTime;
                PlayMusic(phase_2Music);
                break;

            case Phase.Phase3:
                gameTime = gameTime;
                PlayMusic(phase_3Music);
                break;
        }
        timerRunning = true;
    }

    // -------------------------------------------------------------------------
    //  TIMER
    // -------------------------------------------------------------------------

    public void ResetTimer()
    {
        gameTime = 600f;
    }

    public void DecreaseTimer(float amount)
    {
        gameTime -= amount;
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    // -------------------------------------------------------------------------
    //  SCORE
    // -------------------------------------------------------------------------

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void ResetScore()
    {
        score = 0;
    }
}
