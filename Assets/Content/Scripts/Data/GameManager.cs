using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum Phase { Phase1, Phase2, Phase3 }
    public Phase currentPhase;

    [Header("Timer")]
    public float preparationPhaseTime = 180f;
    public float actionPhaseTime = 300f;
    public float gameTime;
    private float scoreTime;
    public bool timerRunning = false;

    [Header("Score")]
    public int score;
    public int treesDestroyed;
    public int buildingsDestroyed;

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

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Start()
    {
        Init();
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
            case Phase.Phase1:
                PlayMusic(phase_1Music);
                break;

            case Phase.Phase2:
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

    // On peut changer la musique automatiquement selon la scène
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("1")) currentPhase = Phase.Phase1;
        if (scene.name.Contains("2")) currentPhase = Phase.Phase2;
        if (scene.name.Contains("3")) currentPhase = Phase.Phase3;
        StopMusic();

        switch (currentPhase)
        {
            case Phase.Phase1:
                gameTime = preparationPhaseTime;
                break;

            case Phase.Phase2:
                scoreTime += gameTime;
                gameTime = actionPhaseTime;
                break;

            case Phase.Phase3:
                scoreTime += gameTime;
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

    public void ResetScore()
    {
        score = 0;
    }

    private void Init()
    {
        money = 10000;
        score = 0;
        gameTime = preparationPhaseTime;
        treesDestroyed = 0;
        buildingsDestroyed = 0;
        timerRunning = false;
        isMuted = false;
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