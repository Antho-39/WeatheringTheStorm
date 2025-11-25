using UnityEngine;

public class BurnBehavior : MonoBehaviour
{
    public enum EntityType
    {
        Tree,
        Building,
        Home
    }

    public EntityType entityType;
    public Sprite damagedSprite;
    public LayerMask fireLayer;
    public float entityHealth;
    public int ScorePenalty;
    private SpriteRenderer entityRenderer;
    private BoxCollider2D entityCollider;
    private float colliderAverageRadius;
    private bool burned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        entityRenderer = GetComponent<SpriteRenderer>();
        entityCollider = GetComponent<BoxCollider2D>();
        burned = false;

        // Using average radius of the box collider extends because using the actual overlap box function won't work
        colliderAverageRadius = (entityCollider.bounds.extents.x + entityCollider.bounds.extents.z) * 0.5f;
        // print(entityCollider.size);
    }

    // Update is called once per frame
    void Update()
    {
        // The box overlap doesn't work and I have no idea why (seems to be super huge but shouldn't be?)
        // if (Physics2D.OverlapBox(transform.position, entityCollider.size, fireLayer))
        // {
        //     entityHealth -= 0.05f;
        // }
        // Same result when I deliberately use small box values
        // if (Physics2D.OverlapBox(transform.position, new Vector2(1f, 1f), fireLayer))
        // {
        //     entityHealth -= 0.05f;
        // }
        
        // Overlap circle works fine for some reason lol
        if (Physics2D.OverlapCircle(transform.position, colliderAverageRadius, fireLayer))
        {
            entityHealth -= 0.05f;
        }

        if(entityHealth <= 0f && burned == false)
        {
            GameManager.Instance.AddScore(ScorePenalty);
            entityRenderer.sprite = damagedSprite;

            if (entityType == EntityType.Tree)
            {
                GameManager.Instance.treesDestroyed++;
            }
            else if (entityType == EntityType.Home)
            {
                GameManager.Instance.homesDestroyed++;
            }
            else if (entityType == EntityType.Building)
            {
                GameManager.Instance.buildingsDestroyed++;
            }
            burned = true;
        }
    }

    // Tried to use this to see the broken box collider but also doesn't work?
    void OnDrawGizmos()
    {
        if (entityCollider == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, entityCollider.size);
        // Gizmos.DrawWireSphere(transform.position, colliderAverageRadius);
    }
}
