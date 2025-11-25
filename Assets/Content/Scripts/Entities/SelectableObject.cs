using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color baseColor;
    public Collider2D ExtinguishCollider;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        baseColor = sr.color;
    }
    
    void OnEnable()
    {
        if(ExtinguishCollider == null)
            return;
        if(GameManager.Instance.currentPhase == GameManager.Phase.Phase2)
        {
            ExtinguishCollider.enabled = true;
        }
        else
        {
            ExtinguishCollider.enabled = false;
        }
    }

    void OnMouseDown()
    {
        if (PlacementManager.Instance.isPlacing) return;

        PlacementManager.Instance.SelectObjectForMove(this);
    }
    
    public void SetSelected(bool selected)
    {
        //isSelected = selected;
        sr.color = selected ? Color.yellow : baseColor;
    }
}