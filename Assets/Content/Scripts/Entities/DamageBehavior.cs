using UnityEngine;

public class BurnBehavior : MonoBehaviour
{
    public Sprite damagedSprite;
    public LayerMask fireLayer;
    private SpriteRenderer entityRenderer;
    private float fireDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        entityRenderer = GetComponent<SpriteRenderer>();
        fireDamage = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics2D.OverlapCircle(transform.position, 1f, fireLayer))
        {
            fireDamage += 0.05f;
        }

        if(fireDamage >= 100f)
        {
            entityRenderer.sprite = damagedSprite;
        }
    }
}
