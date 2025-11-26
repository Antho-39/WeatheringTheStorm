using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color baseColor;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        baseColor = sr.color;
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