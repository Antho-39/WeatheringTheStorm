using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Timer")]
    public float gameTime = 600f;
    public bool timerRunning = true;

    [Header("Score")]
    public int score = 0;

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
        if (scene.name.Contains("1"))
            PlayMusic(phase_1Music);
        else if (scene.name.Contains("2"))
            PlayMusic(phase_2Music);
        else if (scene.name.Contains("3"))
            PlayMusic(phase_2Music);
    }

    // -------------------------------------------------------------------------
    //  TIMER
    // -------------------------------------------------------------------------

    public void ResetTimer()
    {
        gameTime = 600f;
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
