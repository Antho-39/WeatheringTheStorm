using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color baseColor;
    private bool isSelected = false;

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
        isSelected = selected;

        if (selected)
            sr.color = Color.yellow;
        else
            sr.color = baseColor;
    }
}