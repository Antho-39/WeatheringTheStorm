using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireBehvior : MonoBehaviour
{
    public LayerMask FlammableLayers;
    public LayerMask InFlammableLayers;
    private float fireSpreadSpeed = 5f;
    private float minDistance = 0.2f;
    private Vector3 targetScale;
    private Vector2 fireCenter;
    private GameObject prefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetScale = new Vector3(0.1f, 0.1f, 0.1f);
        fireCenter = transform.position;
        prefab = gameObject;
        InvokeRepeating(nameof(Propagation), 5, fireSpreadSpeed);
        InvokeRepeating(nameof(DecreasePlayerScore), 10, 10);
        InvokeRepeating(nameof(Grow), 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Propagation()
    {
        Vector2 targetSpawn = (Random.insideUnitCircle * 1.5f) + fireCenter;

        // Make 20 attempts to spawn more fire within the following restrictions
        for (int attempts = 0; attempts < 20; attempts++)
        {            
            // The random location must be a minimum distance from the center of the current fire
            if (Vector2.Distance(targetSpawn, fireCenter) > minDistance)
            {
                // The random location must not overlap with something inflammable
                // and must overlap with something flammable
                if (!Physics2D.OverlapCircle(targetSpawn, 0.2f, InFlammableLayers)
                    && Physics2D.OverlapCircle(targetSpawn, 0.2f, FlammableLayers))
                {
                    // Spawn fire
                    Instantiate(prefab, targetSpawn, quaternion.identity);
                    break;
                }
            }
        }
    }

    private void Grow()
    {
        // Smoothly grow back towards original scale
        transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(0.65f, 0.65f, 0.65f), 20f * Time.deltaTime);
    }

    private void OnParticleCollision(GameObject other)
    {
        // Debug.Log($"Fire collided with: {other.name}");

        if (transform.localScale.x < 0.2f)
        {
            // Destroy self
            Destroy(gameObject);
        }
            
        // Smoothly shrink towards target scale
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, 20f * Time.deltaTime);
    }

    private void DecreasePlayerScore()
    {
        GameManager.Instance.AddScore(-10);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // // Check if the collided object has the tag "Enemy"
        // if (collision.gameObject.CompareTag("Enemy"))
        // {
        //     // Destroy self
        //     Destroy(gameObject);
        // }
    }
}
