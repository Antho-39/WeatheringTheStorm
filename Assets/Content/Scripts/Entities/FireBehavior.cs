
using UnityEngine;

public class FireBehavior : MonoBehaviour
{
    public bool isAssigned;
    public float spreadDelay = 5f;
    private Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);

    public float NextSpreadTime { get; private set; }

    private float scoreInterval = 10f;
    private float nextScoreTime;


    void Start()
    {
        Initialize();
        nextScoreTime = Time.time + scoreInterval;
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
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, 20f * Time.deltaTime);
        return transform.localScale.x < 0.2f;
    }

    private void OnDestroy()
    {
        FireManager.Instance.UnregisterFire(this);
    }
}
