using UnityEngine;

public class HelicopterShadow : MonoBehaviour
{

    [Header("Réglages")]
    public float maxHeight = 10f;
    public float baseScale = 1f; 
    public float minScaleFactor = 0.3f; 
    public float maxOpacity = 0.7f;
    public float minOpacity = 0.2f;

    [Header("Dynamique")]
    public float helicopterHeight = 0f;

    private Transform helicopter;
    private SpriteRenderer shadowRenderer;

    void Start()
    {
        helicopter = GetComponentInParent<Transform>();
        shadowRendere = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // 1. Suivre la position au sol
        Vector3 pos = helicopter.position;
        pos.z = transform.position.z; // garder l'ombre au même niveau visuel
        transform.position = pos;

        // 2. Calcul du ratio de hauteur
        float t = Mathf.Clamp01(helicopterHeight / maxHeight);

        // 3. Échelle (plus petite quand l'hélico monte)
        float scale = Mathf.Lerp(baseScale, baseScale * minScaleFactor, t);
        transform.localScale = new Vector3(scale, scale, 1f);

        // 4. Opacité (plus claire quand l'hélico est haut)
        Color c = shadowRenderer.color;
        c.a = Mathf.Lerp(maxOpacity, minOpacity, t);
        shadowRenderer.color = c;
    }
}
