using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum Phase { MenuPhase, Phase1, Phase2, Phase3 }
    public Phase currentPhase;

    [Header("Timer")]
    public float preparationPhaseTime;
    public float actionPhaseTime;
    public float gameTime;
    public bool timerRunning = false;

    [Header("Score")]
    public int score;
    public int phase2scoreBonus;
    public int treesDestroyed;
    public int homesDestroyed;
    public int buildingsDestroyed;
    public int rescuedVictim;
    public int notRescuedVictim;

    [Header("Money")]
    public int money;

    [Header("Musique")]
    public AudioSource musicSource;
    public AudioClip phase_1Music;
    public AudioClip phase_2Music;
    public AudioClip phase_3Music;

    public List<PlacedObjectData> placedObjects = new ();
    public PlaceableObjectDatabase database;

    private bool isMuted;

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
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Start()
    {
        Init();
        musicSource = AudioManager.Instance.audioSource;
    }

    private void Update()
    {
        // Gestion du timer
        if (timerRunning)
        {
            gameTime -= Time.deltaTime;            
        }
        
        if (gameTime <= 0)
        {
            switch (currentPhase)
            {
                case Phase.Phase1:
                    StopMusic();
                    SceneLoader.LoadScene("Phase_2_Scene");
                    break;
                case Phase.Phase2:
                    StopMusic();
                    SceneLoader.LoadScene("Phase_3_Scene");
                    break;
                case Phase.Phase3:
                    StopMusic();
                    SceneLoader.LoadScene("EndGame");
                    break;
                default:
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

    public void PlayPhaseMusic()
    {
        switch (currentPhase)
        {
            case Phase.MenuPhase:
                // musicSource.volume = 0.2f;
                PlayMusic(phase_1Music);
                break;

            case Phase.Phase1:
                // We now start phase 1 music in main menu and keep it persistent into phase 1
                // PlayMusic(phase_1Music);
                break;

            case Phase.Phase2:
                // musicSource.volume = 0.1f;
                PlayMusic(phase_2Music);
                break;

            case Phase.Phase3:
                PlayMusic(phase_3Music);
                break;
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Menu")) currentPhase = Phase.MenuPhase;
        if (scene.name.Contains("1")) currentPhase = Phase.Phase1;
        if (scene.name.Contains("2")) currentPhase = Phase.Phase2;
        if (scene.name.Contains("3")) currentPhase = Phase.Phase3;
        
        // Keep phase 1 music playing from menu into phase 1
        switch (currentPhase)
        {
            case Phase.Phase1:
                gameTime = preparationPhaseTime;
                break;

            case Phase.Phase2:
                StopMusic();
                gameTime = actionPhaseTime;
                break;

            case Phase.Phase3:
                StopMusic();
                gameTime = gameTime;
                break;
            default:
                break;
        }
        timerRunning = true;
    }

    // -------------------------------------------------------------------------
    //  TIMER
    // -------------------------------------------------------------------------

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
    //  MONEY
    // -------------------------------------------------------------------------

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void ResetMoney()
    {
        money = 10000;
    }


    // -------------------------------------------------------------------------
    //  SCORE
    // -------------------------------------------------------------------------

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void AddPhase2Score(int amount)
    {
        phase2scoreBonus += amount;
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void Init()
    {
        money = 10000;
        score = 0;
        phase2scoreBonus = 1000;
        gameTime = preparationPhaseTime;
        treesDestroyed = 0;
        buildingsDestroyed = 0;
        rescuedVictim = 0;
        notRescuedVictim = 0;
        timerRunning = false;
        isMuted = false;
    }

    public void AddRescuedVictim()
    {
        rescuedVictim += 1;
    }
    public void AddNotRescuedVictim()
    {
        notRescuedVictim += 1;
    }

    public bool GetMuteState()
    { 
        return isMuted;
    }

    public void SetMuteState(bool mute)
    {
        isMuted = mute;
        this.GetComponent<AudioSource>().mute = isMuted;
    }
}

[System.Serializable]
public class PlacedObjectData
{
    public string id;
    public Vector2 position;
    public float rotation;
}