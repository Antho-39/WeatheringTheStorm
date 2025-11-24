using UnityEngine;
using System.Collections.Generic;

public class FireFighter : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 5f;

    [Header("Movement")]
    public float speed = 0.5f;
    public float extinguishDistance = 0.5f;
    public float avoidStrength = 2f;

    private List<GameObject> firesInRange = new List<GameObject>();
    private GameObject targetFire;

    private Rigidbody2D rb;
    private Vector2 avoidanceVector = Vector2.zero;
    public float rescanInterval = 1f;
    private float rescanTimer = 0f;

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
        /*
        var trigger = GetComponent<CircleCollider2D>();
        if (trigger != null)
        {
            trigger.isTrigger = true;
            trigger.radius = detectionRadius;
        }*/
    }

    void Update()
    {
        UpdateTarget();
        
        // Rescan every X seconds to catch new fires spawning inside detection zone
        rescanTimer -= Time.deltaTime;
        if (rescanTimer <= 0f)
        {
            RescanForFires();
            rescanTimer = rescanInterval;
        }
    }

    void FixedUpdate()
    {
        if (targetFire != null)
            MoveTowardFire();
    }

    // ---------------------------------------------------------
    // DETECTION
    // ---------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Fire"))
        {
            firesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Fire"))
        {
            firesInRange.Remove(other.gameObject);
        }
    }

    void RescanForFires()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, LayerMask.GetMask("Fire"));

        foreach (var h in hits)
        {
            if (!firesInRange.Contains(h.gameObject))
                firesInRange.Add(h.gameObject);
        }

        // Clean destroyed fires
        firesInRange.RemoveAll(f => f == null);
    }

    // ---------------------------------------------------------
    // TARGETING
    // ---------------------------------------------------------
    void UpdateTarget()
    {
        // Retirer tous les feux détruits
        firesInRange.RemoveAll(f => f == null);

        if (firesInRange.Count == 0)
        {
            targetFire = null;
            return;
        }

        targetFire = GetClosestFire();
    }

    GameObject GetClosestFire()
    {
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (var fire in firesInRange)
        {
            if (fire == null) continue;
            /*
            FireBehavior fireScript = fire.GetComponent<FireBehavior>();

            if (fireScript == null) continue;
            
            if (fireScript.isAssigned)
                continue;
            */
            float dist = Vector2.Distance(transform.position, fire.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = fire;
            }
        }
        /*
        if (closest != null)
        {
            closest.GetComponent<FireBehavior>().isAssigned = true;
        }
        */

        return closest;
    }

    // ---------------------------------------------------------
    // MOVEMENT + WATER AVOIDANCE
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

        Vector2 direction = toFire.normalized;

        // Combine normal movement + avoidance
        Vector2 finalDir = (direction + avoidanceVector).normalized;

        rb.MovePosition(rb.position + finalDir * speed * Time.fixedDeltaTime);

        // Reset avoidance each frame (will update if colliding)
        avoidanceVector = Vector2.zero;
    }

    // ---------------------------------------------------------
    // WATER COLLISION = AVOID / SLIDE
    // ---------------------------------------------------------
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            // Compute push-away vector from the water surface normal
            foreach (var contact in collision.contacts)
            {
                avoidanceVector += contact.normal * avoidStrength;
            }
        }
    }

    void ExtinguishFire(GameObject fire)
    {
        FireBehavior fireB = fire.GetComponent<FireBehavior>();
        // Appeler un script Fire si nécessaire
        bool isExtinguished = fireB.Extinguish();

        if (isExtinguished)
        {
            /*
            if (fireB != null)
                fireB.isAssigned = false;
            */
            firesInRange.Remove(fire);
            targetFire = null;
        }
    }
}