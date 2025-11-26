using UnityEngine;
using System.Collections.Generic;

public class FireFighter : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 3f;

    [Header("Movement")]
    public float speed = 0.5f;
    public float extinguishDistance = 0.5f;
    public float avoidStrength = 2f;

    private List<GameObject> firesInRange = new List<GameObject>();
    private GameObject targetFire;

    public AudioClip extinguishAudio;

    private Rigidbody2D rb;
    private Vector2 avoidanceVector = Vector2.zero;
    public float rescanInterval = 1f;
    private float rescanTimer = 0f;

    private Vector3 initialPosition;

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();

        // SAVE initial position
        initialPosition = transform.position;

    }

    void Update()
    {
        if(GameManager.Instance.currentPhase != GameManager.Phase.Phase2)
        {
            return;
        }

        UpdateTarget();

        // Rescan periodically
        rescanTimer -= Time.deltaTime;
        if (rescanTimer <= 0f)
        {
            RescanForFires();
            rescanTimer = rescanInterval;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.currentPhase != GameManager.Phase.Phase2)
        {
            return;
        }
        if (targetFire != null)
            MoveTowardFire();
        else
            ReturnToBase();
    }

    // ---------------------------------------------------------
    // DETECTION
    // ---------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Fire"))
            firesInRange.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Fire"))
            firesInRange.Remove(other.gameObject);
    }

    void RescanForFires()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, LayerMask.GetMask("Fire"));

        foreach (var h in hits)
            if (!firesInRange.Contains(h.gameObject))
                firesInRange.Add(h.gameObject);

        firesInRange.RemoveAll(f => f == null);
    }

    // ---------------------------------------------------------
    // TARGETING
    // ---------------------------------------------------------
    void UpdateTarget()
    {
        firesInRange.RemoveAll(f => f == null);

        if (targetFire != null)
            return;

        targetFire = GetClosestFire();
    }

    GameObject GetClosestFire()
    {
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (var fire in firesInRange)
        {
            if (fire == null) continue;

            FireBehavior fireScript = fire.GetComponent<FireBehavior>();
            if (fireScript == null) continue;

            if (fireScript.isAssigned)
                continue;

            float dist = Vector2.Distance(transform.position, fire.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = fire;
            }
        }

        if (closest != null)
            closest.GetComponent<FireBehavior>().isAssigned = true;

        return closest;
    }

    // ---------------------------------------------------------
    // MOVEMENT
    // ---------------------------------------------------------
    void MoveTowardFire()
    {
        if (targetFire == null) return;

        Vector2 toFire = (targetFire.transform.position - transform.position);
        float dist = toFire.magnitude;

        if (dist <= extinguishDistance)
        {
            ExtinguishFire(targetFire);
            return;
        }

        Vector2 direction = (toFire).normalized;
        Vector2 finalDir = (direction + avoidanceVector).normalized;

        rb.MovePosition(rb.position + finalDir * speed * Time.fixedDeltaTime);

        avoidanceVector = Vector2.zero;
    }

    // ---------------------------------------------------------
    // RETURN TO BASE
    // ---------------------------------------------------------
    void ReturnToBase()
    {
        Vector2 toBase = (initialPosition - transform.position);
        float dist = toBase.magnitude;

        if (dist < 0.1f)
            return;

        Vector2 direction = toBase.normalized;
        Vector2 finalDir = (direction + avoidanceVector).normalized;

        rb.MovePosition(rb.position + finalDir * speed * Time.fixedDeltaTime);

        avoidanceVector = Vector2.zero;
    }

    // ---------------------------------------------------------
    // WATER COLLISION = SLIDE
    // ---------------------------------------------------------
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            foreach (var contact in collision.contacts)
                avoidanceVector += contact.normal * avoidStrength;
        }
    }

    // ---------------------------------------------------------
    // EXTINGUISH
    // ---------------------------------------------------------
    void ExtinguishFire(GameObject fire)
    {
        FireBehavior fireB = fire.GetComponent<FireBehavior>();

        bool isExtinguished = fireB.Extinguish();
        
        if(isExtinguished)
        {
            fireB.isAssigned = false;

            // Cleanup
            firesInRange.Remove(fire);
            targetFire = null;
        }
        AudioManager.Instance.PlaySFXAtPosition(extinguishAudio, transform.position);
    }
}
