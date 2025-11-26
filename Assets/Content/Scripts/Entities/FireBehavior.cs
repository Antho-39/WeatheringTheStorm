
using UnityEngine;

public class FireBehavior : MonoBehaviour
{
    public bool isAssigned;
    public float spreadDelay = 5f;
    private Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);

    public float NextSpreadTime { get; private set; }

    private float scoreInterval = 10f;
    private float nextScoreTime;

    private AudioSource fireLoopSource;

    [Header("Audio")]
    public AudioClip fireLoopSound;
    public AudioClip fireExtinguishSound;
    public float maxSoundDistance = 12f;

    void Start()
    {
        Initialize();
        nextScoreTime = Time.time + scoreInterval;

        fireLoopSource = gameObject.AddComponent<AudioSource>();
        fireLoopSource.loop = true;
        fireLoopSource.playOnAwake = false;
        fireLoopSource.spatialBlend = 1f;
        fireLoopSource.minDistance = 1.5f;
        fireLoopSource.maxDistance = maxSoundDistance;
        fireLoopSource.rolloffMode = AudioRolloffMode.Linear;
        fireLoopSource.volume = 0.6f;

        fireLoopSource.clip = fireLoopSound;

        fireLoopSource.Play();
    }

    public void Initialize()
    {
        isAssigned = false;
        transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
        ResetSpreadTimer();
        FireManager.Instance.RegisterFire(this);
    }

    private void OnParticleCollision(GameObject other)
    {
        Extinguish();
    }

    public void ResetSpreadTimer()
    {
        NextSpreadTime = Time.time + spreadDelay;
    }

    void Update()
    {
        if (fireLoopSource == null) return;

        float dist = Vector2.Distance(Camera.main.transform.position, transform.position);
        float t = Mathf.Clamp01(1f - dist / maxSoundDistance);

        // Évite les feux TROP proches (empêche volume = 1 pile)
        fireLoopSource.volume = Mathf.Lerp(0f, 0.6f, t);

        if (transform.localScale.x >= 0.2f && Time.time >= nextScoreTime)
        {
            GameManager.Instance.AddPhase2Score(-10);
            nextScoreTime = Time.time + scoreInterval;
        }

        if (transform.localScale.x < 0.2f)
        {
            FireManager.Instance.UnregisterFire(this);
            Destroy(gameObject);
        }
    }

    public bool Extinguish()
    {
        if (fireLoopSource != null)
            fireLoopSource.Stop();

        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, 20f * Time.deltaTime);

        bool isFireExtinguished = transform.localScale.x < 0.2f;
        if (isFireExtinguished)
        {
            AudioManager.Instance.PlaySFXAtPosition(
                fireExtinguishSound,
                transform.position,
                12f
            );
        }
        return isFireExtinguished;
    }

    private void OnDestroy()
    {
        FireManager.Instance.UnregisterFire(this);
    }
}
