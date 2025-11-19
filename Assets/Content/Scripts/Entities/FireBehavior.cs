using UnityEngine;

public class FireBehvior : MonoBehaviour
{
    private Vector3 targetScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Destroy self
            Destroy(gameObject);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        // Debug.Log($"Fire collided with: {other.name}");

        if (transform.localScale.x < 0.4f)
        {
            // Destroy self
            Destroy(gameObject);
        }
            
        // Smoothly shrink towards target scale
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, 20f * Time.deltaTime);
    }
}
